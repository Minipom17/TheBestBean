import json
import sqlite3
from pathlib import Path

DB = Path("coffee.db")

description = (
    "One hour with Cynthia and Pavel. She leads the sensory side (cupping, roast); "
    "he brews V60. Sit, taste, and learn how variety and process — washed, natural, honey — "
    "change the cup. All levels; coffee is made for you."
)
description_es = (
    "Una hora con Cynthia y Pavel. Ella lleva lo sensorial (catación, tueste); él prepara V60. "
    "Pruebas y aprendes cómo la variedad y el proceso — lavado, natural, honey — cambian la taza. "
    "Todos los niveles; el café se hace para ti."
)
long_description = (
    "Hosted by Cynthia, roastmaster and cupping specialist, and Pavel, V60 pour-over. You sit; they brew. "
    "Cups from around Peru — different varieties and processes — land in front of you while you talk altitude, "
    "washed vs natural vs honey, and what those words actually taste like. First specialty cup or already deep "
    "in coffee: the conversation meets you there. You leave able to read a bag and know what you like."
)
long_description_es = (
    "Con Cynthia, tostadora y especialista en catación, y Pavel, maestro de V60. Tú te sientas; ellos preparan. "
    "Tazas de distintas regiones, variedades y procesos: lavado, natural, honey. Principiante o avanzado: "
    "la conversación se adapta. Sales sabiendo leer un empaque y reconocer lo que te gusta."
)
syllabus = [
    'Welcome: Cynthia & Pavel — Peru is not one coffee (10 min)||<ul class="tour-syllabus-points"><li>Cynthia (cupping and roast) and Pavel (V60) open the hour together.</li><li>Peru runs from desert coast to Amazon jungle to peaks above 2,000m — so variety, altitude, and process actually show up in the cup.</li><li>We set that frame before the first sip, whether this is your first specialty coffee or you already know the map.</li></ul>||Bourbon, Geisha, Typica, Altitude, Terroir',
    'Tasting flight: process & variety side by side (40 min)||<ul class="tour-syllabus-points"><li>Pavel brews; you stay seated.</li><li>Cups from different regions, varieties, and processes land in front of you.</li><li>You learn what makes washed coffee distinct from natural and honey — acidity, fruit, cleanliness, funk — and how long versus short fermentation changes flavor.</li><li>Fragrance on the dry grounds, then aroma once the coffee is wet. Taste side by side: sweetness, acidity, body.</li></ul>||Washed, Natural, Honey, Long fermentation, Short fermentation, Fragrance, Aroma, V60',
    'Close: your palate, the next bag (10 min)||<ul class="tour-syllabus-points"><li>Pick the cup you liked most.</li><li>Together we name why — sweetness, acidity, body — so you leave knowing what your palate leans toward.</li><li>Next time you pick up a coffee bag, variety and process on the label will point you at coffee you actually like.</li></ul>||Palate, Flavor notes, Reading a bag',
]
equipment = [
    "Coffees from around Peru, brewed for you — Cynthia and Pavel at the table",
    "Guided tasting: cupping language with a Q grader, V60 with Pavel",
    "Water, cups, and a quiet seat — no gear required",
]

conn = sqlite3.connect(DB)
cur = conn.cursor()
cur.execute(
    """
    UPDATE Experiences
    SET Description = ?, DescriptionES = ?, LongDescription = ?, LongDescriptionES = ?,
        Syllabus = ?, ProvidedEquipment = ?, Difficulty = ?
    WHERE Title = ?
    """,
    (
        description,
        description_es,
        long_description,
        long_description_es,
        json.dumps(syllabus),
        json.dumps(equipment),
        "All Levels",
        "Peru Tasting Hour",
    ),
)
print("rows updated:", cur.rowcount)
conn.commit()
conn.close()
