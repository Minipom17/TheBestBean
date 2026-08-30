import re

file_path = "Pages/Experiences.cshtml"
with open(file_path, "r", encoding="utf-8") as f:
    content = f.read()

# I need to find all occurrences of the broken HTML structure
# Specifically:
# <div class="w-1.5 h-1.5 bg-[COLOR]">
# <span class="text-gray-500">... or <span class="text-gray-300">/</span>... or <span class="text-[COLOR]">

# Let's fix the location tags first if any are broken like:
# <div class="w-1.5 h-1.5 bg-[COLOR]">
#   <span class="text-gray-300">/</span>
#   <span class="text-v-black">LOCATION</span>
# </div>

# We want them to be properly formatted:
# <div class="w-1.5 h-1.5 bg-[COLOR]"></div>
# <span class="text-gray-500">MONTH</span>
# <span class="text-gray-300">/</span>
# <span class="text-COLOR">DIFFICULTY</span>

# Or for locations:
# <span class="text-v-black mr-2">LOCATION</span>
# <div class="w-1.5 h-1.5 bg-[COLOR]"></div>

# Let's just use regex to replace all:
# `<div class="w-1.5 h-1.5 bg-[.*?]"(\s*)>`
# where it is not followed by `</div>`
# Wait, I'll just replace `<div class="w-1.5 h-1.5 bg-[#F05A28]">` with `<div class="w-1.5 h-1.5 bg-[#F05A28]"></div>`
# But if it's already closed?
# Let's just run a replace on `">\n` for these specific divs.

content = re.sub(r'(<div class="w-1\.5 h-1\.5 bg-[^>]*?">)\s*(?=<span)', r'\1</div>\n', content)

# I should also check if the locations added previously were broken
# Let's look at the file content in Python
with open("Experiences_fixed.cshtml", "w", encoding="utf-8") as f:
    f.write(content)

print("Fixed unclosed divs")
