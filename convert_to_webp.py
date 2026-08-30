import os
import subprocess
import re

images_dir = "wwwroot/images"
image_files = []

for root, _, files in os.walk(images_dir):
    for file in files:
        if file.lower().endswith(('.jpg', '.jpeg', '.png')):
            image_files.append(os.path.join(root, file))

# Convert to webp
for file in image_files:
    webp_file = os.path.splitext(file)[0] + '.webp'
    # Run ffmpeg
    subprocess.run(['ffmpeg', '-y', '-i', file, '-c:v', 'libwebp', '-q:v', '80', webp_file], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    # Delete original
    os.remove(file)
    print(f"Converted {file} to {webp_file}")

# Update references in code
pages_dir = "Pages"
for root, _, files in os.walk(pages_dir):
    for file in files:
        if file.lower().endswith(('.cshtml', '.cs', '.js')):
            file_path = os.path.join(root, file)
            with open(file_path, "r", encoding="utf-8") as f:
                content = f.read()
            
            # Case insensitive replace .jpg, .jpeg, .png to .webp
            content = re.sub(r'\.jpg|\.jpeg|\.png', '.webp', content, flags=re.IGNORECASE)
            
            with open(file_path, "w", encoding="utf-8") as f:
                f.write(content)

# Update Program.cs
with open("Program.cs", "r", encoding="utf-8") as f:
    content = f.read()
content = re.sub(r'\.jpg|\.jpeg|\.png', '.webp', content, flags=re.IGNORECASE)
with open("Program.cs", "w", encoding="utf-8") as f:
    f.write(content)

print("Finished conversion and code updates.")
