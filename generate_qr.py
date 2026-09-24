import qrcode
from PIL import Image, ImageDraw, ImageFont
import os

url = "https://drive.google.com/file/d/1DKFbMg3VKV0Ec3bFhLVqG5DtN4vcrove/view?usp=drive_link"
logo_path = r"C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789933721848.png"
bg_path = r"C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\sunflower_border_1789933873457.jpg"
out_path = r"C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\rosatel_qr.png"

# 1. Generate QR Code
qr = qrcode.QRCode(
    version=5,
    error_correction=qrcode.constants.ERROR_CORRECT_H,
    box_size=20,
    border=2,
)
qr.add_data(url)
qr.make(fit=True)

qr_img = qr.make_image(fill_color="#cca010", back_color="white").convert('RGBA')

# 2. Add Logo to QR Code
try:
    logo = Image.open(logo_path).convert("RGBA")
    qr_w, qr_h = qr_img.size
    logo_size = int(qr_w * 0.25)
    logo = logo.resize((logo_size, int(logo_size * logo.size[1] / logo.size[0])), Image.LANCZOS)
    
    circle_size = int(qr_w * 0.35)
    circle_bg = Image.new('RGBA', (circle_size, circle_size), (255, 255, 255, 0))
    draw = ImageDraw.Draw(circle_bg)
    draw.ellipse((0, 0, circle_size, circle_size), fill=(255, 255, 255, 255))
    
    logo_pos = ((circle_size - logo.size[0]) // 2, (circle_size - logo.size[1]) // 2)
    circle_bg.paste(logo, logo_pos, logo)
    
    pos = ((qr_w - circle_size) // 2, (qr_h - circle_size) // 2)
    qr_img.paste(circle_bg, pos, circle_bg)
except Exception as e:
    print("Could not load logo:", e)

# 3. Paste onto Background
try:
    bg = Image.open(bg_path).convert("RGBA")
    bg_w, bg_h = bg.size
    
    target_qr_size = int(bg_w * 0.45)
    qr_img = qr_img.resize((target_qr_size, target_qr_size), Image.LANCZOS)
    
    pos = ((bg_w - target_qr_size) // 2, (bg_h - target_qr_size) // 2)
    bg.paste(qr_img, pos, qr_img)
    
    # 4. Add Text
    draw = ImageDraw.Draw(bg)
    
    try:
        font_large = ImageFont.truetype("arialbd.ttf", int(bg_w * 0.035))
    except:
        font_large = ImageFont.load_default()
        
    text_top = "ESCANEA PARA VER EL CATALOGO\nCOMPLETO DE FLORES AMARILLAS"
    text_bottom = "21 DE SEPTIEMBRE: FLORES AMARILLAS"
    
    def draw_text_with_outline(text, position, font, text_color, outline_color):
        x, y = position
        for adj_x, adj_y in [(-3, -3), (3, -3), (-3, 3), (3, 3)]:
            draw.multiline_text((x + adj_x, y + adj_y), text, font=font, fill=outline_color, align="center")
        draw.multiline_text((x, y), text, font=font, fill=text_color, align="center")
        
    bbox_top = draw.multiline_textbbox((0,0), text_top, font=font_large, align="center")
    w_top = bbox_top[2] - bbox_top[0]
    
    bbox_bottom = draw.multiline_textbbox((0,0), text_bottom, font=font_large, align="center")
    w_bottom = bbox_bottom[2] - bbox_bottom[0]
    
    draw_text_with_outline(text_top, ((bg_w - w_top) // 2, int(bg_h * 0.05)), font_large, (0, 0, 0), (255, 255, 255))
    draw_text_with_outline(text_bottom, ((bg_w - w_bottom) // 2, int(bg_h * 0.90)), font_large, (0, 0, 0), (255, 255, 255))
    
    bg.convert('RGB').save(out_path, quality=95)
    print("QR Code generated at:", out_path)
except Exception as e:
    print("Error processing background:", e)
