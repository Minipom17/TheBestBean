import cv2
import numpy as np
import os

img_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\.user_uploaded\media_1789948688899.png'
out_path = r'C:\Users\alext\.gemini\antigravity-ide\brain\13b351a0-c41a-4eb6-9d41-6652032dbf3e\writable_card.png'

img = cv2.imread(img_path)

# Original colors (BGR)
bg_color = np.array([44, 19, 38])
box_color = np.array([61, 36, 55])
text_color = np.array([68, 199, 182])

# Create an output image initially same as background
out = np.full_like(img, bg_color)

# Copy text, make it white
# We can use distance to text_color
dist_text = np.linalg.norm(img.astype(float) - text_color, axis=2)
text_mask = dist_text < 60
out[text_mask] = [255, 255, 255]

# Make the box white
dist_box = np.linalg.norm(img.astype(float) - box_color, axis=2)
box_mask = dist_box < 30
out[box_mask] = [255, 255, 255]

# Find horizontal lines to turn into white writing spaces
# We look for lines in the original text_mask
kernel = np.ones((1, 30), np.uint8)
horizontal_lines = cv2.morphologyEx(text_mask.astype(np.uint8)*255, cv2.MORPH_OPEN, kernel)

contours, _ = cv2.findContours(horizontal_lines, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Draw white rectangles above the lines
for c in contours:
    x, y, w, h = cv2.boundingRect(c)
    if w > 50:  # It's a significant line
        # Draw a white rectangle from just above the line down to the line
        # The height of handwriting space might be around 40 pixels
        cv2.rectangle(out, (x, y - 35), (x + w, y + h), (255, 255, 255), -1)

cv2.imwrite(out_path, out)
print("Saved to", out_path)
