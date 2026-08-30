import re

# 1. Update _Header.cshtml to remove LA CONVENCIÓN 1700-2300M
with open("Pages/Shared/_Header.cshtml", "r", encoding="utf-8") as f:
    header = f.read()

header = header.replace('<span class="hidden lg:inline">LA CONVENCIÓN 1700-2300M</span>', '')
with open("Pages/Shared/_Header.cshtml", "w", encoding="utf-8") as f:
    f.write(header)

# 2. Update _Layout.cshtml to add theme toggle and variables
with open("Pages/Shared/_Layout.cshtml", "r", encoding="utf-8") as f:
    layout = f.read()

tailwind_config_old = """        tailwind.config = {
            theme: {
                extend: {
                    colors: {
                        'v-white': '#FFFFFF',
                        'v-black': '#000000',
                        'v-gray': '#737373',
                        'v-light-gray': '#E5E5E5',"""

tailwind_config_new = """        tailwind.config = {
            darkMode: 'class',
            theme: {
                extend: {
                    colors: {
                        'v-white': 'var(--color-v-white)',
                        'v-black': 'var(--color-v-black)',
                        'v-gray': 'var(--color-v-gray)',
                        'v-light-gray': 'var(--color-v-light-gray)',"""
layout = layout.replace(tailwind_config_old, tailwind_config_new)

style_insertion = """    <style>
        :root {
            --color-v-white: #FFFFFF;
            --color-v-black: #000000;
            --color-v-gray: #737373;
            --color-v-light-gray: #E5E5E5;
        }
        html.dark {
            --color-v-white: #121212;
            --color-v-black: #F5F5F5;
            --color-v-gray: #A3A3A3;
            --color-v-light-gray: #333333;
            background-color: #121212;
            color: #F5F5F5;
        }"""
layout = layout.replace("    <style>", style_insertion)

body_tag_old = '<body class="min-h-screen flex flex-col font-sans text-sm selection:bg-v-black selection:text-v-white relative bg-v-white text-v-black">'
body_tag_new = '<body class="min-h-screen flex flex-col font-sans text-sm selection:bg-v-black selection:text-v-white relative bg-v-white text-v-black transition-colors duration-300">'
layout = layout.replace(body_tag_old, body_tag_new)

# Add the toggle button
# User said: "sandwiched in there right at the top right corner to save space.. I don't want any padding around it"
toggle_btn = """
    <!-- Theme Toggle (Top Right) -->
    <button id="theme-toggle" class="fixed top-0 right-0 z-[1000] w-8 h-8 flex items-center justify-center bg-transparent text-v-black cursor-pointer mix-blend-difference hover:opacity-70 transition-opacity m-0 p-0 border-none outline-none" title="Toggle Theme">
        <!-- Sun icon (visible in dark mode) -->
        <svg id="theme-icon-sun" class="hidden w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"></path>
        </svg>
        <!-- Moon icon (visible in light mode) -->
        <svg id="theme-icon-moon" class="w-4 h-4 text-black" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"></path>
        </svg>
    </button>
"""
layout = layout.replace("    <partial name=\"_Header\" model=\"cartCount\" />", toggle_btn + "\n    <partial name=\"_Header\" model=\"cartCount\" />")

# Add JS logic
js_logic = """
        // Theme Toggle Logic
        const themeToggle = document.getElementById('theme-toggle');
        const iconSun = document.getElementById('theme-icon-sun');
        const iconMoon = document.getElementById('theme-icon-moon');
        
        function setTheme(isDark) {
            if (isDark) {
                document.documentElement.classList.add('dark');
                iconSun.classList.remove('hidden');
                iconMoon.classList.add('hidden');
                localStorage.setItem('theme', 'dark');
            } else {
                document.documentElement.classList.remove('dark');
                iconSun.classList.add('hidden');
                iconMoon.classList.remove('hidden');
                localStorage.setItem('theme', 'light');
            }
        }
        
        // Init
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'dark') {
            setTheme(true);
        } else {
            setTheme(false);
        }
        
        themeToggle.addEventListener('click', () => {
            const isDark = document.documentElement.classList.contains('dark');
            setTheme(!isDark);
        });
"""
layout = layout.replace("    <script>", "    <script>" + js_logic)

with open("Pages/Shared/_Layout.cshtml", "w", encoding="utf-8") as f:
    f.write(layout)

print("Applied theme toggle and header updates")
