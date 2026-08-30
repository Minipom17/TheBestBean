import re

with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# Update image for ID 11
content = content.replace('"https://images.unsplash.com/photo-1542382156909-92500fa260f0?auto=format&fit=crop&q=80&w=1600"', '"/images/hilda_drying.jpg"')
content = content.replace('"https://images.unsplash.com/photo-1542382156909-92500fa260f0?auto=format&fit=crop&q=80&w=600"', '"/images/hilda_drying.jpg"')

new_tours = """
                ,
                new TourItem
                {
                    Id = 14,
                    Title = "ROASTING LAB",
                    Subtitle = "Applied Thermodynamics",
                    Duration = "150 MIN",
                    Level = "INTERMEDIATE",
                    Price = 85.00m,
                    Type = "Workshop",
                    Location = "Lima",
                    ImageUrl = "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?auto=format&fit=crop&q=80&w=1200",
                    GalleryImages = new List<string>
                    {
                        "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?auto=format&fit=crop&q=80&w=600"
                    },
                    Description = "Learn the art and science of coffee roasting. Understand heat transfer, development time, and how to manipulate flavor profiles in the drum.",
                    LongDescription = "A deep dive into roasting profiles. From green coffee density to first crack, you will roast your own batch under expert supervision.",
                    Syllabus = new List<string> { "Thermodynamics of Roasting", "Sample Roasting", "Profile Development" },
                    ProvidedEquipment = new List<string> { "Sample Roaster", "Green Coffee", "Agtron Meter" },
                    RequiredGear = new List<string> { "Notebook", "Closed-toe Shoes" }
                }
            };
        }
    }
}
"""

content = content.replace("}\n                }\n            };\n        }\n    }\n}", "}\n                }" + new_tours)

with open('Pages/Tour.cshtml.cs', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Tour.cshtml.cs")
