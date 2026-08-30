import re

with open("Pages/Experiences.cshtml", "r", encoding="utf-8") as f:
    content = f.read()

# We need to find this pattern for all lab-item cards (Urban Labs).
# <div class="flex justify-between items-center mb-4">
#     <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">Title</h3>
#     <div class="flex items-center gap-2 shrink-0 font-mono-jb text-[10px] font-bold tracking-widest uppercase mt-1">
#         ... (tag internals)
#     </div>
# </div>

# We will replace it with:
# <div class="flex flex-col mb-4">
#     <div class="flex items-center gap-2 font-mono-jb text-[10px] font-bold tracking-widest uppercase mb-2">
#         ... (tag internals)
#     </div>
#     <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">Title</h3>
# </div>

def replace_card_header(match):
    h3_tag = match.group(1)
    tag_internals = match.group(2)
    
    new_header = f"""<div class="flex flex-col mb-4">
                            <div class="flex items-center gap-2 font-mono-jb text-[10px] font-bold tracking-widest uppercase mb-2">
{tag_internals}
                            </div>
                            {h3_tag}
                        </div>"""
    return new_header

# Note: In the original, the h3 might have m-0, we'll keep it.
pattern = r'<div class="flex justify-between items-center mb-4">\s*(<h3[^>]*>.*?</h3>)\s*<div class="flex items-center gap-2 shrink-0 font-mono-jb text-\[10px\] font-bold tracking-widest uppercase mt-1">(.*?)</div>\s*</div>'
content = re.sub(pattern, replace_card_header, content, flags=re.DOTALL)

with open("Pages/Experiences.cshtml", "w", encoding="utf-8") as f:
    f.write(content)
print("Updated Experiences.cshtml tag layouts")
