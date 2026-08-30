with open("Pages/Experiences.cshtml", "r", encoding="utf-8") as f:
    content = f.read()

content = content.replace(">SANTA-TERESA</span>", ">SANTA TERESA</span>")

with open("Pages/Experiences.cshtml", "w", encoding="utf-8") as f:
    f.write(content)
print("Fixed hyphen in Santa Teresa")
