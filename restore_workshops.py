import sqlite3

conn = sqlite3.connect('coffee.db')
c = conn.cursor()

workshops = [
    (
        "Latte Art Masterclass", "Urban Workshops", "Cusco City Center", "August", "All Levels", 
        "Learn the fundamentals of milk texturing and latte art pouring from our award-winning baristas. Includes unlimited milk and coffee practice.",
        "https://images.unsplash.com/photo-1541167760496-1628856ab772?auto=format&fit=crop&w=600&q=80",
        45.00, None, "3 Hours"
    ),
    (
        "Advanced Sensory Evaluation", "Urban Workshops", "Cusco City Center", "September", "All Levels",
        "A deep dive into SCA cupping protocols, defect identification, and advanced palate development. Ideal for aspiring coffee professionals.",
        "https://images.unsplash.com/photo-1497935586351-b67a49e012bf?auto=format&fit=crop&w=600&q=80",
        75.00, None, "2 Hours"
    ),
    (
        "Home Brewing 101", "Urban Workshops", "Cusco City Center", "October", "All Levels",
        "Master the V60, AeroPress, and French Press. Understand extraction variables and how to troubleshoot your morning cup.",
        "https://images.unsplash.com/photo-1495474472207-464a4f54e156?auto=format&fit=crop&w=600&q=80",
        35.00, None, "2.5 Hours"
    ),
    (
        "Espresso Fundamentals", "Urban Workshops", "CUSCO", "SEP", "ADVANCED",
        "Detailed cupping sessions comparing rare lots. Discuss senses, share notes, and calibrate your palate alongside professionals.",
        "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?auto=format&fit=crop&w=1200&q=80",
        55.00, None, ""
    ),
    (
        "SCA Flavor Profiling", "Urban Workshops", "CUSCO", "SEP", "INTERMEDIATE",
        "Train your palate using industry-standard protocols. Blind-taste, score, and chart acidity, body, and tasting notes of exotic Peruvian varietals.",
        "https://images.unsplash.com/photo-1581007871115-f14bc016e0a4?auto=format&fit=crop&w=1200&q=80",
        75.00, None, ""
    ),
    (
        "Espresso Machines Lab", "Urban Workshops", "CUSCO", "SEP", "INTERMEDIATE",
        "Master the craft of precision extraction. Refine your volumetric parameters and sensory dynamics to build the perfect cup.",
        "/images/brewer_withGrinds.webp",
        65.00, None, ""
    ),
    (
        "Roasting Foundations", "Urban Workshops", "SPECIAL", "OCT", "ADVANCED",
        "Official SCA Sensory Skills Foundation certification. Includes rigorous blind triangulation and professional cupping protocol exams.",
        "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?auto=format&fit=crop&w=1200&q=80",
        250.00, "GUEST EVENT", ""
    )
]

for w in workshops:
    c.execute('''
        INSERT INTO Experiences (
            Title, Category, Location, Month, Difficulty, Description, ImageUrl, Price, Tag, Duration, LongDescription, 
            Syllabus, ProvidedEquipment, RequiredGear, GalleryImages
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, '', '[]', '[]', '[]', '[]')
    ''', w)

conn.commit()
conn.close()
