import re

with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# We need to add mock tours for ID 8, 9, 10
# Find the end of the return new List<TourItem>
new_tours = """
                ,
                new TourItem
                {
                    Id = 8,
                    Title = "ADVANCED CUPPING",
                    Subtitle = "Sensory Calibration",
                    Duration = "120 MIN",
                    Level = "INTERMEDIATE",
                    Price = 55.00m,
                    Type = "Workshop",
                    Location = "Calca",
                    ImageUrl = "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?auto=format&fit=crop&q=80&w=1200",
                    GalleryImages = new List<string>
                    {
                        "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?auto=format&fit=crop&q=80&w=600"
                    },
                    Description = "Detailed cupping sessions comparing rare lots. Discuss senses, share notes, and calibrate your palate alongside professionals.",
                    LongDescription = "Advanced cupping for those looking to calibrate their palate to professional standards.",
                    Syllabus = new List<string> { "Triangulation Cupping", "Defect Recognition", "SCA Score Sheet Mastery" },
                    ProvidedEquipment = new List<string> { "Cupping Spoon", "Score Sheets", "Spittoon" },
                    RequiredGear = new List<string> { "Clean Palate" }
                },
                new TourItem
                {
                    Id = 9,
                    Title = "SCA CERTIFICATION",
                    Subtitle = "Professional Exams",
                    Duration = "FULL DAY",
                    Level = "ADVANCED",
                    Price = 450.00m,
                    Type = "Workshop",
                    Location = "Special Events",
                    ImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?auto=format&fit=crop&q=80&w=1200",
                    GalleryImages = new List<string>
                    {
                        "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?auto=format&fit=crop&q=80&w=600"
                    },
                    Description = "An intensive seminar diving into deep coffee theory and science. Led by a certified SCA professional with official certification exams.",
                    LongDescription = "Earn your SCA Certification in this intensive multi-day workshop covering Sensory Skills and Green Coffee.",
                    Syllabus = new List<string> { "SCA Sensory Skills", "SCA Green Coffee", "Written & Practical Exams" },
                    ProvidedEquipment = new List<string> { "Study Materials", "Exam Fees Included", "Sample Sets" },
                    RequiredGear = new List<string> { "Notebook", "Pen" }
                },
                new TourItem
                {
                    Id = 10,
                    Title = "MASTER APPRENTICESHIP",
                    Subtitle = "Farm Processing Immersion",
                    Duration = "72 HOURS",
                    Level = "PRO LEVEL",
                    Price = 850.00m,
                    Type = "Expedition",
                    Location = "Cusco Highlands",
                    ImageUrl = "https://images.unsplash.com/photo-1511556820780-d912e42b4980?auto=format&fit=crop&q=80&w=1600",
                    GalleryImages = new List<string>
                    {
                        "https://images.unsplash.com/photo-1511556820780-d912e42b4980?auto=format&fit=crop&q=80&w=600"
                    },
                    Description = "A highly technical, multi-day origin deep dive designed for professionals. Work directly with farm owners on agronomy, soil health, and experimental lot processing.",
                    LongDescription = "Join our Master Apprenticeship program and live on a coffee farm, learning directly from producers.",
                    Syllabus = new List<string> { "Agronomy & Soil Science", "Experimental Processing", "Advanced Roasting & Q-Grading" },
                    ProvidedEquipment = new List<string> { "Lodging", "Meals", "Processing Equipment" },
                    RequiredGear = new List<string> { "Boots", "Rain Gear", "Notebook" }
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
