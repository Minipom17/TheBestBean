import qrcode
from qrcode.image.styledpil import StyledPilImage
from qrcode.image.styles.moduledrawers.pil import RoundedModuleDrawer, SquareModuleDrawer
from qrcode.image.styles.colormasks import SolidFillColorMask
from PIL import Image, ImageDraw
import os

url = "https://drive.google.com/file/d/1DKFbMg3VKV0Ec3bFhLVqG5DtN4vcrove/view?usp=drive_link"
logo_path = r"C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789933721848.png"
base_dir = r"C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e"

def create_qr_variation(filename, fill_color, use_rounded=False):
    qr = qrcode.QRCode(
        version=5,
        error_correction=qrcode.constants.ERROR_CORRECT_H,
        box_size=20,
        border=2,
    )
    qr.add_data(url)
    qr.make(fit=True)

    drawer = RoundedModuleDrawer() if use_rounded else SquareModuleDrawer()
    color_mask = SolidFillColorMask(back_color=(255, 255, 255), front_color=fill_color)

    qr_img = qr.make_image(
        image_factory=StyledPilImage,
        module_drawer=drawer,
        color_mask=color_mask,
    ).convert('RGBA')

    # Add Logo
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

    out_path = os.path.join(base_dir, filename)
    qr_img.convert('RGB').save(out_path)
    print("Generated:", out_path)

create_qr_variation("qr_gold_square.png", (204, 160, 16), False)
create_qr_variation("qr_red_square.png", (230, 0, 0), False)
create_qr_variation("qr_black_square.png", (0, 0, 0), False)
create_qr_variation("qr_gold_rounded.png", (204, 160, 16), True)
create_qr_variation("qr_red_rounded.png", (230, 0, 0), True)
