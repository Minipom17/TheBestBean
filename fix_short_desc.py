with open("Pages/Tour.cshtml.cs", "r", encoding="utf-8") as f:
    content = f.read()

import re
content = re.sub(r'\s*ShortDescription = "[^"]+",', '', content)

with open("Pages/Tour.cshtml.cs", "w", encoding="utf-8") as f:
    f.write(content)
print("Removed ShortDescription")
