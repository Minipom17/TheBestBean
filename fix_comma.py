with open("Pages/Tour.cshtml.cs", "r", encoding="utf-8") as f:
    content = f.read()

content = content.replace("}\n            \n            new TourItem\n            {\n                Id = 15,", "},\n            new TourItem\n            {\n                Id = 15,")

with open("Pages/Tour.cshtml.cs", "w", encoding="utf-8") as f:
    f.write(content)
print("Fixed missing comma")
