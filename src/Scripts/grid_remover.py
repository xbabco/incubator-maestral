
import cv2
import numpy as np

def remove_grid(image_path, output_path):
    """
    Detects and removes the grid from an image.

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
    gray = cv2.bitwise_not(gray)
    bw = cv2.adaptiveThreshold(gray, 255, cv2.ADAPTIVE_THRESH_MEAN_C,
                                cv2.THRESH_BINARY, 15, -2)

    # Create the images that will use to extract the horizontal and vertical lines
    horizontal = np.copy(bw)
    vertical = np.copy(bw)

    # Specify size on horizontal axis
    cols = horizontal.shape[1]
    horizontal_size = cols // 30

    # Create structure element for extracting horizontal lines
    horizontalStructure = cv2.getStructuringElement(cv2.MORPH_RECT, (horizontal_size, 1))

    # Apply morphology operations to detect horizontal lines
    horizontal = cv2.erode(horizontal, horizontalStructure)
    horizontal = cv2.dilate(horizontal, horizontalStructure)

    # Specify size on vertical axis
    rows = vertical.shape[0]
    verticalsize = rows // 30

    # Create structure element for extracting vertical lines
    verticalStructure = cv2.getStructuringElement(cv2.MORPH_RECT, (1, verticalsize))

    # Apply morphology operations to detect vertical lines
    vertical = cv2.erode(vertical, verticalStructure)
    vertical = cv2.dilate(vertical, verticalStructure)

    # Combine horizontal and vertical lines to get the grid mask
    grid_mask = cv2.add(horizontal, vertical)

    # Invert the grid mask
    grid_mask_inv = cv2.bitwise_not(grid_mask)

    # Remove the grid from the original image using the mask
    result = cv2.bitwise_and(gray, gray, mask=grid_mask_inv)
    
    # Invert the result back to original color
    result = cv2.bitwise_not(result)


    # Save the result
    cv2.imwrite(output_path, result)
    print(f"Grid removed and image saved to {output_path}")

if __name__ == '__main__':
    remove_grid('input-img.jpg', 'output_no_grid.png')
