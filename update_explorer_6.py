import re

html_path = r"C:\Users\alext\Downloads\coffee_flavor_explorer (6).html"
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

with open("Pages/GreenBeans.cshtml", "r", encoding="utf-8") as f:
    gb_content = f.read()

# Replace styles
def replace_style(match):
    return f"<style>\n{styles}\n</style>"
gb_content = re.sub(r'<style>.*?</style>', replace_style, gb_content, count=1, flags=re.DOTALL)

# Replace explorer UI
def replace_ui(match):
    return f"""<!-- FLAVOR EXPLORER -->
        <div class="mb-16">
            <div class="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-20 pb-20">
{main_content}
            </div>
        </div>
        <!-- END FLAVOR EXPLORER -->"""
gb_content = re.sub(r'<!-- FLAVOR EXPLORER -->.*?<!-- END FLAVOR EXPLORER -->', replace_ui, gb_content, flags=re.DOTALL)

# Replace script
def replace_script(match):
    return f"<script>\n{script_content}\n</script>"
gb_content = re.sub(r'<script>[^<]*interactive-canvas[^<]*</script>', replace_script, gb_content, flags=re.DOTALL)

with open("Pages/GreenBeans.cshtml", "w", encoding="utf-8") as f:
    f.write(gb_content)

print("Updated GreenBeans.cshtml with explorer 6")
