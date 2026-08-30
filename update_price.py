import sqlite3

conn = sqlite3.connect('coffee.db')
c = conn.cursor()
c.execute("UPDATE Experiences SET Price = 50 WHERE Title = 'The Cusco Coffee Laboratory'")
conn.commit()
conn.close()
print("Done")
