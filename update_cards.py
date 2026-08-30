import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# We want to replace the title and tags div for lab-items
# Format:
# <div class="flex justify-between items-start mb-4">
#     <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">TITLE</h3>
#     <div class="flex gap-2 shrink-0">
#         <span class="...">MONTH DAY</span>
#         <span class="...">DURATION</span>
#     </div>
# </div>

def repl(match):
    h3_content = match.group(1)
    month_text = match.group(2).split(' ')[0] # just get AUG from AUG 15
    
    # Check what the title was
    if 'Sensory Basics' in h3_content:
        h3_content = 'Sensory Foundations'
    elif 'Brewing Architecture' in h3_content:
        h3_content = 'Extraction Science'
        
    # Determine level and color from the content before this div (the data-difficulty)
    # Actually, we can just hardcode or deduce it. 
    diff = "BEGINNER"
    color = "text-[#0057B7]"
    
    if "Cupping" in h3_content or "Flavor" in h3_content or "Extraction" in h3_content:
        diff = "INTERMEDIATE"
        color = "text-[#F05A28]"
    elif "SCA Cert" in h3_content:
        diff = "ADVANCED"
        color = "text-v-black"
        
    return f"""<div class="flex justify-between items-center mb-4">
                              <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">{h3_content}</h3>
                              <div class="flex items-center gap-2 shrink-0 font-mono-jb text-[10px] font-bold tracking-widest uppercase mt-1">
                                  <div class="w-1.5 h-1.5 bg-[#F05A28]"></div>
                                  <span class="text-gray-500">{month_text}</span>
                                  <span class="text-gray-300">/</span>
                                  <span class="{color}">{diff}</span>
                              </div>
                          </div>"""

# Regex to match the block
pattern = r'<div class="flex justify-between items-start mb-4">\s*<h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">(.*?)</h3>\s*<div class="flex gap-2 shrink-0">\s*<span class="[^"]*">(.*?)</span>\s*<span class="[^"]*">.*?</span>\s*</div>\s*</div>'

content = re.sub(pattern, repl, content, flags=re.DOTALL)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Experiences.cshtml")
