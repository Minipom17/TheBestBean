import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

new_expeditions = """
                <!-- NEW: Santa Teresa High Altitude -->
                <article class="expedition-item flex flex-col group" data-location="santa-teresa" data-difficulty="advanced">
                    <a asp-page="/Tour" asp-route-id="11" class="text-decoration-none text-slate-900 flex flex-col h-full group">
                        <div class="w-full aspect-[21/9] bg-gray-100 mb-8 overflow-hidden rounded-3xl relative">
                             <img src="https://images.unsplash.com/photo-1542382156909-92500fa260f0?auto=format&fit=crop&w=1600&q=80" alt="Santa Teresa Sky Farm" class="img-stark absolute inset-0 w-full h-full object-cover" />
                        </div>
                        <div class="grid grid-cols-1 md:grid-cols-12 gap-12 flex-grow">
                            <div class="md:col-span-7 flex flex-col">
                                <div class="flex items-center gap-3 mb-4 font-mono-jb">
                                    <div class="w-3 h-3 bg-[#0057B7]"></div>
                                    <span class="text-[10px] font-bold uppercase tracking-widest text-v-black">SKY ELEVATION - 4 DAYS</span>
                                    <span class="px-2 py-0.5 bg-slate-100 text-slate-800 rounded-full text-[10px] font-bold tracking-widest uppercase">NOV 05</span>
                                </div>
                                <h3 class="text-3xl md:text-4xl font-semibold tracking-tight mb-4 m-0">Santa Teresa Sky Farm</h3>
                                <p class="text-v-gray text-base leading-relaxed mb-10 font-normal max-w-xl">
                                    Journey to one of the highest altitude coffee plots in the region. Discover how extreme diurnal temperature shifts create dense, intensely sweet coffees on the trail to Machu Picchu.
                                </p>
                                <div class="flex items-center gap-6 mt-auto">
                                    <span class="bg-v-yellow text-v-black px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:opacity-90 transition-opacity font-mono-jb">VIEW DOSSIER</span>
                                    <span class="text-2xl font-semibold text-v-black font-mono-jb">$1100 USD</span>
                                </div>
                            </div>
                            <div class="md:col-span-5 flex flex-col pt-2 md:pt-0 font-mono-jb">
                                <h4 class="text-[10px] font-bold uppercase tracking-widest text-v-gray mb-4">SYLLABUS</h4>
                                <div class="flex flex-col border-t border-v-black font-sans">
                                    <div class="flex flex-col gap-1 py-4 border-b border-v-light-gray">
                                        <span class="font-mono text-[10px] font-bold text-v-black uppercase tracking-widest">DAY 01</span>
                                        <span class="text-sm font-normal text-v-gray">Trek & Acclimatization</span>
                                    </div>
                                    <div class="flex flex-col gap-1 py-4 border-b border-v-light-gray">
                                        <span class="font-mono text-[10px] font-bold text-v-black uppercase tracking-widest">DAY 02</span>
                                        <span class="text-sm font-normal text-v-gray">Canopy Shading & Harvesting</span>
                                    </div>
                                    <div class="flex flex-col gap-1 pt-4 pb-0">
                                        <span class="font-mono text-[10px] font-bold text-v-black uppercase tracking-widest">DAY 03</span>
                                        <span class="text-sm font-semibold text-v-black">High Altitude Processing</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </a>
                </article>

                <!-- NEW: Santa Teresa Experimental -->
                <article class="expedition-item flex flex-col group" data-location="santa-teresa" data-difficulty="intermediate">
                    <a asp-page="/Tour" asp-route-id="12" class="text-decoration-none text-slate-900 flex flex-col h-full group">
                        <div class="w-full aspect-[21/9] bg-gray-100 mb-8 overflow-hidden rounded-3xl relative">
                             <img src="https://images.unsplash.com/photo-1552554746-81a1bbbb866d?auto=format&fit=crop&w=1600&q=80" alt="Experimental Fermentation Lab" class="img-stark absolute inset-0 w-full h-full object-cover" />
                        </div>
                        <div class="grid grid-cols-1 md:grid-cols-12 gap-12 flex-grow">
                            <div class="md:col-span-7 flex flex-col">
                                <div class="flex items-center gap-3 mb-4 font-mono-jb">
                                    <div class="w-3 h-3 bg-[#F05A28]"></div>
                                    <span class="text-[10px] font-bold uppercase tracking-widest text-v-black">INNOVATION LAB - 3 DAYS</span>
                                    <span class="px-2 py-0.5 bg-slate-100 text-slate-800 rounded-full text-[10px] font-bold tracking-widest uppercase">NOV 12</span>
                                </div>
                                <h3 class="text-3xl md:text-4xl font-semibold tracking-tight mb-4 m-0">Innovation Processing</h3>
                                <p class="text-v-gray text-base leading-relaxed mb-10 font-normal max-w-xl">
                                    A deep dive into cutting-edge post-harvest processing. Work with thermal shock, anaerobic fermentation, and carbonic maceration in our partner lab.
                                </p>
                                <div class="flex items-center gap-6 mt-auto">
                                    <span class="bg-v-yellow text-v-black px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:opacity-90 transition-opacity font-mono-jb">VIEW DOSSIER</span>
                                    <span class="text-2xl font-semibold text-v-black font-mono-jb">$950 USD</span>
                                </div>
                            </div>
                            <div class="md:col-span-5 flex flex-col pt-2 md:pt-0 font-mono-jb">
                                <h4 class="text-[10px] font-bold uppercase tracking-widest text-v-gray mb-4">SYLLABUS</h4>
                                <div class="flex flex-col border-t border-v-black font-sans">
                                    <div class="flex flex-col gap-1 py-4 border-b border-v-light-gray">
                                        <span class="font-mono text-[10px] font-bold text-v-black uppercase tracking-widest">DAY 01</span>
                                        <span class="text-sm font-normal text-v-gray">Brix Readings & Microbial Selection</span>
                                    </div>
                                    <div class="flex flex-col gap-1 py-4 border-b border-v-light-gray">
                                        <span class="font-mono text-[10px] font-bold text-v-black uppercase tracking-widest">DAY 02</span>
                                        <span class="text-sm font-normal text-v-gray">Anaerobic Environment Control</span>
                                    </div>
                                    <div class="flex flex-col gap-1 pt-4 pb-0">
                                        <span class="font-mono text-[10px] font-bold text-v-black uppercase tracking-widest">DAY 03</span>
                                        <span class="text-sm font-semibold text-v-black">Drying Protocol & Profiling</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </a>
                </article>

                <!-- NEW: Santa Teresa Traditional -->
                <article class="expedition-item flex flex-col group" data-location="santa-teresa" data-difficulty="beginner">
                    <a asp-page="/Tour" asp-route-id="13" class="text-decoration-none text-slate-900 flex flex-col h-full group">
                        <div class="w-full aspect-[21/9] bg-gray-100 mb-8 overflow-hidden rounded-3xl relative">
                             <img src="https://images.unsplash.com/photo-1524414139215-35c9118a8677?auto=format&fit=crop&w=1600&q=80" alt="Heritage Washing Station" class="img-stark absolute inset-0 w-full h-full object-cover" />
                        </div>
                        <div class="grid grid-cols-1 md:grid-cols-12 gap-12 flex-grow">
                            <div class="md:col-span-7 flex flex-col">
                                <div class="flex items-center gap-3 mb-4 font-mono-jb">
                                    <div class="w-3 h-3 bg-v-black"></div>
                                    <span class="text-[10px] font-bold uppercase tracking-widest text-v-black">HERITAGE TRIP - 2 DAYS</span>
                                    <span class="px-2 py-0.5 bg-slate-100 text-slate-800 rounded-full text-[10px] font-bold tracking-widest uppercase">NOV 19</span>
                                </div>
                                <h3 class="text-3xl md:text-4xl font-semibold tracking-tight mb-4 m-0">Heritage Washing Station</h3>
                                <p class="text-v-gray text-base leading-relaxed mb-10 font-normal max-w-xl">
                                    Experience the timeless rhythm of traditional washed coffee. Hand-pick alongside local farmers, depulp using hand-cranked machinery, and dry the parchment on raised beds.
                                </p>
                                <div class="flex items-center gap-6 mt-auto">
                                    <span class="bg-v-yellow text-v-black px-8 py-4 font-bold uppercase tracking-widest text-[10px] group-hover:opacity-90 transition-opacity font-mono-jb">VIEW DOSSIER</span>
                                    <span class="text-2xl font-semibold text-v-black font-mono-jb">$800 USD</span>
                                </div>
                            </div>
                            <div class="md:col-span-5 flex flex-col pt-2 md:pt-0 font-mono-jb">
                                <h4 class="text-[10px] font-bold uppercase tracking-widest text-v-gray mb-4">SYLLABUS</h4>
                                <div class="flex flex-col border-t border-v-black font-sans">
                                    <div class="flex flex-col gap-1 py-4 border-b border-v-light-gray">
                                        <span class="font-mono text-[10px] font-bold text-v-black uppercase tracking-widest">DAY 01</span>
                                        <span class="text-sm font-normal text-v-gray">Traditional Harvesting & Depulping</span>
                                    </div>
                                    <div class="flex flex-col gap-1 pt-4 pb-0">
                                        <span class="font-mono text-[10px] font-bold text-v-black uppercase tracking-widest">DAY 02</span>
                                        <span class="text-sm font-semibold text-v-black">Washing & Sun Drying</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </a>
                </article>
"""

