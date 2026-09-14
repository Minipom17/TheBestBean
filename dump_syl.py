import sqlite3
import json

conn = sqlite3.connect('coffee.db')
cursor = conn.cursor()

cursor.execute("SELECT Syllabus FROM Experiences WHERE Id=14")
row = cursor.fetchone()
if row:
    syl = json.loads(row[0])
    with open('syl.json', 'w') as f:
        json.dump(syl, f, indent=2)
    print("Wrote to syl.json")
