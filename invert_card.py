import numpy as np
from PIL import Image
import os

img2_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789940750277.png'
out_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\inverted_card.png'

img = Image.open(img2_path).convert('RGB')
arr = np.array(img).astype(float)

gray = np.dot(arr, [0.2989, 0.5870, 0.1140]) 

# Source Grayscale values:
# Text: ~25
# Green BG: ~170
# Light Green Box: ~215

# Target colors:
# Text -> Green [182, 199, 68]
# Green BG -> Purple [35, 16, 41]
# Light Green Box -> Lighter Purple [55, 36, 61]

x = [0, 25, 170, 215, 255]
r_y = [182, 182, 35, 55, 55]
g_y = [199, 199, 16, 36, 36]
b_y = [68, 68, 41, 61, 61]

new_r = np.interp(gray, x, r_y)
new_g = np.interp(gray, x, g_y)
new_b = np.interp(gray, x, b_y)

new_arr = np.stack([new_r, new_g, new_b], axis=2).astype(np.uint8)
new_img = Image.fromarray(new_arr)
new_img.save(out_path)
print("Saved to", out_path)
