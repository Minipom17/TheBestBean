import os
import re

# Update _Layout.cshtml to remove the CSS variables inversion we added
with open("Pages/Shared/_Layout.cshtml", "r", encoding="utf-8") as f:
    layout = f.read()

# Remove the inverted variables in html.dark
layout = re.sub(r'html\.dark \{.*?\}', '', layout, flags=re.DOTALL)
# Also change body tag to support dark background
layout = layout.replace('bg-v-white text-v-black', 'bg-v-white dark:bg-[#111111] text-v-black dark:text-[#F5F5F5]')

with open("Pages/Shared/_Layout.cshtml", "w", encoding="utf-8") as f:
    f.write(layout)

# Replace hardcoded colors with dark mode variants across all .cshtml
pages_dir = "Pages"
for root, _, files in os.walk(pages_dir):
    for file in files:
        if file.endswith(".cshtml"):
            file_path = os.path.join(root, file)
            with open(file_path, "r", encoding="utf-8") as f:
                content = f.read()
            
            # Text colors
            content = re.sub(r'(?<!dark:)text-v-black(?!/)', r'text-v-black dark:text-[#F5F5F5]', content)
            content = re.sub(r'(?<!dark:)text-black(?!/)', r'text-black dark:text-[#F5F5F5]', content)
            content = re.sub(r'(?<!dark:)text-slate-900(?!/)', r'text-slate-900 dark:text-[#F5F5F5]', content)
            
            # Background colors
            content = re.sub(r'(?<!dark:)bg-v-white(?!/)', r'bg-v-white dark:bg-[#111111]', content)
            content = re.sub(r'(?<!dark:)bg-white(?!/)', r'bg-white dark:bg-[#111111]', content)
            content = re.sub(r'(?<!dark:)bg-gray-100(?!/)', r'bg-gray-100 dark:bg-[#1A1A1A]', content)
            content = re.sub(r'(?<!dark:)bg-slate-100(?!/)', r'bg-slate-100 dark:bg-[#1A1A1A]', content)
            
            # Borders
            content = re.sub(r'(?<!dark:)border-v-black(?![/\w-])', r'border-v-black dark:border-[#F5F5F5]', content)
            content = re.sub(r'(?<!dark:)border-v-black/10', r'border-v-black/10 dark:border-[#F5F5F5]/10', content)
            content = re.sub(r'(?<!dark:)border-v-black/20', r'border-v-black/20 dark:border-[#F5F5F5]/20', content)
            
            # Remove duplicate classes if they somehow got added
            content = content.replace('dark:text-[#F5F5F5] dark:text-[#F5F5F5]', 'dark:text-[#F5F5F5]')
            content = content.replace('dark:bg-[#111111] dark:bg-[#111111]', 'dark:bg-[#111111]')
            content = content.replace('dark:bg-[#1A1A1A] dark:bg-[#1A1A1A]', 'dark:bg-[#1A1A1A]')

            with open(file_path, "w", encoding="utf-8") as f:
                f.write(content)

print("Applied explicitly styled dark mode to all views.")
