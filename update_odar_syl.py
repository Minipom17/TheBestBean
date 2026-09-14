import sqlite3
import json

new_syl = [
    "Intro & V60|30 min||00:00 - 00:10 | Peru and Cusco varieties, high altitude.<br><br>00:10 - 00:30 | How the cone works, ratio, and a brew at the factory.",
    "Sensory|45 min||00:30 - 00:45 | Flavor wheel, the SCA, cupping forms.<br><br>00:45 - 01:15 | Guided cupping with Odar (Q grader).",
    "Roast|45 min||01:15 - 02:00 | Sort greens with the roast master, pick quakers and defects, bite the bean for acidity."
]

conn = sqlite3.connect('coffee.db')
cursor = conn.cursor()
cursor.execute("UPDATE Experiences SET Syllabus = ? WHERE Title LIKE 'Odar Lab%'", (json.dumps(new_syl),))
conn.commit()
conn.close()
print("Updated Odar Lab Syllabus in database.")
