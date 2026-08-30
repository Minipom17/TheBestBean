import re

# 1. Update GreenBeans.cshtml
with open("Pages/GreenBeans.cshtml", "r", encoding="utf-8") as f:
    gb_content = f.read()

# The header block to remove:
# <div class="mb-16 border-b border-v-black pb-8">
#     <span class="text-[10px] font-mono-jb uppercase tracking-widest text-v-gray block font-bold mb-3">12° SUR // ARCHIVE REF: 001</span>
#     <h1 class="text-5xl md:text-8xl font-sans font-black tracking-tighter mb-4 uppercase">
#         ALL COFFEES
#     </h1>
#     <p class="font-mono-jb text-v-black font-bold uppercase tracking-widest text-xs">
#         GET IT GREEN OR ROASTED — CUSCO, PERU
#     </p>
# </div>
pattern_header = r'<div class="mb-16 border-b border-v-black pb-8">[\s\S]*?</div>'
gb_content = re.sub(pattern_header, '', gb_content, count=1)

with open("Pages/GreenBeans.cshtml", "w", encoding="utf-8") as f:
    f.write(gb_content)
print("Updated GreenBeans.cshtml")

# 2. Update Experiences.cshtml to include location tags
with open("Pages/Experiences.cshtml", "r", encoding="utf-8") as f:
    exp_content = f.read()

# We will split by `<article ` and process each chunk
chunks = exp_content.split('<article ')
for i in range(1, len(chunks)):
    chunk = chunks[i]
    
    # Extract location
    loc_match = re.search(r'data-location="([^"]+)"', chunk)
    if loc_match:
        loc_val = loc_match.group(1).upper()
        # For 'special', let's just make it 'SPECIAL EVENT' or keep it 'SPECIAL'
        
        # Now find the tag container
        tag_pattern = r'(<div class="flex items-center gap-2 font-mono-jb text-\[10px\] font-bold tracking-widest uppercase mb-2">)(.*?)(</div>)'
        
        def insert_loc(m):
            prefix = m.group(1)
            internals = m.group(2)
            suffix = m.group(3)
            
            # Prevent double adding
            if f'<span class="text-v-black">{loc_val}</span>' not in internals:
                internals += f'\n                                  <span class="text-gray-300">/</span>\n                                  <span class="text-v-black">{loc_val}</span>\n                              '
            return prefix + internals + suffix
        
        chunks[i] = re.sub(tag_pattern, insert_loc, chunk, count=1, flags=re.DOTALL)

exp_content = '<article '.join(chunks)

with open("Pages/Experiences.cshtml", "w", encoding="utf-8") as f:
    f.write(exp_content)
print("Updated Experiences.cshtml")
