import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# Update Sensory Foundations (ID 7)
pattern7 = r'(<a asp-page="/Tour" asp-route-id="7" class="[^"]*">\s*<div class="[^"]*">\s*<img src=")[^"]+(" alt="[^"]*" class="[^"]*" />)'
content = re.sub(pattern7, r'\1/images/SCAA_FlavorWheel.01.18.15.jpg\2', content)

# Update Brewing (ID 6)
pattern6 = r'(<a asp-page="/Tour" asp-route-id="6" class="[^"]*">\s*<div class="[^"]*">\s*<img src=")[^"]+(" alt="[^"]*" class="[^"]*" />)'
content = re.sub(pattern6, r'\1/images/brewer_withGrinds.jpg\2', content)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Experiences.cshtml images")
