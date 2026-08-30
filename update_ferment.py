import re

# 1. Update Experiences.cshtml
with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    exp_content = f.read()

# Update Fermentation Camp image
pattern_img5 = r'(<a asp-page="/Tour" asp-route-id="5".*?<img src=")[^"]+(" alt="Fermentation Tanks")'
exp_content = re.sub(pattern_img5, r'\1/images/ferment.jpg\2', exp_content, flags=re.DOTALL)

# Update Cloud Forest Immersion title
exp_content = exp_content.replace('Cloud Forest Immersion', 'Franz\'s Farm [1600m]')

# Update Santa Teresa title
exp_content = exp_content.replace('Santa Teresa Sky Farm', 'Santa Teresa Sky Farm [1800m]')

# Write back
with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(exp_content)
print("Updated Experiences.cshtml")

# 2. Update Tour.cshtml.cs
with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    cs_content = f.read()

# Update Fermentation Camp image
def replace_tour_5_img(match):
    block = match.group(0)
    return re.sub(r'ImageUrl = "[^"]+"', 'ImageUrl = "/images/ferment.jpg"', block)
pattern_cs5 = r'new TourItem\s*\{\s*Id = 5,.*?\}'
cs_content = re.sub(pattern_cs5, replace_tour_5_img, cs_content, flags=re.DOTALL)

# Update titles
cs_content = cs_content.replace('Cloud Forest Immersion', 'Franz\'s Farm [1600m]')
cs_content = cs_content.replace('Santa Teresa Sky Farm', 'Santa Teresa Sky Farm [1800m]')

# Write back
with open('Pages/Tour.cshtml.cs', 'w', encoding='utf-8') as f:
    f.write(cs_content)
print("Updated Tour.cshtml.cs")
