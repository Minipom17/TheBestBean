import os
import re

def fix_experiences_html(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # The pattern matches: <div class="w-1.5 h-1.5 bg-whatever"> followed by whitespace and a span or text
    # Let's just find <div class="w-1.5 h-1.5 bg-[something]"> and if it doesn't have </div> right after it, fix it.
    
    # We will use regex to find: <div class="w-1.5 h-1.5 bg-([^"]+)">\s*(<span|<div)
    # But wait, there's already a </div> closing it later on line 384!
    # So if we add </div> here, we have an extra </div> later which breaks layout.
    # Actually, in the file:
    # 381: <div class="w-1.5 h-1.5 bg-[#4F46E5]">
    # 382:   <span class="text-gray-300">/</span>
    # 383:   <span class="text-v-black dark:text-[#F5F5F7]">CUSCO</span>
    # 384: </div>
    # 385: <span class="text-gray-500">FEB</span>
    
    # We should replace that whole block to structure it correctly.
    # Let's just use regex to fix this specific structure block.
    
    pattern = re.compile(r'(<div class="w-1\.5 h-1\.5 bg-[^"]+">)\s*(<span class="text-gray-300">/</span>\s*<span[^>]+>[^<]+</span>)\s*</div>', re.MULTILINE)
    
    # Replace with: \1</div>\n\2
    content = pattern.sub(r'\1</div>\n\2', content)

    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

fix_experiences_html("Pages/Experiences.cshtml")

def apply_andean_earth_theme(directory):
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".cshtml"):
                file_path = os.path.join(root, file)
                with open(file_path, "r", encoding="utf-8") as f:
                    content = f.read()
                
                # Update dark mode colors to Andean Earth Palette
                content = content.replace("dark:bg-[#45553B]", "dark:bg-[#2F3A28]")
                content = content.replace("dark:text-[#F5F5F7]", "dark:text-[#FAF3DD]")
                
                # Catch any text-black or text-v-black or text-slate-900 that don't have dark:text
                # Regex to find class="...text-v-black..." without dark:text
                def add_dark_text(match):
                    cls_string = match.group(1)
                    if "dark:text" not in cls_string:
                        return f'class="{cls_string} dark:text-[#FAF3DD]"'
                    return match.group(0)
                
                # Match class="..." containing text-v-black or text-black or text-slate-900
                content = re.sub(r'class="([^"]*(?:text-v-black|text-black|text-slate-900)[^"]*)"', add_dark_text, content)
                
                with open(file_path, "w", encoding="utf-8") as f:
                    f.write(content)

apply_andean_earth_theme("Pages")
print("Fixed Experiences HTML and applied Andean Earth Theme.")