content = content.replace("                    </a>\n                </article>\n\n            </div>\n        </section>", "                    </a>\n                </article>\n" + new_expeditions + "\n            </div>\n        </section>")

js_update = """            // Expeditions filtering logic
            const expeditionLocationFilters = document.querySelectorAll('#expedition-location-filters .filter-btn');
            const expeditionDifficultyFilters = document.querySelectorAll('#expedition-difficulty-filters .filter-btn');
            const expeditionItems = document.querySelectorAll('.expedition-item');
            
            let currentExpeditionLocation = 'all';
            let currentExpeditionDifficulty = 'all';

            function filterExpeditions() {
                expeditionItems.forEach(item => {
                    const itemLocation = item.getAttribute('data-location') || 'all'; 
                    const itemDifficulty = item.getAttribute('data-difficulty');
                    
                    const matchesLocation = currentExpeditionLocation === 'all' || itemLocation === currentExpeditionLocation;
                    const matchesDifficulty = currentExpeditionDifficulty === 'all' || itemDifficulty === currentExpeditionDifficulty;

                    if (matchesLocation && matchesDifficulty) {
                        item.style.display = 'flex';
                    } else {
                        item.style.display = 'none';
                    }
                });
            }

            if(expeditionLocationFilters.length > 0) {
                expeditionLocationFilters.forEach(btn => {
                    btn.addEventListener('click', () => {
                        expeditionLocationFilters.forEach(b => {
                            b.classList.remove('active', 'border-b', 'border-v-black', 'pb-1', 'text-v-black');
                            b.classList.add('text-v-gray', 'hover:text-v-black');
                        });
                        btn.classList.add('active', 'border-b', 'border-v-black', 'pb-1', 'text-v-black');
                        btn.classList.remove('text-v-gray', 'hover:text-v-black');
                        
                        currentExpeditionLocation = btn.getAttribute('data-filter');
                        filterExpeditions();
                    });
                });
            }

            expeditionDifficultyFilters.forEach(btn => {"""

content = re.sub(r'// Expeditions filtering logic.*?expeditionDifficultyFilters\.forEach\(btn => \{', js_update, content, flags=re.DOTALL)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Experiences.cshtml")
