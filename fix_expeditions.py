import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# 1. Update the main origin expeditions photo (ID 4 - Cloud Forest)
pattern_img = r'(<a asp-page="/Tour" asp-route-id="4" class="[^"]*">\s*<!-- Image Block spans full 8 cols -->\s*<div class="[^"]*">\s*<img src=")[^"]+(" alt="Cloud Forest" class="[^"]*" />)'
content = re.sub(pattern_img, r'\1/images/20260720_082721.jpg\2', content)

# 2. Remove the duplicated Brewing card (ID 3) from Urban Labs
# It starts with <!-- Lab Item 1 --> and goes to just before <!-- NEW: In-Depth Cupping Session --> (which doesn't exist anymore maybe?)
# Actually let's just find the article with asp-route-id="3" and remove it.
# We'll match <!-- Lab Item 1 --> to </article>
pattern_lab1 = r'<!-- Lab Item 1 -->\s*<article class="lab-item[^>]*>.*?<a asp-page="/Tour" asp-route-id="3".*?</article>'
content = re.sub(pattern_lab1, '', content, flags=re.DOTALL)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Experiences.cshtml")

# 3. Update Tour 4 in C# code
with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    cs_content = f.read()

def replace_tour_4(match):
    block = match.group(0)
    return re.sub(r'ImageUrl = "[^"]+"', 'ImageUrl = "/images/20260720_082721.jpg"', block)

pattern_cs = r'new TourItem\s*\{\s*Id = 4,.*?RequiredGear = new List<string> \{.*?\}\s*\}'
cs_content = re.sub(pattern_cs, replace_tour_4, cs_content, flags=re.DOTALL)

with open('Pages/Tour.cshtml.cs', 'w', encoding='utf-8') as f:
    f.write(cs_content)
print("Updated Tour.cshtml.cs")
