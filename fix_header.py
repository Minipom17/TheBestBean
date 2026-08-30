with open("Pages/Shared/_Header.cshtml", "r", encoding="utf-8") as f:
    content = f.read()

new_header = """@model int

<!-- TOP STICKY BAR (Swiss Master Architecture) -->
<header class="px-6 md:px-12 py-3 flex flex-col justify-between text-[10px] font-bold uppercase tracking-widest rule-thin sticky top-0 bg-v-white/95 backdrop-blur-sm z-50 text-v-black font-mono-jb">
    
    <!-- Top Row (Always Visible) -->
    <div class="flex flex-row justify-between items-center w-full">
        <!-- Left Side: Brand -->
        <div class="flex items-center gap-6 text-v-gray">
            <a asp-page="/Index" class="flex flex-col text-decoration-none group mr-2">
                <span class="font-bold text-v-black tracking-tighter text-base md:text-lg font-sans leading-none group-hover:text-v-red transition-colors">12° SUR</span>
                <span class="text-[7px] text-v-gray uppercase tracking-widest font-mono-jb mt-[2px] leading-none">Andean Coffee Experiences</span>
            </a>
            <span class="hidden lg:inline">LA CONVENCIÓN 1700-2300M</span>
        </div>

        <!-- Right Side: Utility & Cart & Toggle -->
        <div class="flex items-center gap-4 lg:gap-6">
            <button type="button" id="cmarket-toggle" class="group hidden md:flex items-center gap-1.5 px-2 py-0.5 border border-v-black/20 hover:border-v-black hover:bg-v-black hover:text-v-white transition-all cursor-pointer text-[10px] font-mono-jb uppercase tracking-widest text-v-black" title="Click to toggle between Kilograms and Pounds">
                <span>C-MARKET:</span>
                <span id="cmarket-price" class="text-v-blue group-hover:text-v-yellow font-bold transition-colors">$5.56/KG</span>
                <span id="cmarket-unit-hint" class="text-[8px] text-v-gray group-hover:text-v-white/70 ml-0.5">[⇄ LB]</span>
            </button>
            
            <div class="flex flex-col items-end gap-[3px]">
                <a asp-page="/Cart" class="hover:text-v-red transition-colors text-decoration-none font-bold text-v-black leading-none py-[1px] block">[ CART: @Model ]</a>
                <a asp-page="/Experiences" class="text-v-black flex items-center gap-1.5 leading-none py-[1px] block text-[9px] hover:text-v-green transition-colors text-decoration-none">
                    <span class="w-1.5 h-1.5 rounded-full bg-v-green animate-pulse inline-block"></span>
                    <span>BOOKINGS OPEN</span>
                </a>
            </div>

            <!-- Mobile Hamburger/Chevron Toggle -->
            <button id="mobile-nav-toggle" class="lg:hidden p-2 text-v-black focus:outline-none hover:bg-gray-100 rounded">
                <svg id="chevron-icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="transition-transform duration-300">
                    <polyline points="6 9 12 15 18 9"></polyline>
                </svg>
            </button>
        </div>
    </div>

    <!-- Bottom Row (Collapsible Menu for Mobile, Inline for Desktop) -->
    <nav id="mobile-nav-menu" class="hidden lg:flex flex-col lg:flex-row items-start lg:items-center w-full mt-4 lg:mt-0 gap-4 lg:gap-8 text-v-gray justify-start lg:justify-end pt-4 lg:pt-0 border-t lg:border-t-0 border-gray-200">
        <a asp-page="/GreenBeans" class="hover:text-v-black transition-colors text-decoration-none w-full lg:w-auto py-2 lg:py-0">COFFEE</a>
        <a asp-page="/BrewMethods" class="hover:text-v-black transition-colors hidden sm:block text-decoration-none w-full lg:w-auto py-2 lg:py-0">BREW GUIDES</a>
        <a asp-page="/Experiences" class="hover:text-v-black transition-colors text-decoration-none w-full lg:w-auto py-2 lg:py-0">EXPERIENCES</a>
        <a asp-page="/Wholesale" class="hover:text-v-black transition-colors hidden md:block text-decoration-none w-full lg:w-auto py-2 lg:py-0">WHOLESALE</a>
        <a asp-page="/OS" class="hover:text-v-blue text-v-black transition-colors text-decoration-none font-bold w-full lg:w-auto py-2 lg:py-0">OS</a>
    </nav>
</header>

<script>
    document.addEventListener('DOMContentLoaded', function() {
        // Mobile Toggle Logic
        const navToggle = document.getElementById('mobile-nav-toggle');
        const navMenu = document.getElementById('mobile-nav-menu');
        const chevronIcon = document.getElementById('chevron-icon');

        if (navToggle && navMenu) {
            navToggle.addEventListener('click', function() {
                navMenu.classList.toggle('hidden');
                navMenu.classList.toggle('flex');
                if (navMenu.classList.contains('hidden')) {
                    chevronIcon.style.transform = 'rotate(0deg)';
                } else {
                    chevronIcon.style.transform = 'rotate(180deg)';
                }
            });
        }

        // C-Market Logic
        const toggleBtn = document.getElementById('cmarket-toggle');
        const priceSpan = document.getElementById('cmarket-price');
        const hintSpan = document.getElementById('cmarket-unit-hint');
        
        if (!toggleBtn || !priceSpan || !hintSpan) return;

        const basePriceLb = 2.52;
        const kgFactor = 2.20462;
        const basePriceKg = (basePriceLb * kgFactor).toFixed(2); // 5.56

        function setUnit(unit) {
            if (unit === 'LB') {
                priceSpan.textContent = `$${basePriceLb.toFixed(2)}/LB`;
                hintSpan.textContent = '[⇄ KG]';
                toggleBtn.setAttribute('title', 'Click to toggle between Pounds and Kilograms');
                localStorage.setItem('cmarket_unit', 'LB');
            } else {
                priceSpan.textContent = `$${basePriceKg}/KG`;
                hintSpan.textContent = '[⇄ LB]';
                toggleBtn.setAttribute('title', 'Click to toggle between Kilograms and Pounds');
                localStorage.setItem('cmarket_unit', 'KG');
            }
        }

        const savedUnit = localStorage.getItem('cmarket_unit') || 'KG';
        setUnit(savedUnit);

        toggleBtn.addEventListener('click', function() {
            const currentUnit = localStorage.getItem('cmarket_unit') || 'KG';
            setUnit(currentUnit === 'KG' ? 'LB' : 'KG');
        });
    });
</script>
"""

# Replace all content
with open("Pages/Shared/_Header.cshtml", "w", encoding="utf-8") as f:
    f.write(new_header)
print("Updated _Header.cshtml")
