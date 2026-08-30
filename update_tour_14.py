import re

with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# Update the Tour 14
def replace_tour_14(match):
    return """new TourItem
                {
                    Id = 14,
                    Title = "ROASTING LAB",
                    Subtitle = "Applied Thermodynamics",
                    Duration = "150 MIN",
                    Level = "INTERMEDIATE",
                    Price = 85.00m,
                    Type = "Workshop",
                    Location = "Lima",
                    ImageUrl = "/images/20260422_141746.jpg",
                    GalleryImages = new List<string>
                    {
                        "/images/20251227_115647.jpg",
                        "/images/20260129_111246.jpg",
                        "/images/20260129_111259.mp4",
                        "/images/20260207_110342.jpg",
                        "/images/20260610_154127.jpg"
                    },
                    Description = "Learn the art and science of coffee roasting. Understand heat transfer, development time, and how to manipulate flavor profiles in the drum.",
                    LongDescription = "A deep dive into roasting profiles. From green coffee density to first crack, you will roast your own batch under expert supervision.",
                    Syllabus = new List<string> { "Thermodynamics of Roasting", "Sample Roasting", "Profile Development" },
                    ProvidedEquipment = new List<string> { "Sample Roaster", "Green Coffee", "Agtron Meter" },
                    RequiredGear = new List<string> { "Notebook", "Closed-toe Shoes" }
                }"""

pattern = r'new TourItem\s*\{\s*Id = 14,.*?RequiredGear = new List<string> \{.*?\}\s*\}'
content = re.sub(pattern, replace_tour_14, content, flags=re.DOTALL)

with open('Pages/Tour.cshtml.cs', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Tour.cshtml.cs for Roasting")
