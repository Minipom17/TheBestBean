import re

with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

def replace_tour_7(match):
    block = match.group(0)
    return re.sub(r'ImageUrl = "[^"]+"', 'ImageUrl = "/images/SCAA_FlavorWheel.01.18.15.jpg"', block)

def replace_tour_6(match):
    block = match.group(0)
    block = re.sub(r'ImageUrl = "[^"]+"', 'ImageUrl = "/images/brewer_withGrinds.jpg"', block)
    # Add learning_Table_arial.jpg to GalleryImages if not present
    if '"/images/learning_Table_arial.jpg"' not in block:
        block = re.sub(r'GalleryImages = new List<string>\s*\{\s*', 'GalleryImages = new List<string>\n                    {\n                        "/images/learning_Table_arial.jpg",\n                        ', block)
    return block

pattern7 = r'new TourItem\s*\{\s*Id = 7,.*?RequiredGear = new List<string> \{.*?\}\s*\}'
content = re.sub(pattern7, replace_tour_7, content, flags=re.DOTALL)

pattern6 = r'new TourItem\s*\{\s*Id = 6,.*?RequiredGear = new List<string> \{.*?\}\s*\}'
content = re.sub(pattern6, replace_tour_6, content, flags=re.DOTALL)

with open('Pages/Tour.cshtml.cs', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Tour.cshtml.cs")
