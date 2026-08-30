import re

with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# First we need to find Tour 6 block and replace it
# It looks something like:
#                 new TourItem
#                 {
#                     Id = 6,
#                     Title = "EXTRACTION SCIENCE",
#                     Subtitle = "...",
#                     ...
#                 },

def replace_tour_6(match):
    return """new TourItem
                {
                    Id = 6,
                    Title = "BREWING",
                    Subtitle = "Advanced Brewing Architecture",
                    Duration = "180 MIN",
                    Level = "ADVANCED",
                    Price = 65.00m,
                    Type = "Workshop",
                    Location = "Cusco Highlands",
                    ImageUrl = "/images/learning_Table_arial.jpg",
                    GalleryImages = new List<string>
                    {
                        "/images/TdsChart_learning.jpg",
                        "/images/gringo_listening_to_herber.jpg",
                        "/images/gring_playing_brewing.jpg",
                        "/images/20260715_173507.jpg",
                        "/images/20260715_173515.jpg",
                        "/images/20260715_172723.jpg"
                    },
                    Description = "Master the craft of precision extraction. Refine your volumetric parameters and sensory dynamics to build the perfect cup.",
                    LongDescription = "Explore the science behind the perfect brew. Master volumetric ratios, temperature variables, and extraction yields.",
                    Syllabus = new List<string> { "Extraction Theory", "Water Chemistry", "TDS & Yield Optimization" },
                    ProvidedEquipment = new List<string> { "Refractometer", "Various Brewers", "Precision Scales" },
                    RequiredGear = new List<string> { "Digital or Hardcopy Notebook" }
                }"""

# Using regex to find the block for ID 6
pattern = r'new TourItem\s*\{\s*Id = 6,.*?RequiredGear = new List<string> \{.*?\}\s*\}'
content = re.sub(pattern, replace_tour_6, content, flags=re.DOTALL)

with open('Pages/Tour.cshtml.cs', 'w', encoding='utf-8') as f:
    f.write(content)
print("Updated Tour.cshtml.cs")
