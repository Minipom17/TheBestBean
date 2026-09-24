import numpy as np
from PIL import Image
import os

img_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789948688899.png'
base_dir = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e'

img = Image.open(img_path).convert('RGB')
arr = np.array(img).astype(float)
gray = np.dot(arr, [0.2989, 0.5870, 0.1140]) 

palettes = [
    {
        "name": "card_blue_gold.png",
        "bg": [10, 20, 45],
        "box": [25, 40, 70],
        "text": [255, 215, 0]
    },
    {
        "name": "card_charcoal_coral.png",
        "bg": [30, 30, 30],
        "box": [50, 50, 50],
        "text": [255, 127, 80]
    },
    {
        "name": "card_forest_cream.png",
        "bg": [15, 40, 25],
        "box": [30, 65, 45],
        "text": [245, 245, 220]
    },
    {
        "name": "card_coffee_latte.png",
        "bg": [50, 30, 20],
        "box": [75, 45, 30],
        "text": [240, 220, 190]
    },
    {
        "name": "card_terracotta_mint.png",
        "bg": [120, 50, 40],
        "box": [140, 70, 60],
        "text": [210, 255, 230]
    }
]

for p in palettes:
    x = [0, 25, 45, 178, 255]
    
    r_y = [p['bg'][0], p['bg'][0], p['box'][0], p['text'][0], p['text'][0]]
    g_y = [p['bg'][1], p['bg'][1], p['box'][1], p['text'][1], p['text'][1]]
    b_y = [p['bg'][2], p['bg'][2], p['box'][2], p['text'][2], p['text'][2]]
    
    new_r = np.interp(gray, x, r_y)
    new_g = np.interp(gray, x, g_y)
    new_b = np.interp(gray, x, b_y)
    
    new_arr = np.stack([new_r, new_g, new_b], axis=2).astype(np.uint8)
    new_img = Image.fromarray(new_arr)
    out_path = os.path.join(base_dir, p['name'])
    new_img.save(out_path)
    print("Generated:", out_path)
