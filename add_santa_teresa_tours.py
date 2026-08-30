import re

with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

new_tours = """
                ,
                new TourItem
                {
                    Id = 11,
                    Title = "SANTA TERESA SKY FARM",
                    Subtitle = "High Altitude Cultivation",
                    Duration = "4 DAYS",
                    Level = "ADVANCED",
                    Price = 1100.00m,
                    Type = "Expedition",
                    Location = "Santa Teresa",
                    ImageUrl = "https://images.unsplash.com/photo-1542382156909-92500fa260f0?auto=format&fit=crop&q=80&w=1600",
                    GalleryImages = new List<string>
                    {
                        "https://images.unsplash.com/photo-1542382156909-92500fa260f0?auto=format&fit=crop&q=80&w=600"
                    },
                    Description = "Journey to one of the highest altitude coffee plots in the region. Discover how extreme diurnal temperature shifts create dense, intensely sweet coffees on the trail to Machu Picchu.",
                    LongDescription = "Experience high altitude coffee farming on the trail to Machu Picchu. You will witness extreme diurnal temperature shifts and their effect on coffee density and sweetness.",
                    Syllabus = new List<string> { "Trek & Acclimatization", "Canopy Shading & Harvesting", "High Altitude Processing" },
                    ProvidedEquipment = new List<string> { "Trekking Gear", "Lodging", "Meals" },
                    RequiredGear = new List<string> { "Hiking Boots", "Warm Clothing" }
                },
                new TourItem
                {
                    Id = 12,
                    Title = "INNOVATION PROCESSING",
                    Subtitle = "Experimental Fermentation Lab",
                    Duration = "3 DAYS",
                    Level = "INTERMEDIATE",
                    Price = 950.00m,
                    Type = "Expedition",
                    Location = "Santa Teresa",
                    ImageUrl = "https://images.unsplash.com/photo-1552554746-81a1bbbb866d?auto=format&fit=crop&q=80&w=1600",
                    GalleryImages = new List<string>
                    {
                        "https://images.unsplash.com/photo-1552554746-81a1bbbb866d?auto=format&fit=crop&q=80&w=600"
                    },
                    Description = "A deep dive into cutting-edge post-harvest processing. Work with thermal shock, anaerobic fermentation, and carbonic maceration in our partner lab.",
                    LongDescription = "Explore the science of coffee fermentation in Santa Teresa. From Brix readings to drying protocols, this expedition covers it all.",
                    Syllabus = new List<string> { "Brix Readings & Microbial Selection", "Anaerobic Environment Control", "Drying Protocol & Profiling" },
                    ProvidedEquipment = new List<string> { "Lab Equipment", "Lodging", "Meals" },
                    RequiredGear = new List<string> { "Notebook", "Thermometer" }
                },
                new TourItem
                {
                    Id = 13,
                    Title = "HERITAGE WASHING STATION",
                    Subtitle = "Traditional Processes",
                    Duration = "2 DAYS",
                    Level = "BEGINNER",
                    Price = 800.00m,
                    Type = "Expedition",
                    Location = "Santa Teresa",
                    ImageUrl = "https://images.unsplash.com/photo-1524414139215-35c9118a8677?auto=format&fit=crop&q=80&w=1600",
                    GalleryImages = new List<string>
                    {
                        "https://images.unsplash.com/photo-1524414139215-35c9118a8677?auto=format&fit=crop&q=80&w=600"
                    },
                    Description = "Experience the timeless rhythm of traditional washed coffee. Hand-pick alongside local farmers, depulp using hand-cranked machinery, and dry the parchment on raised beds.",
                    LongDescription = "Immerse yourself in the traditional methods of coffee washing. A hands-on experience covering everything from harvesting to sun drying.",
                    Syllabus = new List<string> { "Traditional Harvesting & Depulping", "Washing & Sun Drying" },
                    ProvidedEquipment = new List<string> { "Harvesting Basket", "Lodging", "Meals" },
                    RequiredGear = new List<string> { "Comfortable Shoes", "Hat" }
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
