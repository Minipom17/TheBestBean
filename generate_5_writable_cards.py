import numpy as np
from PIL import Image, ImageDraw
import os

img_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789948688899.png'
base_dir = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e'

img = Image.open(img_path).convert('RGB')
arr = np.array(img).astype(float)
gray = np.dot(arr, [0.2989, 0.5870, 0.1140]) 

palettes = [
    {
        "name": "writable_purple.png",
        "bg": [38, 19, 44],
    },
    {
        "name": "writable_blue.png",
        "bg": [10, 20, 45],
    },
    {
        "name": "writable_green.png",
        "bg": [15, 40, 25],
    },
    {
        "name": "writable_brown.png",
        "bg": [50, 30, 20],
    },
    {
        "name": "writable_charcoal.png",
        "bg": [30, 30, 30],
    }
]

# Writing boxes (explicit white shading beside text)
boxes = [
    [65, 316, 500, 520],    # Main Box
    [405, 48, 545, 78],     # SCA
    [405, 103, 545, 133],    # MASL
    [175, 595, 515, 625],   # Region
    [145, 675, 515, 705],   # Farm
    [210, 755, 515, 785],   # Producer
    [200, 835, 515, 865],   # Varietal
    [190, 915, 515, 945]    # Process
]

for p in palettes:
    x = [0, 25, 45, 178, 255]
    r_y = [p['bg'][0], p['bg'][0], 255, 255, 255]
    g_y = [p['bg'][1], p['bg'][1], 255, 255, 255]
    b_y = [p['bg'][2], p['bg'][2], 255, 255, 255]
    
    new_r = np.interp(gray, x, r_y)
    new_g = np.interp(gray, x, g_y)
    new_b = np.interp(gray, x, b_y)
    
    new_arr = np.stack([new_r, new_g, new_b], axis=2).astype(np.uint8)
    new_img = Image.fromarray(new_arr)
    
    draw = ImageDraw.Draw(new_img)
    for b in boxes:
        draw.rectangle(b, fill=(255, 255, 255))
        
    out_path = os.path.join(base_dir, p['name'])
    new_img.save(out_path)
    print("Generated:", out_path)
