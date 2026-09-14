import os
from PIL import Image

paths = [
    'wwwroot/Media/experiences/tour-22/cynthia-hero.jpg', 
    'wwwroot/Media/experiences/tour-25/cynthia-brew-01.webp', 
    'wwwroot/Media/cusco_lab_1.jpg'
]

for p in paths:
    try:
        with Image.open(p) as img:
            print(f'{p}: {img.size}')
    except Exception as e:
        print(f'{p}: {e}')
