from pathlib import Path
import json
import shutil
import sqlite3

from PIL import Image, ImageOps

ROOT = Path(r"C:\Users\alext\source\repos\TheBestBean\TheBestBean")
SRC = Path(r"C:\Users\alext\Desktop\Best Bean")
OUT = ROOT / "wwwroot" / "Media" / "tours" / "odar"
OUT.mkdir(parents=True, exist_ok=True)

SHOTS = [
    (SRC / r"Pics\cupping\20260720_090632.jpg", "01-lab-cupping-table.jpg"),
    (SRC / r"Pics\cupping\20260720_090648.jpg", "02-odar-cupping-spoon.jpg"),
    (SRC / r"Pics\cupping\20260807_183714.jpg", "03-cupping-forms.jpg"),
    (SRC / r"Pics\cupping\20251113_165437.jpg", "04-guided-cupping.jpg"),
    (SRC / "SCAA_FlavorWheel.01.18.15.jpg", "05-flavor-wheel.jpg"),
]


def save_jpeg(src: Path, dest: Path):
    im = Image.open(src)
    im = ImageOps.exif_transpose(im)
    if im.mode not in ("RGB", "L"):
        im = im.convert("RGB")
    elif im.mode == "L":
        im = im.convert("RGB")
    im.save(dest, "JPEG", quality=88, optimize=True)
    print("wrote", dest.name, im.size)


for src, name in SHOTS:
    if not src.exists():
        raise SystemExit(f"missing {src}")
    save_jpeg(src, OUT / name)

gallery = [f"/Media/tours/odar/{name}" for _, name in SHOTS]
hero = gallery[0]

odar = {
    "Title": "Odar Lab [Sensory & Roast]",
    "TitleES": "Lab Odar [Sensorial y tueste]",
    "Category": "Urban Workshops",
    "Location": "Cusco",
    "Month": "Year-round",
    "Difficulty": "All Levels",
    "Duration": "2.5 HOURS",
    "Price": 50,
    "Tag": None,
    "ImageUrl": hero,
    "Description": "Hosted by Odar, a certified Q grader, at his factory and café two minutes apart in Cusco. Same craft as the main lab, with more time on sensory: Peruvian varieties, V60 at the factory, a guided SCA cupping, then roast-floor work — sorting greens, spotting quakers, and tasting acidity in the bean.",
    "DescriptionES": "Con Odar, Q grader certificado, en su fábrica y café a dos minutos en Cusco. Más tiempo en lo sensorial: variedades del Perú, V60 en fábrica, catación SCA guiada y trabajo en tueste.",
    "LongDescription": "Odar owns a factory and a café a two-minute walk apart in Cusco. He covers the same ground as the featured laboratory — brew, sensory, roast — with the emphasis on tasting. Ten minutes on varieties in Peru and Cusco and why altitude matters, then a V60 at the factory: how the cone works, ratio, and a cup. After that, the flavor wheel, what the SCA is, how a cupping form is scored, and a guided cupping with Odar. Close on the roast floor: sort green beans before they go in, pick quakers and defects after, and bite a bean to feel acidity.",
    "LongDescriptionES": "Odar tiene fábrica y café a dos minutos en Cusco. Diez minutos de variedades y altitud, V60 en la fábrica, rueda de sabores y catación guiada, y al final clasificación de verde, quakers y morder el grano para la acidez.",
    "Syllabus": [
        "Intro & V60: Peru and Cusco varieties, high altitude (10 min), then how the cone works, ratio, and a brew at the factory",
        "Sensory: flavor wheel, the SCA, cupping forms, then a guided cupping with Odar (Q grader)",
        "Roast: sort greens with the roast master, pick quakers and defects, bite the bean for acidity",
    ],
    "ProvidedEquipment": [
        "V60 brew at the factory",
        "Guided SCA cupping with a Q grader",
        "Green-bean sorting and roast-floor tasting",
        "Apron and cupping spoons",
    ],
    "RequiredGear": ["Closed-toe shoes"],
    "GalleryImages": gallery,
}

