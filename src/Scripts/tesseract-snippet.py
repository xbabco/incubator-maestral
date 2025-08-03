from PIL import Image
import pytesseract, csv, re

# 1. Load your scan
img = Image.open("input-img.jpg")

# 2. OCR just digits (and ignore letters/check-marks)
data = pytesseract.image_to_string(img, config="--psm 6 digits")

# 3. Extract all numbers
nums = re.findall(r"\d+", data)

# 4. Group into rows of 9 (one per category)
rows = [nums[i:i+9] for i in range(0, len(nums), 9)]

# 5. Write CSV
with open("output-data.csv", "w", newline="") as f:
    writer = csv.writer(f)
    writer.writerow(["Style","Collage","Monochrome","Neon",
                     "Trick I","Trick II","Exposure","Trick III","Trick IV"])
    writer.writerows(rows)
