import qrcode
from PIL import Image, ImageDraw
import random
import os

data = "https://purplebean.coffee"

def generate_qr(filename, accent_colors):
    qr = qrcode.QRCode(
        version=4, 
        error_correction=qrcode.constants.ERROR_CORRECT_H, 
        box_size=1,
        border=0,
    )
    qr.add_data(data)
    qr.make(fit=True)

    matrix = qr.modules
    n = len(matrix)

    box_size = 20
    padding = 4 * box_size
    width = n * box_size + padding * 2
    height = n * box_size + padding * 2

    bg_color = "#E4E9C8"
    main_color = "#271825"

    special_colored_cells = {}
    for col in range(n):
        run_start = -1
        for row in range(n):
            if matrix[row][col]:
                if run_start == -1:
                    run_start = row
            else:
                if run_start != -1:
                    run_length = row - run_start
                    if run_length >= 4:
                        num_to_color = random.choice([1, 2])
                        indices = random.sample(range(run_start, row), num_to_color)
                        for idx in indices:
                            special_colored_cells[(idx, col)] = random.choice(accent_colors)
                    run_start = -1
        if run_start != -1:
            run_length = n - run_start
            if run_length >= 4:
                num_to_color = random.choice([1, 2])
                indices = random.sample(range(run_start, n), num_to_color)
                for idx in indices:
                    special_colored_cells[(idx, col)] = random.choice(accent_colors)

    center_size = 9
    start_cutout = (n - center_size) // 2
    end_cutout = start_cutout + center_size

    img = Image.new("RGB", (width, height), bg_color)
    draw = ImageDraw.Draw(img)

    for row in range(n):
        for col in range(n):
            if start_cutout <= row < end_cutout and start_cutout <= col < end_cutout:
                continue
                
            if matrix[row][col]:
                x = padding + col * box_size
                y = padding + row * box_size
                color = special_colored_cells.get((row, col), main_color)
                draw.rounded_rectangle([x, y, x + box_size, y + box_size], radius=8, fill=color)

    logo_path = r"C:\Users\alext\Desktop\PBlogos\logo.png"
    try:
        logo = Image.open(logo_path).convert("RGBA")
        logo_size = center_size * box_size
        logo = logo.resize((logo_size, logo_size), Image.Resampling.LANCZOS)
        
        logo_x = padding + start_cutout * box_size
        logo_y = padding + start_cutout * box_size
        
        cx = logo_x + logo_size / 2
        cy = logo_y + logo_size / 2
        r = logo_size / 2
        draw.ellipse([cx - r, cy - r, cx + r, cy + r], fill=bg_color)
        
        img.paste(logo, (logo_x, logo_y), logo)
    except Exception as e:
        print("Could not load logo:", e)

    artifact_dir = r"C:\Users\alext\.gemini\antigravity-ide\brain\4b133b32-1b99-4e2e-95ba-233689eeed02"
    full_path = os.path.join(artifact_dir, filename)
    img.save(full_path)
    print(f"Saved {filename}")

# Generate 3 variations with different color schemes
generate_qr("qr_var1.png", ["#C54B8C", "#9966CC", "#FF3B1F", "#A1045A"])
generate_qr("qr_var2.png", ["#FF3B1F", "#FF6B6B", "#C54B8C"])
generate_qr("qr_var3.png", ["#9966CC", "#B57EDC", "#602080"])
