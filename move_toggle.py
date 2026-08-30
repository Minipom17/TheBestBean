import re

# Remove toggle from _Layout.cshtml
with open("Pages/Shared/_Layout.cshtml", "r", encoding="utf-8") as f:
    layout = f.read()

pattern_toggle = r'<!-- Theme Toggle \(Top Right\) -->.*?</button>'
layout = re.sub(pattern_toggle, '', layout, flags=re.DOTALL)
with open("Pages/Shared/_Layout.cshtml", "w", encoding="utf-8") as f:
    f.write(layout)

# Add toggle to _Header.cshtml at the top right
with open("Pages/Shared/_Header.cshtml", "r", encoding="utf-8") as f:
    header = f.read()

toggle_btn = """
        <!-- Theme Toggle -->
        <button id="theme-toggle" class="flex items-center justify-center p-0 m-0 ml-2 text-v-black dark:text-[#F5F5F5] hover:opacity-70 transition-opacity cursor-pointer" title="Toggle Theme">
            <svg id="theme-icon-sun" class="hidden w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"></path>
            </svg>
            <svg id="theme-icon-moon" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"></path>
            </svg>
        </button>
"""

# Insert right after the BOOKINGS OPEN div
header = header.replace('<span>BOOKINGS OPEN</span>\n            </a>\n        </div>', '<span>BOOKINGS OPEN</span>\n            </a>\n        </div>' + toggle_btn)

with open("Pages/Shared/_Header.cshtml", "w", encoding="utf-8") as f:
    f.write(header)

print("Moved theme toggle to header")
