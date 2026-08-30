import re

with open('Pages/Experiences.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

new_lab = """
                <!-- NEW: Roasting Lab -->
                <article class="lab-item flex flex-col group" data-location="lima" data-difficulty="intermediate">
                    <a asp-page="/Tour" asp-route-id="14" class="text-decoration-none text-slate-900 flex flex-col h-full group">
                        <div class="w-full aspect-[3/2] bg-gray-100 mb-6 overflow-hidden rounded-3xl relative">
                            <img src="/images/20260422_141746.jpg" alt="Roasting Lab" class="img-stark absolute inset-0 w-full h-full object-cover" />
                        </div>
                        <div class="flex justify-between items-center mb-4">
                            <h3 class="text-2xl md:text-3xl font-semibold tracking-tight text-v-black m-0">Roasting</h3>
                            <div class="flex items-center gap-2 shrink-0 font-mono-jb text-[10px] font-bold tracking-widest uppercase mt-1">
                                <div class="w-1.5 h-1.5 bg-[#F05A28]"></div>
                                <span class="text-gray-500">DEC</span>
                                <span class="text-gray-300">/</span>
                                <span class="text-[#F05A28]">INTERMEDIATE</span>
                            </div>
                        </div>
                        <p class="text-v-gray text-sm md:text-base leading-relaxed mb-6 pb-6 border-b border-v-black/10 font-normal flex-grow">
                            Learn the art and science of coffee roasting. Understand heat transfer, development time, and how to manipulate flavor profiles in the drum.
                        </p>
                        <div class="flex items-center justify-between mt-auto pt-4 font-mono-jb">
                            <span class="text-2xl font-bold text-v-black">$85</span>
                            <span class="text-sm font-bold text-v-black hover:text-v-gray transition-colors flex items-center">
                                Reserve <span class="ml-2 transform group-hover:translate-x-1 transition-transform">&rarr;</span>
                            </span>
                        </div>
                    </a>
                </article>
"""

# Insert right before SECTION: EXPEDITIONS
content = re.sub(r'(</article>)(\s*</div>\s*</section>\s*<!-- SECTION: EXPEDITIONS -->)', r'\1\n' + new_lab + r'\2', content, flags=re.DOTALL)

with open('Pages/Experiences.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
print("Added Roasting Lab")
