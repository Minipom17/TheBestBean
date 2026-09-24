from PIL import Image, ImageDraw
import os

img_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789948688899.png'
out_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\test_boxes.png'

img = Image.open(img_path).convert('RGB')
draw = ImageDraw.Draw(img)

# Main Box approx
draw.rectangle([68, 316, 500, 520], outline=(0, 255, 0), width=3)

# Line boxes approx
boxes = [
    [440, 50, 545, 85],     # SCA
    [440, 105, 545, 140],    # MASL
    [185, 595, 515, 630],   # Region
    [155, 675, 515, 710],   # Farm
    [210, 755, 515, 790],   # Producer
    [200, 835, 515, 870],   # Varietal
    [190, 915, 515, 950]    # Process
]

for b in boxes:
    draw.rectangle(b, fill=(255, 255, 255))

img.save(out_path)
print("Saved to", out_path)
