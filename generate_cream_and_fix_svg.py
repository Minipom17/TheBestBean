import numpy as np
from PIL import Image
import os

base_dir = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e'
img_path = os.path.join(base_dir, r'.user_uploaded\media_1789955235652.png')
out_img_path = os.path.join(base_dir, r'cream_card.png')

# 1. Color mapping for Cream card
img = Image.open(img_path).convert('RGB')
arr = np.array(img).astype(float)
gray = np.dot(arr, [0.2989, 0.5870, 0.1140]) 

x = [0, 25, 170, 215, 255]
r_y = [182, 182, 253, 255, 255]
g_y = [199, 199, 246, 255, 255]
b_y = [68, 68, 227, 255, 255]

new_r = np.interp(gray, x, r_y)
new_g = np.interp(gray, x, g_y)
new_b = np.interp(gray, x, b_y)

new_arr = np.stack([new_r, new_g, new_b], axis=2).astype(np.uint8)

# Add subtle noise for "textured white" effect
np.random.seed(42)
noise = np.random.normal(0, 4, new_arr.shape).astype(float)
mask = gray > 100
mask = np.stack([mask, mask, mask], axis=2)
textured_arr = np.clip(new_arr.astype(float) + noise * mask, 0, 255).astype(np.uint8)

textured_img = Image.fromarray(textured_arr)
textured_img.save(out_img_path)
print("Saved cream card to", out_img_path)

# 2. Fix SVG
svg_path = os.path.join(base_dir, r'farmer_vector.svg')
with open(svg_path, 'r', encoding='utf-8') as f:
    svg_content = f.read()

if 'viewBox' not in svg_content:
    svg_content = svg_content.replace('width="606" height="505"', 'viewBox="0 0 606 505" width="100%" height="100%"')

with open(svg_path, 'w', encoding='utf-8') as f:
    f.write(svg_content)
print("Fixed SVG file")
