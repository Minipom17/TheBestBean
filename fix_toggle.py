import re

with open("Pages/Shared/_Header.cshtml", "r", encoding="utf-8") as f:
    header = f.read()

# I will place it completely separate in the header as an absolute element inside the <header> tag which is sticky top-0
toggle_fixed = """
    <!-- Mobile & Desktop Theme Toggle (Absolute Top Right Corner) -->
    <button id="theme-toggle" class="absolute top-0 right-0 bg-gray-200 dark:bg-gray-800 text-v-black dark:text-[#F5F5F5] flex items-center justify-center p-1 m-0 hover:bg-gray-300 dark:hover:bg-gray-700 transition-colors cursor-pointer" style="width: 24px; height: 24px;" title="Toggle Theme">
        <svg id="theme-icon-sun" class="hidden w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"></path>
        </svg>
        <svg id="theme-icon-moon" class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"></path>
        </svg>
    </button>
"""

# Remove the old toggle from inside the div
header = re.sub(r'<!-- Theme Toggle -->.*?</button>', '', header, flags=re.DOTALL)

# Inject the new absolute toggle right after the opening <header> tag
header = re.sub(r'(<header[^>]*>)', r'\1' + '\n' + toggle_fixed, header)

with open("Pages/Shared/_Header.cshtml", "w", encoding="utf-8") as f:
    f.write(header)

print("Toggle fixed to absolute top right corner of the header.")
