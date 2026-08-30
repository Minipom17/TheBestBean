import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# Update the Brewing (ID 6) image
# Looking for: alt="Brewing Architecture" or whatever it was (ID 6 is Extraction Science/Brewing)
# Let's search by asp-route-id="6" to be safe.

pattern = r'(<a asp-page="/Tour" asp-route-id="6" class="[^"]*">\s*<div class="[^"]*">\s*<img src=")[^"]+(" alt="[^"]*" class="[^"]*" />)'
content = re.sub(pattern, r'\1/images/learning_Table_arial.jpg\2', content)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Experiences.cshtml image for Brewing")
