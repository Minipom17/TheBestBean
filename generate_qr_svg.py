import qrcode
import random

# Data for QR code
data = "https://purplebean.coffee"

# Create QR code instance
qr = qrcode.QRCode(
    version=4, # Increase version to ensure enough size for a center cutout
    error_correction=qrcode.constants.ERROR_CORRECT_H, # High error correction for center logo
    box_size=20,
    border=4,
)
qr.add_data(data)
qr.make(fit=True)

matrix = qr.modules
n = len(matrix)

# Colors
bg_color = "#E4E9C8" # Light beige
main_color = "#271825" # Dark purple/black
accent_colors = ["#C54B8C", "#9966CC", "#FF3B1F", "#A1045A"] # Pinks, purples, reds

# Find vertical runs of 4+ to apply accents
special_colored_cells = {} # (row, col): color
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
                    # Color 1 or 2 of these
                    num_to_color = random.choice([1, 2])
                    indices_to_color = random.sample(range(run_start, row), num_to_color)
                    for idx in indices_to_color:
                        special_colored_cells[(idx, col)] = random.choice(accent_colors)
                run_start = -1
    
    # Check at end of column
    if run_start != -1:
        run_length = n - run_start
        if run_length >= 4:
            num_to_color = random.choice([1, 2])
            indices_to_color = random.sample(range(run_start, n), num_to_color)
            for idx in indices_to_color:
                special_colored_cells[(idx, col)] = random.choice(accent_colors)

# Center cutout size
center_size = 9
start_cutout = (n - center_size) // 2
end_cutout = start_cutout + center_size

box_size = 20
width = n * box_size
height = n * box_size

# SVG Generation
svg = f'<svg xmlns="http://www.w3.org/2000/svg" width="{width}" height="{height}" viewBox="0 0 {width} {height}">\n'
svg += f'  <rect width="{width}" height="{height}" fill="{bg_color}" />\n'

# Draw modules
for row in range(n):
    for col in range(n):
        # Skip center area for logo
        if start_cutout <= row < end_cutout and start_cutout <= col < end_cutout:
            continue
            
        if matrix[row][col]:
            x = col * box_size
            y = row * box_size
            color = special_colored_cells.get((row, col), main_color)
            # Use rounded rectangles for that droplet/blob aesthetic
            svg += f'  <rect x="{x}" y="{y}" width="{box_size}" height="{box_size}" rx="8" ry="8" fill="{color}" />\n'

# Overlay Logo
# Since it's local, we can link directly to the absolute path provided by the user
logo_path = r"C:\Users\alext\Desktop\PBlogos\logo.png"
logo_x = start_cutout * box_size
logo_y = start_cutout * box_size
logo_size = center_size * box_size

# Draw a circle backing for the logo just in case
cx = logo_x + logo_size / 2
cy = logo_y + logo_size / 2
radius = logo_size / 2
svg += f'  <circle cx="{cx}" cy="{cy}" r="{radius}" fill="{bg_color}" />\n'

# Convert backslashes for SVG href
logo_href = logo_path.replace("\\", "/")
svg += f'  <image href="file:///{logo_href}" x="{logo_x}" y="{logo_y}" width="{logo_size}" height="{logo_size}" />\n'

svg += '</svg>\n'

with open("custom_qr_code.svg", "w", encoding="utf-8") as f:
    f.write(svg)

print("Generated custom_qr_code.svg successfully!")
