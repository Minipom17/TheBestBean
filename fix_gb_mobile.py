import re

with open("Pages/GreenBeans.cshtml", "r", encoding="utf-8") as f:
    content = f.read()

# 1. Reverse mobile order
# <div class="lg:col-span-7 flex flex-col justify-center relative w-full">
content = content.replace(
    '<div class="lg:col-span-7 flex flex-col justify-center relative w-full">',
    '<div class="lg:col-span-7 flex flex-col justify-center relative w-full order-2 lg:order-1">'
)

# <div class="lg:col-span-5 flex flex-col justify-center">
content = content.replace(
    '<div class="lg:col-span-5 flex flex-col justify-center">',
    '<div class="lg:col-span-5 flex flex-col justify-center order-1 lg:order-2">'
)

# 2. Fix the getClosestActiveNodeIndex function
content = content.replace(
    'return closestIdx !== -1 ? closestIdx : 0;',
    'return closestIdx !== -1 ? closestIdx : 0;' # Keep it so they can easily grab the main node
)

# 3. Add pointInTriangle check to mousedown and touchstart
mousedown_replace = """canvas.addEventListener('mousedown', (e) => {
                const rect = canvas.getBoundingClientRect();
                const mousePos = { x: e.clientX - rect.left, y: e.clientY - rect.top };
                
                // Check if inside triangle
                const bary = getBarycentric(mousePos, triangle.A, triangle.B, triangle.C);
                if (bary.t < -0.05 || bary.br < -0.05 || bary.bl < -0.05) return; // Outside triangle
                
                draggingNodeIndex = getClosestActiveNodeIndex(mousePos);"""
content = re.sub(
    r"canvas\.addEventListener\('mousedown', \(e\) => \{\s*const rect = canvas\.getBoundingClientRect\(\);\s*const mousePos = \{ x: e\.clientX - rect\.left, y: e\.clientY - rect\.top \};\s*draggingNodeIndex = getClosestActiveNodeIndex\(mousePos\);",
    mousedown_replace,
    content
)

touchstart_replace = """canvas.addEventListener('touchstart', (e) => {
                const rect = canvas.getBoundingClientRect();
                const touchPos = { x: e.touches[0].clientX - rect.left, y: e.touches[0].clientY - rect.top };
                
                const bary = getBarycentric(touchPos, triangle.A, triangle.B, triangle.C);
                if (bary.t < -0.05 || bary.br < -0.05 || bary.bl < -0.05) return; // Outside triangle
                
                e.preventDefault(); 
                draggingNodeIndex = getClosestActiveNodeIndex(touchPos);"""
content = re.sub(
    r"canvas\.addEventListener\('touchstart', \(e\) => \{\s*e\.preventDefault\(\);\s*const rect = canvas\.getBoundingClientRect\(\);\s*const touchPos = \{ x: e\.touches\[0\]\.clientX - rect\.left, y: e\.touches\[0\]\.clientY - rect\.top \};\s*draggingNodeIndex = getClosestActiveNodeIndex\(touchPos\);",
    touchstart_replace,
    content
)


with open("Pages/GreenBeans.cshtml", "w", encoding="utf-8") as f:
    f.write(content)
print("Updated GreenBeans.cshtml")
