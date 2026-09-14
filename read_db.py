import sqlite3
import json

conn = sqlite3.connect('coffee.db')
cursor = conn.cursor()

cursor.execute("SELECT Id, Title, Syllabus FROM Experiences WHERE Syllabus LIKE '%Introduction Block%'")
rows = cursor.fetchall()
for row in rows:
    print(f"Id: {row[0]}, Title: {row[1]}")
    try:
        syl = json.loads(row[2])
        for idx, item in enumerate(syl):
            print(f"--- item {idx} ---")
            print(item[:100] + "...")
    except Exception as e:
        print("Error parsing JSON:", e)
