import re

html_path = r"C:\Users\alext\Downloads\coffee_flavor_explorer (1).html"
with open(html_path, "r", encoding="utf-8") as f:
    explorer_html = f.read()

# Extract styles
styles_match = re.search(r'<style>(.*?)</style>', explorer_html, re.DOTALL)
styles = styles_match.group(1) if styles_match else ""

# Extract main content
main_match = re.search(r'<main[^>]*>(.*?)</main>', explorer_html, re.DOTALL)
main_content = main_match.group(1) if main_match else ""

# Extract script
script_match = re.search(r'<script>(.*?)</script>', explorer_html, re.DOTALL)
script_content = script_match.group(1) if script_match else ""

fonts = '<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&family=Space+Mono:ital,wght@0,400;0,700;1,400&display=swap" rel="stylesheet">'

# Read GreenBeans.cshtml
with open("Pages/GreenBeans.cshtml", "r", encoding="utf-8") as f:
    gb_content = f.read()

# We'll inject the styles and fonts at the top, just after the @{...} block.
# We'll inject the main_content right before the <!-- Simple Filters --> div.
# We'll inject the script_content into the @section Scripts if there is one, or just at the end.

# 1. Styles & Fonts
header_injection = f"""
{fonts}
<style>
{styles}
</style>
"""
# Insert after } of @{ ... }
gb_content = re.sub(r'(ViewData\["Title"\].*?\n\})', r'\1\n' + header_injection, gb_content, count=1, flags=re.DOTALL)

# 2. Main Content
# We can wrap it in a div so it doesn't break the existing container
explorer_ui = f"""
        <!-- FLAVOR EXPLORER -->
        <div class="mb-16">
            <div class="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-20 pb-20">
{main_content}
            </div>
        </div>
        <!-- END FLAVOR EXPLORER -->
"""

gb_content = gb_content.replace('<!-- Simple Filters -->', explorer_ui + '\n        <!-- Simple Filters -->')

# 3. Script
script_injection = f"""
<script>
{script_content}
</script>
"""
gb_content += '\n' + script_injection

with open("Pages/GreenBeans.cshtml", "w", encoding="utf-8") as f:
    f.write(gb_content)

print("Flavor explorer injected into GreenBeans.cshtml")
