import urllib.request
import json
import random

url = 'https://raw.githubusercontent.com/jonathantneal/google-fonts-complete/master/google-fonts.json'
print("Downloading font list...")
data = urllib.request.urlopen(url).read()
fonts = list(json.loads(data).keys())

selected_fonts = fonts[:1000]

html = """<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Purple Bean - 1000 Unique Styles</title>
"""

# Load fonts individually so one failure doesn't break a batch of 50
for font in selected_fonts:
    font_param = font.replace(" ", "+")
    html += f'    <link href="https://fonts.googleapis.com/css2?family={font_param}&display=swap" rel="stylesheet" onerror="this.onerror=null;this.href=\'\';">\n'

html += """    <style>
        body {
            font-family: sans-serif;
            background-color: #FFFDF0;
            margin: 0;
            padding: 40px 20px;
            color: #333;
        }
        h1 { text-align: center; font-family: 'Outfit', sans-serif; font-size: 3rem; margin-bottom: 10px; }
        p.subtitle { text-align: center; font-size: 1.2rem; color: #666; margin-bottom: 50px; }
        
        .grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
            gap: 30px;
            max-width: 1400px;
            margin: 0 auto;
        }
        
        .card {
            background: white;
            border-radius: 16px;
            padding: 30px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.05);
            display: flex;
            flex-direction: column;
            align-items: center;
            border: 1px solid #eee;
        }

        .logo-container {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 30px;
            margin-bottom: 30px;
            width: 100%;
            height: 100px;
        }

        .icon-mask {
            width: 70px;
            height: 70px;
            object-fit: contain;
            flex-shrink: 0;
            border-radius: 50%;
        }

        .logo-text {
            font-size: 3.5rem;
            margin-top: -15px;
            line-height: normal;
            display: flex;
            align-items: center;
            height: 100%;
        }
        
        .details {
            width: 100%;
            border-top: 1px solid #eee;
            padding-top: 20px;
            display: flex;
            justify-content: space-between;
        }

        .detail-item {
            display: flex;
            flex-direction: column;
        }
        
        .detail-item span.label { font-size: 0.8rem; color: #888; text-transform: lowercase; letter-spacing: 1px; }
        .detail-item span.value { font-size: 1.1rem; font-weight: bold; }
    </style>
</head>
<body>

    <h1>1000 Unique Purple Bean Styles</h1>
    <p class="subtitle">Exploring 1000 different typography styles.</p>

    <div class="grid">
"""

def get_random_purple():
    r = random.randint(30, 200)
    g = random.randint(0, 100)
    b = random.randint(100, 255)
    return f"#{r:02x}{g:02x}{b:02x}"

for font in selected_fonts:
    color = get_random_purple()
    font_family_attr = f"'{font}', sans-serif" if " " in font else font
    
    html += f"""
        <div class="card">
            <div class="logo-container">
                <img src="logo.png" class="icon-mask" alt="logo" />
                <div class="logo-text" style="font-family: {font_family_attr}; color: {color}; text-transform: lowercase;">
                    purplebean
                </div>
            </div>
            
            <div class="details">
                <div class="detail-item">
                    <span class="label">Color</span>
                    <span class="value" style="color: {color};">{color}</span>
                </div>
                <div class="detail-item" style="text-align: right;">
                    <span class="label">Typography</span>
                    <span class="value" style="font-family: {font_family_attr};">{font}</span>
                </div>
            </div>
        </div>
"""

html += """
    </div>
</body>
</html>
"""

with open('brandbook_50_styles.html', 'w', encoding='utf-8') as f:
    f.write(html)

print("Generated brandbook_50_styles.html with 1000 fonts.")
