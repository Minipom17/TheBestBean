import re

with open("Pages/Shared/_Layout.cshtml", "r", encoding="utf-8") as f:
    layout = f.read()

# Remove the FIRST injected block (under tailwind.config script tag)
# The first script tag looks like:
# <script>
#         // Theme Toggle Logic
#         const themeToggle = document.getElementById('theme-toggle');
# ...
#         });
#         tailwind.config = {

pattern = r'// Theme Toggle Logic.*?\}\);\s*(?=tailwind\.config = \{)'
layout = re.sub(pattern, '', layout, flags=re.DOTALL)

with open("Pages/Shared/_Layout.cshtml", "w", encoding="utf-8") as f:
    f.write(layout)

print("Fixed duplicate script injection")
