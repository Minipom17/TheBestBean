import sqlite3
import shutil
import os
import json

# Setup image directories
os.makedirs('wwwroot/images/experiences', exist_ok=True)
img1 = r'C:\Users\alext\.gemini\antigravity-ide\brain\6b71ec5a-67c9-4f72-9b17-67fb833b6993\cusco_workshop_1_1787411048773.jpg'
img2 = r'C:\Users\alext\.gemini\antigravity-ide\brain\6b71ec5a-67c9-4f72-9b17-67fb833b6993\cusco_workshop_2_1787411065906.jpg'
img3 = r'C:\Users\alext\.gemini\antigravity-ide\brain\6b71ec5a-67c9-4f72-9b17-67fb833b6993\cusco_workshop_3_1787411079187.jpg'

shutil.copy(img1, 'wwwroot/images/experiences/cusco_workshop_1.jpg')
shutil.copy(img2, 'wwwroot/images/experiences/cusco_workshop_2.jpg')
shutil.copy(img3, 'wwwroot/images/experiences/cusco_workshop_3.jpg')

conn = sqlite3.connect('coffee.db')
c = conn.cursor()

# 1. Delete old Urban Workshops
c.execute("DELETE FROM Experiences WHERE Category = 'Urban Workshops'")

# 2. Insert the new Cusco workshop
title = "The Cusco Coffee Laboratory"
category = "Urban Workshops"
location = "Cusco"
month = "Weekly"
difficulty = "All Levels"
description = "Join us in our state-of-the-art laboratory in the heart of Cusco. This comprehensive 2.5-hour workshop covers everything from bean evaluation and sensory development to advanced extraction techniques. Perfect for coffee enthusiasts looking to deepen their understanding of the craft."
image_url = "/images/experiences/cusco_workshop_1.jpg"
price = 45.00
tag = "FEATURED"
duration = "2.5 HOURS"
long_description = "Our Cusco Coffee Laboratory workshop is designed to elevate your coffee journey. We begin with a theoretical foundation in coffee agronomy and processing, followed by an intensive sensory session where you will learn to identify key flavor notes and defects. Finally, you will get hands-on experience dialing in espresso and perfecting pour-over recipes under the guidance of our master baristas."
syllabus_json = json.dumps([
    "Sensory Evaluation: Train your palate to identify the subtle flavor notes inherent in Peruvian specialty coffee.",
    "The Science of Extraction: Understand the variables that affect extraction, from grind size to water temperature.",
    "Hands-on Brewing: Practice dialing in espresso and perfecting your pour-over technique."
])
provided_equipment_json = json.dumps(["Cupping Spoons", "Brewing Equipment", "Aprons", "Tasting Notebook"])
required_gear_json = json.dumps(["Comfortable clothing"])
gallery_images_json = json.dumps([
    "/images/experiences/cusco_workshop_1.jpg",
    "/images/experiences/cusco_workshop_2.jpg",
    "/images/experiences/cusco_workshop_3.jpg"
])

c.execute('''
    INSERT INTO Experiences (
        Title, Category, Location, Month, Difficulty, Description, ImageUrl, Price, Tag, Duration, LongDescription, 
        Syllabus, ProvidedEquipment, RequiredGear, GalleryImages
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
''', (
    title, category, location, month, difficulty, description, image_url, price, tag, duration, long_description,
    syllabus_json, provided_equipment_json, required_gear_json, gallery_images_json
))

conn.commit()
conn.close()
print("Database updated successfully.")
