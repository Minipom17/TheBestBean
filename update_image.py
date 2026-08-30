import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# 1. Update the Santa Teresa Sky Farm image (ID 11)
# Search for ID 11 and its image
pattern = r'(<a asp-page="/Tour" asp-route-id="11" class="text-decoration-none text-slate-900 flex flex-col h-full group">\s*<div class="[^"]*">\s*<img src=")[^"]+(" alt="Santa Teresa Sky Farm")'
content = re.sub(pattern, r'\1/images/hilda_drying.jpg\2', content)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Experiences.cshtml image")
