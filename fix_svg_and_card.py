import numpy as np
from PIL import Image
import os
import cv2

base_dir = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e'
img_path = os.path.join(base_dir, r'.user_uploaded\media_1789955235652.png')
out_img_path = os.path.join(base_dir, r'white_purple_card.png')

# 1. Color mapping for White/Purple card
img = Image.open(img_path).convert('RGB')
arr = np.array(img).astype(float)
gray = np.dot(arr, [0.2989, 0.5870, 0.1140]) 

x = [0, 25, 170, 215, 255]
r_y = [38, 38, 255, 240, 240]
g_y = [19, 19, 255, 235, 235]
b_y = [44, 44, 255, 245, 245]

new_r = np.interp(gray, x, r_y)
new_g = np.interp(gray, x, g_y)
new_b = np.interp(gray, x, b_y)

new_arr = np.stack([new_r, new_g, new_b], axis=2).astype(np.uint8)
new_img = Image.fromarray(new_arr)
new_img.save(out_img_path)
print("Saved white card to", out_img_path)

# 2. Convert PNG icon to standard SVG using OpenCV
icon_path = os.path.join(base_dir, r'.user_uploaded\media_1789957535127.png')
svg_path = os.path.join(base_dir, r'farmer_vector_fixed.svg')

icon = cv2.imread(icon_path, cv2.IMREAD_GRAYSCALE)
# Add a white border to ensure contours don't touch the edge weirdly
icon = cv2.copyMakeBorder(icon, 10, 10, 10, 10, cv2.BORDER_CONSTANT, value=255)
_, binary = cv2.threshold(icon, 127, 255, cv2.THRESH_BINARY_INV)

contours, hierarchy = cv2.findContours(binary, cv2.RETR_TREE, cv2.CHAIN_APPROX_TC89_KCOS)

h, w = icon.shape

svg_elements = []
if hierarchy is not None:
    for i, cnt in enumerate(contours):
        approx = cv2.approxPolyDP(cnt, 0.6, True) # 0.6 pixel smoothing
        if len(approx) < 3: continue
        
        points = " ".join([f"{p[0][0]},{p[0][1]}" for p in approx])
        
        parent_idx = hierarchy[0][i][3]
        depth = 0
        p = parent_idx
        while p != -1:
            depth += 1
            p = hierarchy[0][p][3]
            
        color = "black" if depth % 2 == 0 else "white"
        
        svg_elements.append(f'  <polygon points="{points}" fill="{color}" />\n')

with open(svg_path, 'w') as f:
    f.write(f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {w} {h}" width="100%" height="100%">\n')
    f.writelines(svg_elements)
    f.write('</svg>')
print("Saved SVG using contours")
