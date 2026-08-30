import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    exp_content = f.read()

# 1. Advanced Cupping (ID 8) tag change to ADVANCED
def replace_cupping_tag(match):
    block = match.group(0)
    # Replace intermediate color/tag with advanced (black/ADVANCED)
    block = block.replace('bg-[#F05A28]', 'bg-black')
    block = block.replace('text-[#F05A28]">INTERMEDIATE', 'text-black">ADVANCED')
    return block

pattern_cupping = r'(<a asp-page="/Tour" asp-route-id="8".*?<h3[^>]*>Advanced Cupping</h3>.*?<div[^>]*>.*?</div>\s*</div>)'
exp_content = re.sub(pattern_cupping, replace_cupping_tag, exp_content, flags=re.DOTALL)

# 2. Reverse Roasting Beginner and Advanced images in Experiences.cshtml
# Roasting Beginner (ID 15) current: /images/20260129_111246.jpg
# Roasting Advanced (ID 16) current: /images/20260610_154127.jpg

# For ID 15
def replace_roasting_15(match):
    return match.group(1) + '/images/20260610_154127.jpg' + match.group(2)
pattern_15 = r'(<a asp-page="/Tour" asp-route-id="15".*?<img src=")[^"]+(")'
exp_content = re.sub(pattern_15, replace_roasting_15, exp_content, flags=re.DOTALL)

# For ID 16
def replace_roasting_16(match):
    return match.group(1) + '/images/20260129_111246.jpg' + match.group(2)
pattern_16 = r'(<a asp-page="/Tour" asp-route-id="16".*?<img src=")[^"]+(")'
exp_content = re.sub(pattern_16, replace_roasting_16, exp_content, flags=re.DOTALL)

# 3. Update Santa Teresa ID 12 to "Santa Teresa [Wilam's Farm]" and use new image
def replace_santa_12(match):
    block = match.group(0)
    block = re.sub(r'Innovation Processing', 'Santa Teresa [Wilam\'s Farm]', block)
    block = re.sub(r'<img src="[^"]+"', '<img src="/images/gringo_happy_with_farmer.jpg"', block)
    return block
pattern_12 = r'(<a asp-page="/Tour" asp-route-id="12".*?</article>)'
exp_content = re.sub(pattern_12, replace_santa_12, exp_content, flags=re.DOTALL)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(exp_content)
print("Updated Experiences.cshtml")

# 4. Now update Tour.cshtml.cs
with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    cs_content = f.read()

# Reverse ID 15 and 16 Images
def replace_cs_15(match):
    return match.group(1) + '/images/20260610_154127.jpg"'
pattern_cs_15 = r'(Id = 15,.*?ImageUrl = ")[^"]+"'
cs_content = re.sub(pattern_cs_15, replace_cs_15, cs_content, flags=re.DOTALL)

def replace_cs_16(match):
    return match.group(1) + '/images/20260129_111246.jpg"'
pattern_cs_16 = r'(Id = 16,.*?ImageUrl = ")[^"]+"'
cs_content = re.sub(pattern_cs_16, replace_cs_16, cs_content, flags=re.DOTALL)

# Update ID 12
def replace_cs_12(match):
    block = match.group(0)
    block = re.sub(r'Title = "[^"]+"', 'Title = "Santa Teresa [Wilam\'s Farm]"', block)
    block = re.sub(r'ImageUrl = "[^"]+"', 'ImageUrl = "/images/gringo_happy_with_farmer.jpg"', block)
    
    # Replace GalleryImages for ID 12
    gallery = """GalleryImages = new List<string>
                    {
                        "/images/20260730_140444(0).jpg",
                        "/images/20260730_150815.mp4",
                        "/images/20260730_150926.mp4",
                        "/images/20260730_162729.jpg",
                        "/images/20260730_162737.jpg",
                        "/images/20260730_162909.jpg",
                        "/images/20260730_162916.jpg",
                        "/images/gringo_happy_with_farmer.jpg"
                    }"""
    block = re.sub(r'GalleryImages = new List<string>\s*\{[^}]*\}', gallery, block, flags=re.DOTALL)
    return block

pattern_cs_12 = r'(new TourItem\s*\{\s*Id = 12,.*?\})'
# We have to be careful with regex for TourItem since it contains nested braces for lists.
# Let's just do a simpler search and replace for ID 12 section.
# Actually, the regex above will fail if it stops at the first `}`.
# I'll manually replace the strings in Tour.cshtml.cs for ID 12 since I know the structure.
