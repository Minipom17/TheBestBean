import re

# -----------------
# 1. Experiences.cshtml
# -----------------
with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    exp_content = f.read()

# We need to insert the 4 new cards into the Urban Labs section.
# We can just append them right before the closing tag of the Urban Labs grid.
# The grid ends where the 'SECTION: EXPEDITIONS' starts.
# Actually, the Urban Labs grid is here: `<div class="lg:col-span-8 grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-16" id="labs-list">`
# Let's find `</div>\s*</section>\s*<!-- SECTION: EXPEDITIONS -->`

new_cards = """
                <!-- NEW: Roasting Beginner -->
                <article class="lab-item flex flex-col group" data-location="cusco" data-difficulty="beginner">
                    <a asp-page="/Tour" asp-route-id="15" class="text-decoration-none text-slate-900 flex flex-col h-full group">
                        <div class="w-full aspect-[3/2] bg-gray-100 mb-6 overflow-hidden rounded-3xl relative">
                            <img src="/images/20260129_111246.jpg" alt="Roasting Basics" class="img-stark absolute inset-0 w-full h-full object-cover" />
                        </div>
                        <div class="flex justify-between items-center mb-4">
                            <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">Roasting</h3>
                            <div class="flex items-center gap-2 shrink-0 font-mono-jb text-[10px] font-bold tracking-widest uppercase mt-1">
                                <div class="w-1.5 h-1.5 bg-[#4F46E5]"></div>
                                <span class="text-gray-500">JAN</span>
                                <span class="text-gray-300">/</span>
                                <span class="text-[#4F46E5]">BEGINNER</span>
                            </div>
                        </div>
                        <p class="text-v-gray text-sm md:text-base leading-relaxed mb-6 pb-6 border-b border-v-black/10 font-normal flex-grow">
                            An introduction to the roasting process. Discover the fundamentals of coffee roasting, from drying to first crack, and understand basic heat application.
                        </p>
                        <div class="flex items-center justify-between mt-auto pt-4 font-mono-jb">
                            <span class="text-2xl font-bold text-v-black">$65</span>
                            <span class="text-sm font-bold text-v-black hover:text-v-gray transition-colors flex items-center">
                                Reserve <span class="ml-2 transform group-hover:translate-x-1 transition-transform">&rarr;</span>
                            </span>
                        </div>
                    </a>
                </article>

                <!-- NEW: Roasting Advanced -->
                <article class="lab-item flex flex-col group" data-location="cusco" data-difficulty="advanced">
                    <a asp-page="/Tour" asp-route-id="16" class="text-decoration-none text-slate-900 flex flex-col h-full group">
                        <div class="w-full aspect-[3/2] bg-gray-100 mb-6 overflow-hidden rounded-3xl relative">
                            <img src="/images/20260610_154127.jpg" alt="Advanced Profile Roasting" class="img-stark absolute inset-0 w-full h-full object-cover" />
                        </div>
                        <div class="flex justify-between items-center mb-4">
                            <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">Roasting</h3>
                            <div class="flex items-center gap-2 shrink-0 font-mono-jb text-[10px] font-bold tracking-widest uppercase mt-1">
                                <div class="w-1.5 h-1.5 bg-black"></div>
                                <span class="text-gray-500">MAR</span>
                                <span class="text-gray-300">/</span>
                                <span class="text-black">ADVANCED</span>
                            </div>
                        </div>
                        <p class="text-v-gray text-sm md:text-base leading-relaxed mb-6 pb-6 border-b border-v-black/10 font-normal flex-grow">
                            Master curve manipulation and advanced heat transfer. Analyze Rate of Rise, manipulate development time, and execute complex roast profiles.
                        </p>
                        <div class="flex items-center justify-between mt-auto pt-4 font-mono-jb">
                            <span class="text-2xl font-bold text-v-black">$115</span>
                            <span class="text-sm font-bold text-v-black hover:text-v-gray transition-colors flex items-center">
                                Reserve <span class="ml-2 transform group-hover:translate-x-1 transition-transform">&rarr;</span>
                            </span>
                        </div>
                    </a>
                </article>

                <!-- NEW: Brewing Beginner -->
                <article class="lab-item flex flex-col group" data-location="cusco" data-difficulty="beginner">
                    <a asp-page="/Tour" asp-route-id="17" class="text-decoration-none text-slate-900 flex flex-col h-full group">
                        <div class="w-full aspect-[3/2] bg-gray-100 mb-6 overflow-hidden rounded-3xl relative">
                            <img src="/images/learning_Table_arial.jpg" alt="Brewing Foundations" class="img-stark absolute inset-0 w-full h-full object-cover" />
                        </div>
                        <div class="flex justify-between items-center mb-4">
                            <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">Brewing</h3>
                            <div class="flex items-center gap-2 shrink-0 font-mono-jb text-[10px] font-bold tracking-widest uppercase mt-1">
                                <div class="w-1.5 h-1.5 bg-[#4F46E5]"></div>
                                <span class="text-gray-500">FEB</span>
                                <span class="text-gray-300">/</span>
                                <span class="text-[#4F46E5]">BEGINNER</span>
                            </div>
                        </div>
                        <p class="text-v-gray text-sm md:text-base leading-relaxed mb-6 pb-6 border-b border-v-black/10 font-normal flex-grow">
                            Start your journey into manual brewing. Learn the basics of pour-over and immersion brewing to make a great cup at home.
                        </p>
                        <div class="flex items-center justify-between mt-auto pt-4 font-mono-jb">
                            <span class="text-2xl font-bold text-v-black">$35</span>
                            <span class="text-sm font-bold text-v-black hover:text-v-gray transition-colors flex items-center">
                                Reserve <span class="ml-2 transform group-hover:translate-x-1 transition-transform">&rarr;</span>
                            </span>
                        </div>
                    </a>
                </article>

                <!-- NEW: Brewing Advanced -->
                <article class="lab-item flex flex-col group" data-location="cusco" data-difficulty="advanced">
                    <a asp-page="/Tour" asp-route-id="18" class="text-decoration-none text-slate-900 flex flex-col h-full group">
                        <div class="w-full aspect-[3/2] bg-gray-100 mb-6 overflow-hidden rounded-3xl relative">
                            <img src="/images/TdsChart_learning.jpg" alt="Advanced Extraction Science" class="img-stark absolute inset-0 w-full h-full object-cover" />
                        </div>
                        <div class="flex justify-between items-center mb-4">
                            <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">Brewing</h3>
                            <div class="flex items-center gap-2 shrink-0 font-mono-jb text-[10px] font-bold tracking-widest uppercase mt-1">
                                <div class="w-1.5 h-1.5 bg-black"></div>
                                <span class="text-gray-500">MAY</span>
                                <span class="text-gray-300">/</span>
                                <span class="text-black">ADVANCED</span>
                            </div>
                        </div>
                        <p class="text-v-gray text-sm md:text-base leading-relaxed mb-6 pb-6 border-b border-v-black/10 font-normal flex-grow">
                            Push the boundaries of extraction and yield. Utilize refractometry to map extraction curves, and manipulate water chemistry.
                        </p>
                        <div class="flex items-center justify-between mt-auto pt-4 font-mono-jb">
                            <span class="text-2xl font-bold text-v-black">$65</span>
                            <span class="text-sm font-bold text-v-black hover:text-v-gray transition-colors flex items-center">
                                Reserve <span class="ml-2 transform group-hover:translate-x-1 transition-transform">&rarr;</span>
                            </span>
                        </div>
                    </a>
                </article>
"""

