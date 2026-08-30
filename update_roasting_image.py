import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# Replace the image for Roasting lab (ID 14)
pattern = r'(<a asp-page="/Tour" asp-route-id="14" class="[^"]*">\s*<div class="[^"]*">\s*<img src=")[^"]+(" alt="Roasting Lab" class="[^"]*" />)'
content = re.sub(pattern, r'\1/images/20260422_141746.jpg\2', content)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Experiences.cshtml image for Roasting")
