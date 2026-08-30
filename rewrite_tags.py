import re

file_path = "Pages/Experiences.cshtml"
with open(file_path, "r", encoding="utf-8") as f:
    content = f.read()

def replace_block(match):
    full_block = match.group(0)
    
    # Extract month
    month_match = re.search(r'<span class="text-gray-500">([A-Z]{3})</span>', full_block)
    month = month_match.group(1) if month_match else "TBD"
    
    # Extract difficulty
    diff_match = re.search(r'<span class="text-\[#?.*?\]">(BEGINNER|INTERMEDIATE|ADVANCED)</span>', full_block, re.IGNORECASE)
    if not diff_match:
        diff_match = re.search(r'<span class="text-black">(BEGINNER|INTERMEDIATE|ADVANCED)</span>', full_block, re.IGNORECASE)
    if not diff_match:
        diff_match = re.search(r'<span class="text-v-black">(BEGINNER|INTERMEDIATE|ADVANCED)</span>', full_block, re.IGNORECASE)
        
    difficulty = diff_match.group(1).upper() if diff_match else "LEVEL"
    
    # Define colors
    color = "#F05A28" # Default orange
    if difficulty == "BEGINNER":
        color = "#4F46E5" # Indigo
    elif difficulty == "INTERMEDIATE":
        color = "#F05A28" # Orange
    elif difficulty == "ADVANCED":
        color = "#000000" # Black

    # Extract location (try from data-location in the parent article, but we don't have it in this regex)
    # So we'll try to extract location from the text if it exists (e.g. <span class="text-v-black">LIMA</span>)
    loc_match = re.search(r'<span class="text-v-black">(CUSCO|LIMA|CALCA|SPECIAL)</span>', full_block, re.IGNORECASE)
    if loc_match:
        location = loc_match.group(1).upper()
    else:
        location = "" # Some items like "The Coffee Project" don't have this block, wait, they have a different block.

    # Build new block
    loc_html = f'<span class="text-v-black">{location}</span>\n<span class="text-gray-300">/</span>\n' if location else ''
    
    new_html = f"""<div class="flex items-center gap-2 font-mono-jb text-[10px] font-bold tracking-widest uppercase mb-2">
    {loc_html}<div class="w-1.5 h-1.5" style="background-color: {color}"></div>
    <span class="text-gray-500">{month}</span>
    <span class="text-gray-300">/</span>
    <span style="color: {color}">{difficulty}</span>
</div>"""
    return new_html

# The regex should match the entire <div class="flex items-center gap-2... mb-2">...</div>
# Wait, some have unclosed divs and span multiple lines!
# I'll just use a non-greedy match that captures everything between <div class="flex items-center gap-2 font-mono-jb text-[10px] font-bold tracking-widest uppercase mb-2"> and the <h3
# because the h3 immediately follows this block!
pattern = r'<div class="flex items-center gap-2 font-mono-jb text-\[10px\] font-bold tracking-widest uppercase mb-2">[\s\S]*?(?=<h3)'

content = re.sub(pattern, lambda m: replace_block(m) + '\n', content)

# I also need to fix the Advanced Cupping one which has mt-1 instead of mb-2 and is placed AFTER the h3!
# Let's fix that specific one first if it exists.
pattern_adv = r'<div class="flex items-center gap-2 shrink-0 font-mono-jb text-\[10px\] font-bold tracking-widest uppercase mt-1">[\s\S]*?</div>\s*</div>'
# Actually, I'll just restore from source control and run the script, wait, I can just write my own manual fix for that one if needed.

with open("Experiences.cshtml", "w", encoding="utf-8") as f:
    f.write(content)

print("Rewrote tag blocks")
