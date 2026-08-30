import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# Update JS active state removal
content = content.replace(
    "b.classList.remove('active', 'bg-v-black', 'text-v-white');\n                        b.classList.add('text-v-black', 'hover:bg-v-light-gray');",
    "b.classList.remove('active', 'border-b', 'border-v-black', 'pb-1', 'text-v-black');\n                        b.classList.add('text-v-gray', 'hover:text-v-black');"
)

# Update JS active state addition
content = content.replace(
    "btn.classList.add('active', 'bg-v-black', 'text-v-white');\n                    btn.classList.remove('text-v-black', 'hover:bg-v-light-gray');",
    "btn.classList.add('active', 'border-b', 'border-v-black', 'pb-1', 'text-v-black');\n                    btn.classList.remove('text-v-gray', 'hover:text-v-black');"
)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)

print("Done")
