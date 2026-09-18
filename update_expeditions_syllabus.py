import sqlite3, json

conn = sqlite3.connect("coffee.db")
c = conn.cursor()

santa_teresa_syllabus = [
  "Pickup: Santa Teresa or Cusco | 10:00 AM || We'll arrange a pickup from your hotel and drive along the scenic Hidroeléctrica corridor.",
  "Farm Walk: Cloud forest ecosystem | 11:30 AM || We will explore the farm's unique microclimate, observe the shade canopy, and see the coffee cherries on the trees.",
  "Processing: Harvest and washing | 1:00 PM || Get hands-on experience picking ripe cherries and watch the family process them using traditional washed methods.",
  "Cupping: Taste the harvest | 3:00 PM || Walk among the parchment drying beds and then sit down for a fresh cup of coffee right at the source.",
  "Departure: Onward journey | 4:30 PM || We will drop you off so you can continue your trip toward Machu Picchu or head back to Cusco."
]

la_catarata_syllabus = [
  "Arrival: Jaén | Morning || Arrive in Jaén either by a short flight from Lima or an overnight bus journey.",
  "Farm Walk: Meet Ángel | 10:00 AM || Arrive at La Catarata, take a picture by the farm sign, and enjoy a guided plot walk with producer Ángel.",
  "Agronomy: Ripeness and Brix | 11:30 AM || Learn about selective picking by measuring the Brix (sugar content) of the cherries directly on the tree.",
  "Wet Mill: Processing techniques | 1:30 PM || Follow the cherry's journey through the mill: measuring pH, pulping, fermentation, and washing.",
  "Cupping: Cup of Excellence Geisha | 3:30 PM || Observe the parchment drying process and finish with an exclusive cupping of their award-winning Geisha lot."
]

c.execute("UPDATE Experiences SET Syllabus = ? WHERE Title LIKE '%Santa Teresa%'", (json.dumps(santa_teresa_syllabus),))
c.execute("UPDATE Experiences SET Syllabus = ? WHERE Title LIKE '%La Catarata%'", (json.dumps(la_catarata_syllabus),))

conn.commit()
conn.close()

print("Updated syllabus in DB successfully.")
