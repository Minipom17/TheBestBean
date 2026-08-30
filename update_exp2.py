import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# Replace the pills with simple colored text. 
# We'll replace the `<div class="flex gap-2 shrink-0">` block with a generic sleek text style.
# The original pills look like: 
# <span class="px-3 py-1 bg-slate-100 text-slate-800 rounded-full text-[10px] font-bold tracking-widest uppercase font-mono-jb">AUG 28</span>
# <span class="px-3 py-1 bg-slate-900 text-white rounded-full text-[10px] font-bold tracking-widest uppercase font-mono-jb">90 MIN</span>
# <span class="px-3 py-1 bg-red-50 text-red-800 rounded-full text-[10px] font-bold tracking-widest uppercase font-mono-jb shrink-0">60 Min</span>
# We'll just strip the `bg-... text-white px-3 py-1 rounded-full` classes from any span inside the Urban Labs grid (lines 75-263 roughly).
# Wait, it's easier to just do regex replacement for the specific spans.

def replace_pills(match):
    # Match the inner spans
    spans = match.group(0)
    spans = re.sub(r'bg-[a-z0-9\-]+', '', spans)
    spans = re.sub(r'text-[a-z0-9\-]+', '', spans)
    spans = re.sub(r'rounded-full', '', spans)
    spans = re.sub(r'px-\d+', '', spans)
    spans = re.sub(r'py-\d+', '', spans)
    # clean up multiple spaces
    spans = re.sub(r'\s+', ' ', spans)
    
    # Add our generic sleek style to the parent div
    # Wait, the parent div is matched. We should replace the `gap-2 shrink-0` with `gap-1 shrink-0 font-mono-jb text-[10px] font-bold tracking-widest uppercase items-center text-v-blue`
    # Let's just do it cleanly.
    return spans.replace('class=" ', 'class="')

content = re.sub(
    r'<span class="px-3 py-1 bg-[a-z0-9\-]+ text-[a-z0-9\-]+ rounded-full text-\[10px\] font-bold tracking-widest uppercase font-mono-jb( shrink-0)?">',
    r'<span class="text-[10px] font-bold tracking-widest uppercase font-mono-jb text-v-blue">',
    content
)

# And fix the right arrow character that got messed up due to encoding.
content = content.replace('Reserve <span class="ml-2 transform group-hover:translate-x-1 transition-transform">?</span>', 'Reserve <span class="ml-2 transform group-hover:translate-x-1 transition-transform">&rarr;</span>')

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)

print("Done")
