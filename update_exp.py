import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# 1. Replace Urban Labs outer <a> tag styling
content = re.sub(
    r'<a asp-page="/Tour" asp-route-id="(\d+)" class="text-decoration-none text-slate-900 flex flex-col h-full bg-white rounded-3xl p-6 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">',
    r'<a asp-page="/Tour" asp-route-id="\1" class="text-decoration-none text-slate-900 flex flex-col h-full group">',
    content
)

# 2. Replace aspect-[4/3] with aspect-[3/2] and rounded-2xl with rounded-3xl
content = re.sub(
    r'<div class="w-full aspect-\[4/3\] bg-gray-100 mb-6 overflow-hidden rounded-2xl relative( border border-red-200)?">',
    r'<div class="w-full aspect-[3/2] bg-gray-100 mb-6 overflow-hidden rounded-3xl relative\1">',
    content
)

# 3. Replace the Urban Lab footer (price & button)
content = re.sub(
    r'<div class="flex items-center justify-between mt-auto border-t border-v-light-gray pt-4 font-mono-jb">\s*<span class="text-2xl font-semibold text-v-black">\$(\d+) USD</span>\s*<span class="text-\[10px\] font-bold uppercase tracking-widest text-slate-900 bg-gray-100 hover:bg-gray-200 px-4 py-2 rounded-full transition-colors flex items-center">\s*.*?\s*</span>\s*</div>',
    r'<div class="flex items-center justify-between mt-auto pt-4 font-mono-jb">\n                            <span class="text-2xl font-bold text-v-black">$\1</span>\n                            <span class="text-sm font-bold text-v-black hover:text-v-gray transition-colors flex items-center">\n                                Reserve <span class="ml-2 transform group-hover:translate-x-1 transition-transform">?</span>\n                            </span>\n                        </div>',
    content
)

# 4. Modify the Urban Lab paragraphs to include the border bottom
content = re.sub(
    r'<p class="text-v-gray text-sm md:text-base leading-relaxed mb-10 font-normal pr-4 flex-grow">',
    r'<p class="text-v-gray text-sm md:text-base leading-relaxed mb-6 pb-6 border-b border-v-black/10 font-normal flex-grow">',
    content
)

# 5. Modify Origin Expeditions outer <a> tag
content = re.sub(
    r'<a asp-page="/Tour" asp-route-id="(\d+)" class="text-decoration-none text-slate-900 flex flex-col bg-white rounded-3xl p-6 md:p-10 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">',
    r'<a asp-page="/Tour" asp-route-id="\1" class="text-decoration-none text-slate-900 flex flex-col h-full group">',
    content
)

# For Expeditions, remove the mb-10 from the image container and make it rounded-3xl.
content = re.sub(
    r'<div class="w-full aspect-\[21/9\] bg-gray-100 mb-10 overflow-hidden rounded-2xl relative">',
    r'<div class="w-full aspect-[21/9] bg-gray-100 mb-8 overflow-hidden rounded-3xl relative">',
    content
)

# For Expeditions, change the VIEW DOSSIER buttons to be specifically colored.
content = content.replace(
    '<span class="bg-v-black text-v-white px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:bg-v-red group-hover:text-v-black transition-colors font-mono-jb">',
    '<span class="bg-v-red text-v-white px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:opacity-90 transition-opacity font-mono-jb">'
)
content = content.replace(
    '<span class="bg-v-black text-v-white px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:bg-v-blue group-hover:text-v-black transition-colors font-mono-jb">',
    '<span class="bg-v-blue text-v-white px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:opacity-90 transition-opacity font-mono-jb">'
)
content = content.replace(
    '<span class="bg-v-black text-v-white px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:bg-v-yellow group-hover:text-v-black transition-colors font-mono-jb">',
    '<span class="bg-v-yellow text-v-black px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:opacity-90 transition-opacity font-mono-jb">'
)

# The Coffee Project (Crash Course) - this one had different HTML, let's fix it too.
content = re.sub(
    r'<article class="lab-item flex flex-col group md:col-span-2 bg-white rounded-3xl p-6 shadow-sm border border-gray-100 mb-2 hover:shadow-md transition-shadow"(.*?)>',
    r'<article class="lab-item flex flex-col group md:col-span-2 mb-2"\1>',
    content
)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)

print("Done")
