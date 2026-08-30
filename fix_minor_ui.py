import re

with open('Pages/Tour.cshtml', 'r', encoding='utf-8') as f:
    tour_content = f.read()

# Fix Tour layout
tour_content = tour_content.replace(
    '<div class="flex justify-between items-center text-sm">',
    '<div class="flex justify-between items-start text-sm gap-4">'
)
tour_content = tour_content.replace(
    '<span class="text-v-gray font-normal">Duration</span>',
    '<span class="text-v-gray font-normal shrink-0">Duration</span>'
)
tour_content = tour_content.replace(
    '<span class="text-v-gray font-normal">Location</span>',
    '<span class="text-v-gray font-normal shrink-0">Location</span>'
)
tour_content = tour_content.replace(
    '<span class="text-v-gray font-normal">Level</span>',
    '<span class="text-v-gray font-normal shrink-0">Level</span>'
)
tour_content = tour_content.replace(
    '<span class="text-v-black font-semibold">@Model.Tour.Duration</span>',
    '<span class="text-v-black font-semibold text-right">@Model.Tour.Duration</span>'
)
tour_content = tour_content.replace(
    '<span class="text-v-black font-semibold">@Model.Tour.Location</span>',
    '<span class="text-v-black font-semibold text-right">@Model.Tour.Location</span>'
)
tour_content = tour_content.replace(
    '<span class="text-v-black font-semibold">@Model.Tour.Level</span>',
    '<span class="text-v-black font-semibold text-right">@Model.Tour.Level</span>'
)

with open('Pages/Tour.cshtml', 'w', encoding='utf-8') as f:
    f.write(tour_content)

# Fix Experiences filters
with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    exp_content = f.read()

# Replace active filter button class
exp_content = exp_content.replace(
    'class="filter-btn active px-3 py-1.5 border border-v-black text-[10px] font-bold font-mono-jb uppercase tracking-widest bg-v-black text-v-white transition-colors"',
    'class="filter-btn active border-b border-v-black pb-1 mr-2 text-[10px] font-bold font-mono-jb uppercase tracking-widest text-v-black transition-colors bg-transparent"'
)

# Replace inactive filter button class
exp_content = exp_content.replace(
    'class="filter-btn px-3 py-1.5 border border-v-black text-[10px] font-bold font-mono-jb uppercase tracking-widest text-v-black hover:bg-v-light-gray transition-colors"',
    'class="filter-btn mr-2 text-[10px] font-bold font-mono-jb uppercase tracking-widest text-v-gray hover:text-v-black transition-colors bg-transparent border-none p-0 cursor-pointer"'
)

# Replace gap-2 with gap-4 for the filter containers
exp_content = exp_content.replace(
    '<div class="flex flex-wrap gap-2"',
    '<div class="flex flex-wrap gap-4"'
)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(exp_content)

print("Done")
