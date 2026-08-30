import re

with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    cs_content = f.read()

# 1. Update Advanced Cupping (ID 8) Level to "Advanced"
def replace_tour_8(match):
    block = match.group(0)
    return re.sub(r'Level = "Intermediate"', 'Level = "Advanced"', block)

pattern_8 = r'new TourItem\s*\{\s*Id = 8,.*?\}'
cs_content = re.sub(pattern_8, replace_tour_8, cs_content, flags=re.DOTALL)

# 2. Duplicate Roasting (ID 14 - Intermediate) to ID 15 (Beginner) and ID 16 (Advanced)
# 3. Duplicate Brewing (ID 6 - Intermediate) to ID 17 (Beginner) and ID 18 (Advanced)

# We will just write a function to append new mock data to the list.
# Let's find the end of the mock data list.
def add_mock_tours(match):
    prefix = match.group(1)
    
    new_tours = """
            new TourItem
            {
                Id = 15,
                Title = "Roasting Basics",
                ShortDescription = "An introduction to the roasting process.",
                LongDescription = "Discover the fundamentals of coffee roasting. Learn the stages of the roast, from drying to first crack, and understand basic heat application.",
                Price = 65,
                Duration = "45 Min",
                Location = "Cusco",
                Type = "Urban Lab",
                Level = "Beginner",
                ImageUrl = "/images/20260129_111246.jpg",
                GalleryImages = new List<string> { "/images/20260422_141746.jpg", "/images/20260207_110342.jpg" },
                Syllabus = new List<string> { "Module 1: Green Coffee Analysis", "Module 2: The Roasting Curve", "Module 3: Tasting the Roast" },
                ProvidedEquipment = new List<string> { "Sample Roaster", "Green Beans" },
                RequiredGear = new List<string> { "Notebook" }
            },
            new TourItem
            {
                Id = 16,
                Title = "Advanced Profile Roasting",
                ShortDescription = "Master curve manipulation and advanced heat transfer.",
                LongDescription = "A deep dive into advanced roasting techniques. Analyze Rate of Rise (RoR), manipulate development time, and execute complex roast profiles to highlight specific terroir characteristics.",
                Price = 115,
                Duration = "120 Min",
                Location = "Cusco",
                Type = "Urban Lab",
                Level = "Advanced",
                ImageUrl = "/images/20260610_154127.jpg",
                GalleryImages = new List<string> { "/images/20251227_115647.jpg", "/images/20260129_111246.jpg" },
                Syllabus = new List<string> { "Module 1: Thermodynamics", "Module 2: RoR Analysis", "Module 3: Profile Execution" },
                ProvidedEquipment = new List<string> { "Production Roaster", "Cropster Software" },
                RequiredGear = new List<string> { "Data Log" }
            },
            new TourItem
            {
                Id = 17,
                Title = "Brewing Foundations",
                ShortDescription = "Start your journey into manual brewing.",
                LongDescription = "Learn the basics of pour-over and immersion brewing. Understand grind size, water temperature, and basic recipes to make a great cup at home.",
                Price = 35,
                Duration = "45 Min",
                Location = "Cusco",
                Type = "Urban Lab",
                Level = "Beginner",
                ImageUrl = "/images/learning_Table_arial.jpg",
                GalleryImages = new List<string> { "/images/gringo_listening_to_herber.jpg" },
                Syllabus = new List<string> { "Module 1: Brewing Variables", "Module 2: The V60", "Module 3: French Press" },
                ProvidedEquipment = new List<string> { "Brewing Kit", "Kettle" },
                RequiredGear = new List<string> { "None" }
            },
            new TourItem
            {
                Id = 18,
                Title = "Advanced Extraction Science",
                ShortDescription = "Push the boundaries of extraction and yield.",
                LongDescription = "For professionals and serious enthusiasts. Utilize refractometry to measure TDS and extraction yield, map extraction curves, and manipulate water chemistry.",
                Price = 65,
                Duration = "90 Min",
                Location = "Cusco",
                Type = "Urban Lab",
                Level = "Advanced",
                ImageUrl = "/images/TdsChart_learning.jpg",
                GalleryImages = new List<string> { "/images/20260715_173507.jpg", "/images/20260715_173515.jpg", "/images/20260715_172723.jpg" },
                Syllabus = new List<string> { "Module 1: Water Chemistry", "Module 2: Refractometry", "Module 3: Yield Optimization" },
                ProvidedEquipment = new List<string> { "Refractometer", "Precision Grinder" },
                RequiredGear = new List<string> { "Calculator" }
            }
"""
    return prefix + new_tours + "\n        };"

cs_content = re.sub(r'(new TourItem\s*\{.*?\}\s*)\s*\}\s*;', add_mock_tours, cs_content, flags=re.DOTALL)

with open('Pages/Tour.cshtml.cs', 'w', encoding='utf-8') as f:
    f.write(cs_content)
print("Updated Tour.cshtml.cs")