# Inject before the closing div of labs-list
exp_content = re.sub(r'(</div>\s*</section>\s*<!-- SECTION: EXPEDITIONS -->)', new_cards + r'\n\1', exp_content)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(exp_content)
print("Updated Experiences.cshtml")

# -----------------
# 2. Tour.cshtml
# -----------------
with open('Pages/Tour.cshtml', 'r', encoding='utf-8') as f:
    tour_content = f.read()

# Add a picture deck at the bottom.
# Inside the <main> block, after the equipment section, or spanning full width below main.
# Let's put it right after the </main> tag inside the flex-grow container.
gallery_html = """
        <!-- Picture Deck -->
        @if (Model.Tour.GalleryImages != null && Model.Tour.GalleryImages.Any())
        {
            <div class="mt-24 mb-12">
                <div class="flex items-center gap-4 mb-8">
                    <h2 class="text-3xl font-bold tracking-tight text-v-black m-0">ARCHIVE DOSSIER</h2>
                    <div class="h-px bg-gray-200 flex-grow"></div>
                </div>
                
                <div class="flex overflow-x-auto gap-6 pb-8 snap-x scrollbar-hide" style="scrollbar-width: none;">
                    @foreach (var img in Model.Tour.GalleryImages)
                    {
                        <div class="snap-center shrink-0 w-[80vw] md:w-[600px] aspect-[4/3] rounded-3xl overflow-hidden relative shadow-sm">
                            <img src="@img" class="absolute inset-0 w-full h-full object-cover img-stark" alt="Archive Image" loading="lazy" />
                        </div>
                    }
                </div>
            </div>
        }
"""

tour_content = re.sub(r'(</main>)', r'\1\n' + gallery_html, tour_content)

with open('Pages/Tour.cshtml', 'w', encoding='utf-8') as f:
    f.write(tour_content)
print("Updated Tour.cshtml")

