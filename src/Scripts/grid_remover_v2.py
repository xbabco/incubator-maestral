
import cv2
import numpy as np

def remove_grid_with_trackbar(image_path, output_path):
    """
    Detects and removes the grid from an image with trackbars for parameter tuning.

    Args:
        image_path (str): The path to the input image.
        output_path (str): The path to save the output image.
    """
    # Load the image
    src = cv2.imread(image_path, cv2.IMREAD_COLOR)

    # Check if image is loaded fine
    if src is None:
        print(f'Error opening image: {image_path}')
        return

    # Transform source image to gray if it is not already
    if len(src.shape) != 2:
        gray = cv2.cvtColor(src, cv2.COLOR_BGR2GRAY)
    else:
        gray = src

    # Apply adaptiveThreshold at the bitwise_not of gray
    gray_not = cv2.bitwise_not(gray)
    bw = cv2.adaptiveThreshold(gray_not, 255, cv2.ADAPTIVE_THRESH_MEAN_C,
                                cv2.THRESH_BINARY, 15, -2)

    # Create a window to display the result
    cv2.namedWindow('Result')

    # Create trackbars for horizontal and vertical size
    cv2.createTrackbar('H_Size', 'Result', 30, 100, lambda x: None)
    cv2.createTrackbar('V_Size', 'Result', 30, 100, lambda x: None)
    cv2.setTrackbarMin('H_Size', 'Result', 1)
    cv2.setTrackbarMin('V_Size', 'Result', 1)

    result = None

    while True:
        # Get the current trackbar positions
        horizontal_size = cv2.getTrackbarPos('H_Size', 'Result')
        vertical_size = cv2.getTrackbarPos('V_Size', 'Result')

        # Create the images that will use to extract the horizontal and vertical lines
        horizontal = np.copy(bw)
        vertical = np.copy(bw)

        # Create structure element for extracting horizontal lines
        horizontalStructure = cv2.getStructuringElement(cv2.MORPH_RECT, (horizontal_size, 1))

        # Apply morphology operations to detect horizontal lines
        horizontal = cv2.erode(horizontal, horizontalStructure)
        horizontal = cv2.dilate(horizontal, horizontalStructure)

        # Create structure element for extracting vertical lines
        verticalStructure = cv2.getStructuringElement(cv2.MORPH_RECT, (1, vertical_size))

        # Apply morphology operations to detect vertical lines
        vertical = cv2.erode(vertical, verticalStructure)
        vertical = cv2.dilate(vertical, verticalStructure)

        # Combine horizontal and vertical lines to get the grid mask
        grid_mask = cv2.add(horizontal, vertical)

        # Invert the grid mask
        grid_mask_inv = cv2.bitwise_not(grid_mask)

        # Remove the grid from the original image using the mask
        result_masked = cv2.bitwise_and(gray, gray, mask=grid_mask_inv)
        
        # Invert the result back to original color
        result = cv2.bitwise_not(result_masked)

        # Show the result
        cv2.imshow('Result', result)

        # Wait for 27ms, if the user presses 'ESC' or 'q', exit the loop
        key = cv2.waitKey(27)
        if key == 27 or key == ord('q'):
            break

    # Save the final result if it exists
    if result is not None:
        cv2.imwrite(output_path, result)
        print(f"Grid removed and image saved to {output_path}")

    # Destroy all windows
    cv2.destroyAllWindows()

if __name__ == '__main__':
    remove_grid_with_trackbar('input-img.jpg', 'output_no_grid_v2.png')
