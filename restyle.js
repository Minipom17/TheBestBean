const fs = require('fs');
const path = require('path');

const filePath = path.join(__dirname, 'Pages', 'Experiences.cshtml');
let content = fs.readFileSync(filePath, 'utf8');

// 1. Featured Workshop Item
content = content.replace(
    /class="lab-item flex flex-col group md:col-span-2 border-2 border-v-black p-6 bg-v-white mb-2"/g,
    'class="lab-item flex flex-col group md:col-span-2 bg-white rounded-3xl p-6 shadow-sm border border-gray-100 mb-2 hover:shadow-md transition-shadow"'
);
content = content.replace(
    /class="md:col-span-5 aspect-\[16\/9\] bg-v-light-gray overflow-hidden relative block text-decoration-none"/g,
    'class="md:col-span-5 aspect-[16/9] bg-gray-100 rounded-2xl overflow-hidden relative block text-decoration-none"'
);
// Featured pill
content = content.replace(
    /class="px-2 py-1 bg-v-yellow text-v-black text-\[10px\] font-bold tracking-widest uppercase font-mono-jb shrink-0"/g,
    'class="px-3 py-1 bg-red-50 text-red-800 rounded-full text-[10px] font-bold tracking-widest uppercase font-mono-jb shrink-0"'
);

// 2. Standard Lab Items
content = content.replace(
    /<a asp-page="\/Tour" asp-route-id="(\d+)" class="text-decoration-none text-v-black flex flex-col h-full">/g,
    '<a asp-page="/Tour" asp-route-id="$1" class="text-decoration-none text-slate-900 flex flex-col h-full bg-white rounded-3xl p-6 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">'
);
content = content.replace(
    /<div class="w-full aspect-\[4\/3\] bg-v-light-gray mb-6 overflow-hidden relative">/g,
    '<div class="w-full aspect-[4/3] bg-gray-100 mb-6 overflow-hidden rounded-2xl relative">'
);
content = content.replace(
    /<div class="w-full aspect-\[4\/3\] bg-v-light-gray mb-6 overflow-hidden relative border border-v-red">/g,
    '<div class="w-full aspect-[4/3] bg-gray-100 mb-6 overflow-hidden rounded-2xl relative border border-red-200">'
);
// Lab pills
content = content.replace(
    /class="px-2 py-1 bg-v-black text-v-white text-\[10px\] font-bold tracking-widest uppercase font-mono-jb shrink-0"/g,
    'class="px-3 py-1 bg-slate-900 text-white rounded-full text-[10px] font-bold tracking-widest uppercase font-mono-jb shrink-0"'
);

// 3. Expedition Items
content = content.replace(
    /<a asp-page="\/Tour" asp-route-id="(\d+)" class="text-decoration-none text-v-black block">/g,
    '<a asp-page="/Tour" asp-route-id="$1" class="text-decoration-none text-slate-900 block bg-white rounded-3xl p-6 md:p-10 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">'
);
content = content.replace(
    /<div class="w-full aspect-\[21\/9\] bg-v-light-gray mb-10 overflow-hidden relative">/g,
    '<div class="w-full aspect-[21/9] bg-gray-100 mb-10 overflow-hidden rounded-2xl relative">'
);

// General links
content = content.replace(
    /class="text-\[10px\] font-bold uppercase tracking-widest text-v-black group-hover:text-[a-z-]+ transition-colors flex items-center"/g,
    'class="text-[10px] font-bold uppercase tracking-widest text-slate-900 bg-gray-100 hover:bg-gray-200 px-4 py-2 rounded-full transition-colors flex items-center"'
);

// Tags
content = content.replace(
    /class="absolute top-3 left-3 bg-v-black text-v-white px-2 py-0.5 text-\[9px\] font-mono-jb font-bold uppercase tracking-widest"/g,
    'class="absolute top-3 left-3 bg-white text-slate-900 rounded-full px-3 py-1 text-[9px] font-mono-jb font-bold uppercase tracking-widest shadow-sm"'
);
content = content.replace(
    /class="absolute top-3 right-3 bg-v-red text-v-white px-2 py-0.5 text-\[9px\] font-mono-jb font-bold uppercase tracking-widest"/g,
    'class="absolute top-3 right-3 bg-red-600 text-white rounded-full px-3 py-1 text-[9px] font-mono-jb font-bold uppercase tracking-widest shadow-sm"'
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Styling updated.");
