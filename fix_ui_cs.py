import re

with open('Pages/Tour.cshtml.cs', 'r', encoding='utf-8') as f:
    cs_content = f.read()

# Reverse ID 15 and 16 Images
def replace_cs_15(match):
    return match.group(1) + '/images/20260610_154127.jpg"'
pattern_cs_15 = r'(Id = 15,.*?ImageUrl = ")[^"]+"'
cs_content = re.sub(pattern_cs_15, replace_cs_15, cs_content, flags=re.DOTALL)

def replace_cs_16(match):
    return match.group(1) + '/images/20260129_111246.jpg"'
pattern_cs_16 = r'(Id = 16,.*?ImageUrl = ")[^"]+"'
cs_content = re.sub(pattern_cs_16, replace_cs_16, cs_content, flags=re.DOTALL)

# Let's manually replace ID 12's info using simple string replacement or split.
# Split at `Id = 12,` and then find the first `}` that closes the TourItem.
# Actually, since it has nested Lists with `{}`, we can just replace the specific fields.
start_idx = cs_content.find("Id = 12,")
if start_idx != -1:
    end_idx = cs_content.find("new TourItem", start_idx)
    if end_idx == -1: end_idx = len(cs_content)
    
    block = cs_content[start_idx:end_idx]
    
    block = re.sub(r'Title = "[^"]+"', 'Title = "Santa Teresa [Wilam\'s Farm]"', block)
    block = re.sub(r'ImageUrl = "[^"]+"', 'ImageUrl = "/images/gringo_happy_with_farmer.jpg"', block)
    
    gallery = """GalleryImages = new List<string>
                    {
                        "/images/20260730_140444(0).jpg",
                        "/images/20260730_150815.mp4",
                        "/images/20260730_150926.mp4",
                        "/images/20260730_162729.jpg",
                        "/images/20260730_162737.jpg",
                        "/images/20260730_162909.jpg",
                        "/images/20260730_162916.jpg",
                        "/images/gringo_happy_with_farmer.jpg"
                    }"""
    block = re.sub(r'GalleryImages = new List<string>\s*\{[^}]*\}', gallery, block, flags=re.DOTALL)
    
    cs_content = cs_content[:start_idx] + block + cs_content[end_idx:]

with open('Pages/Tour.cshtml.cs', 'w', encoding='utf-8') as f:
    f.write(cs_content)
print("Updated Tour.cshtml.cs")
