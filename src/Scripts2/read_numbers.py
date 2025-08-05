
import cv2
import pytesseract
import numpy as np


def show_wait_destroy(winname, img):
    cv2.imshow(winname, img)
    cv2.moveWindow(winname, 500, 0)
    cv2.waitKey(0)
    cv2.destroyWindow(winname)


def main():
    """
    Main function to process the image and extract numbers.
    """
    # Load the image
    image = cv2.imread("input-img.jpg")

    # Convert to grayscale
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    # show_wait_destroy("gray", gray)

    # imgBlured = cv2.GaussianBlur(gray, (21, 21), 3)
    # show_wait_destroy("imgBlured", imgBlured)

    # Apply binary threshold
    thresh = cv2.threshold(gray, 128, 255, cv2.THRESH_BINARY_INV, cv2.THRESH_OTSU)[1]

    # Use Hough Line Transform to detect lines
    lines = cv2.HoughLinesP(thresh, 1, np.pi / 180, 100, minLineLength=100, maxLineGap=10)

    # Draw the detected lines on the image
    for line in lines:
        x1, y1, x2, y2 = line[0]
        cv2.line(image, (x1, y1), (x2, y2), (36, 255, 12), 2)

    # --- 1. Create a clean grid mask ---
    # Create a new black image to draw the lines on
    grid_mask = np.zeros(image.shape[:2], dtype=np.uint8)

    # 1. Create a blank mask the same size as your input
    hough_mask = np.zeros_like(gray)

    # 2. For each line, draw it into the mask in white
    for x1, y1, x2, y2 in lines.reshape(-1,4):
        cv2.line(hough_mask, (x1, y1), (x2, y2), 255, thickness=2)

    # show_wait_destroy("hough_mask", hough_mask)

    # 3. Merge that mask into your grid_mask (which is also single-channel)
    #    You can use bitwise_or so any Hough-detected line will be included.
    grid_mask = cv2.bitwise_or(grid_mask, hough_mask)
    # show_wait_destroy("grid_mask", grid_mask)

    # Multi-pass horizontal line detection
    for kernel_size in [25, 15]:
        horizontal_kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (kernel_size, 1))
        detect_horizontal = cv2.morphologyEx(thresh, cv2.MORPH_OPEN, horizontal_kernel, iterations=2)
        cnts = cv2.findContours(detect_horizontal, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        cnts = cnts[0] if len(cnts) == 2 else cnts[1]
        for c in cnts:
            cv2.drawContours(grid_mask, [c], -1, 255, 2)

    # Multi-pass vertical line detection
    for kernel_size in [25, 15]:
        vertical_kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (1, kernel_size))
        detect_vertical = cv2.morphologyEx(thresh, cv2.MORPH_OPEN, vertical_kernel, iterations=2)
        cnts = cv2.findContours(detect_vertical, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        cnts = cnts[0] if len(cnts) == 2 else cnts[1]
        for c in cnts:
            cv2.drawContours(grid_mask, [c], -1, 255, 2)

    # --- 2. Find cell contours ---
    cnts = cv2.findContours(grid_mask, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    cnts = cnts[0] if len(cnts) == 2 else cnts[1]

    # Sort contours from top-to-bottom, left-to-right
    (cnts, boundingBoxes) = zip(*sorted(zip(cnts, [cv2.boundingRect(c) for c in cnts]),
        key=lambda b: (b[1][1], b[1][0])))

    # --- 3. OCR on each cell ---
    results = []
    for c in cnts:
        area = cv2.contourArea(c)
        # Filter out contours that are too small or too large to be a cell
        if area > 1000 and area < 10000:
            x, y, w, h = cv2.boundingRect(c)

            # Extract the cell from the original grayscale image
            cell = gray[y:y+h, x:x+w]

            # Pad the cell to improve OCR accuracy
            cell = cv2.copyMakeBorder(cell, 10, 10, 10, 10, cv2.BORDER_CONSTANT, value=[255,255,255])

            # Apply a gentle blur to the cell to smooth out noise
            cell = cv2.GaussianBlur(cell, (3, 3), 0)

            # Use Tesseract to extract text
            # --psm 7 treats the image as a single line of text
            text = pytesseract.image_to_string(
                cell,
                config='--psm 7 --oem 3 -c tessedit_char_whitelist=0123456789'
            ).strip()

            if text:
                results.append(text)

    # --- Placeholder for cell extraction ---

    # --- Placeholder for OCR on each cell ---

    # Save the result for inspection
    cv2.imwrite("output_with_lines.jpg", image)
    print("Image processed successfully (placeholders).")

    # --- 4. Display the results ---
    print("Extracted Numbers:")
    # We will assume a table with 8 columns for formatting the output.
    # This is a simplification and might need adjustment.
    col_count = 8
    for i in range(0, len(results), col_count):
        print(" ".join(f"{num: >4}" for num in results[i:i+col_count]))

if __name__ == "__main__":
    main()
