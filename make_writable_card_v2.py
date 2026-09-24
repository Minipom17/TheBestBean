import numpy as np
from PIL import Image, ImageDraw
import cv2
import os

img_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789948688899.png'
out_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\writable_card.png'

# 1. Color mapping preserving AA
img = Image.open(img_path).convert('RGB')
arr = np.array(img).astype(float)
gray = np.dot(arr, [0.2989, 0.5870, 0.1140]) 

x = [0, 25, 45, 178, 255]
r_y = [38, 38, 255, 255, 255]
g_y = [19, 19, 255, 255, 255]
b_y = [44, 44, 255, 255, 255]

new_r = np.interp(gray, x, r_y)
new_g = np.interp(gray, x, g_y)
new_b = np.interp(gray, x, b_y)

new_arr = np.stack([new_r, new_g, new_b], axis=2).astype(np.uint8)

# 2. Find lines and draw white boxes
cv_img = cv2.imread(img_path)
text_color = np.array([68, 199, 182]) # BGR
dist_text = np.linalg.norm(cv_img.astype(float) - text_color, axis=2)
text_mask = dist_text < 60

kernel = np.ones((1, 30), np.uint8)
horizontal_lines = cv2.morphologyEx(text_mask.astype(np.uint8)*255, cv2.MORPH_OPEN, kernel)

contours, _ = cv2.findContours(horizontal_lines, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

pil_img = Image.fromarray(new_arr)
draw = ImageDraw.Draw(pil_img)

for c in contours:
    x_c, y_c, w, h = cv2.boundingRect(c)
    if w > 50:
        draw.rectangle([x_c, y_c - 35, x_c + w, y_c + h], fill=(255, 255, 255))

pil_img.save(out_path)
print("Saved to", out_path)