cinthya = {
    "Title": "Cinthya Lab [V60, Espresso & Cupping]",
    "TitleES": "Lab Cinthya [V60, espresso y catación]",
    "Category": "Urban Workshops",
    "Location": "Cusco",
    "Month": "Year-round",
    "Difficulty": "All Levels",
    "Duration": "2.5 HOURS",
    "Price": 50,
    "Tag": None,
    "ImageUrl": "",
    "Description": "Hosted by Cinthya, a Q grader, competition judge, and roaster. Her roaster sits away from the tasting room, so this session stays on the bar: V60 pour-over, espresso, and a guided cupping. A strong option when the main laboratory is full.",
    "DescriptionES": "Con Cinthya, Q grader, jueza y tostadora. El tostador queda lejos del área de cata, así que la sesión es V60, espresso y catación guiada. Alternativa cuando el laboratorio principal está lleno.",
    "LongDescription": "Cinthya is a roaster, a Q grader, and a judge. Her roaster is not next to the presentation space, so this tour does not walk the roast floor. You stay with brew and sensory: V60 pour-over, espresso on the machine, and a cupping she leads. Photos of her space will go up when we have them.",
    "LongDescriptionES": "Cinthya es tostadora, Q grader y jueza. El tostador no está junto al espacio de presentación, así que el recorrido es V60, espresso y catación. Subiremos fotos de su local cuando las tengamos.",
    "Syllabus": [
        "V60 pour-over: grind, ratio, and a cup you brew",
        "Espresso: extraction, tasting, and how the bar works",
        "Guided cupping: SCA palate work with a Q grader and judge",
    ],
    "ProvidedEquipment": [
        "V60 pour-over",
        "Espresso tasting",
        "Guided cupping",
    ],
    "RequiredGear": [],
    "GalleryImages": [],
}


def upsert(cur, row, match_title_parts):
    like = " OR ".join(["Title LIKE ?" for _ in match_title_parts])
    args = [f"%{p}%" for p in match_title_parts]
    existing = cur.execute(f"SELECT Id FROM Experiences WHERE {like}", args).fetchone()
    payload = dict(row)
    payload["Syllabus"] = json.dumps(row["Syllabus"])
    payload["ProvidedEquipment"] = json.dumps(row["ProvidedEquipment"])
    payload["RequiredGear"] = json.dumps(row["RequiredGear"])
    payload["GalleryImages"] = json.dumps(row["GalleryImages"])
    cols = [
        "Title", "TitleES", "Category", "Location", "Month", "Difficulty", "Duration",
        "Price", "Tag", "ImageUrl", "Description", "DescriptionES", "LongDescription",
        "LongDescriptionES", "Syllabus", "ProvidedEquipment", "RequiredGear", "GalleryImages",
    ]
    if existing:
        sets = ", ".join([f"{c}=?" for c in cols])
        cur.execute(
            f"UPDATE Experiences SET {sets} WHERE Id=?",
            [payload[c] for c in cols] + [existing[0]],
        )
        print("updated", payload["Title"], "id", existing[0])
        return existing[0]
    q = f"INSERT INTO Experiences ({', '.join(cols)}) VALUES ({', '.join(['?']*len(cols))})"
    cur.execute(q, [payload[c] for c in cols])
    print("inserted", payload["Title"], "id", cur.lastrowid)
    return cur.lastrowid


db = ROOT / "coffee.db"
con = sqlite3.connect(db)
cur = con.cursor()
upsert(cur, odar, ["Odar"])
upsert(cur, cinthya, ["Cinthya"])

keep = (
    "The Cusco Coffee Laboratory",
    "Odar Lab [Sensory & Roast]",
    "Cinthya Lab [V60, Espresso & Cupping]",
)
cur.execute(
    """
    UPDATE Experiences
    SET Category = 'Archived'
    WHERE Category IN ('Urban Workshops', 'Urban Labs')
      AND Title NOT IN (?, ?, ?)
    """,
    keep,
)
print("archived", cur.rowcount, "old workshops")
cur.execute("UPDATE SiteContent SET Value = 'CUSCO' WHERE Key = 'Experiences_UrbanLabs_Subtitle'")
if cur.rowcount == 0:
    cur.execute(
        "INSERT INTO SiteContent (Key, Value, Page) VALUES ('Experiences_UrbanLabs_Subtitle', 'CUSCO', 'Experiences')"
    )
print("subtitle", cur.execute("SELECT Value FROM SiteContent WHERE Key = 'Experiences_UrbanLabs_Subtitle'").fetchone())
con.commit()
con.close()
print("db ok")
