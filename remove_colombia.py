import os
import sqlite3
import re

files_to_check = ['Models/SampleDataSeeder.cs', 'wwwroot/js/coffee-map.js']
for f in files_to_check:
    if os.path.exists(f):
        with open(f, 'r', encoding='utf-8') as file:
            content = file.read()
        
        # Simple replacements
        content = content.replace('Colombia', 'Peru')
        content = content.replace('colombia', 'peru')
        
        with open(f, 'w', encoding='utf-8') as file:
            file.write(content)

# Update database
db_file = 'app.db'
if os.path.exists(db_file):
    conn = sqlite3.connect(db_file)
    c = conn.cursor()
    c.execute("SELECT name FROM sqlite_master WHERE type='table';")
    tables = c.fetchall()
    for table in tables:
        t_name = table[0]
        c.execute(f"PRAGMA table_info({t_name})")
        columns = c.fetchall()
        for col in columns:
            if col[2] == 'TEXT':
                try:
                    c.execute(f"UPDATE {t_name} SET {col[1]} = REPLACE({col[1]}, 'Colombia', 'Peru') WHERE {col[1]} LIKE '%Colombia%'")
                    c.execute(f"UPDATE {t_name} SET {col[1]} = REPLACE({col[1]}, 'Colombian', 'Peruvian') WHERE {col[1]} LIKE '%Colombian%'")
                except:
                    pass
    conn.commit()
    conn.close()

db_file2 = 'coffee.db'
if os.path.exists(db_file2):
    conn = sqlite3.connect(db_file2)
    c = conn.cursor()
    c.execute("SELECT name FROM sqlite_master WHERE type='table';")
    tables = c.fetchall()
    for table in tables:
        t_name = table[0]
        c.execute(f"PRAGMA table_info({t_name})")
        columns = c.fetchall()
        for col in columns:
            if col[2] == 'TEXT':
                try:
                    c.execute(f"UPDATE {t_name} SET {col[1]} = REPLACE({col[1]}, 'Colombia', 'Peru') WHERE {col[1]} LIKE '%Colombia%'")
                    c.execute(f"UPDATE {t_name} SET {col[1]} = REPLACE({col[1]}, 'Colombian', 'Peruvian') WHERE {col[1]} LIKE '%Colombian%'")
                except:
                    pass
    conn.commit()
    conn.close()

print('Done')
