"""Turn the official 2023 Peru limit shapefiles into talk vectors.

Source (GEOSDOT / geoportal, EPSG:4326), downloaded as SHAPE-ZIP:

  v_departamentos_2023
  v_provincias_2023
  v_distritos_2023

The lab talk draws these as SVG. Tolerances stay under a pixel at the
zoom each slide uses, so the coast and district lines stay the official
shape instead of a simplified sketch.
"""

from __future__ import annotations

import json
import shutil
import unicodedata
from pathlib import Path

import shapefile
from shapely.geometry import mapping, shape
from shapely.validation import make_valid

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "wwwroot" / "data" / "talk"
CACHE = ROOT / "data" / "boundaries" / "official-2023"
DOWNLOADS = Path(r"C:\Users\alext\Downloads")

COFFEE_PROVINCES = {
    "jaen",
    "san ignacio",
    "chachapoyas",
    "rodriguez de mendoza",
    "utcubamba",
    "chanchamayo",
    "satipo",
    "oxapampa",
    "leoncio prado",
    "la convencion",
    "sandia",
}

GEMS = {
    "san ignacio",
    "chirinos",
    "tabaconas",
    "namballe",
    "huarango",
    "la coipa",
    "san jose de lourdes",
    "lonya grande",
    "omia",
    "cochamal",
    "huambo",
    "mariscal benavides",
    "villa rica",
    "oxapampa",
    "chontabamba",
    "perene",
    "pichanaqui",
    "san ramon",
    "san luis de shuaro",
    "satipo",
    "pangoa",
    "mazamari",
    "jose crespo y castillo",
    "mariano damaso beraun",
    "daniel alomia robles",
    "hermilio valdizan",
    "inkawasi",
    "santa teresa",
    "echarate",
    "huayopata",
    "villa virgen",
    "san juan del oro",
    "san pedro de putina punco",
    "alto inambari",
    "yanahuaya",
}
HUBS = {"jaen", "rupa-rupa", "santa ana"}

ACCENTS = {
    "cusco": "Cusco",
    "huanuco": "Huánuco",
    "junin": "Junín",
    "san martin": "San Martín",
    "apurimac": "Apurímac",
    "ancash": "Áncash",
    "huancavelica": "Huancavelica",
    "jaen": "Jaén",
    "rodriguez de mendoza": "Rodríguez de Mendoza",
    "la convencion": "La Convención",
    "jose crespo y castillo": "José Crespo y Castillo",
    "mariano damaso beraun": "Mariano Dámaso Beraún",
    "daniel alomia robles": "Daniel Alomía Robles",
    "hermilio valdizan": "Hermilio Valdizán",
    "san jose de lourdes": "San José de Lourdes",
    "san jose del alto": "San José del Alto",
    "perene": "Perené",
    "pichanaqui": "Pichanaqui",
    "san ramon": "San Ramón",
    "san luis de shuaro": "San Luis de Shuaro",
    "omia": "Omia",
}


def fold(value: str) -> str:
    text = unicodedata.normalize("NFD", value or "")
    return "".join(ch for ch in text if unicodedata.category(ch) != "Mn").lower().strip()


def title_place(raw: str) -> str:
    folded = fold(raw)
    if folded in ACCENTS:
        return ACCENTS[folded]
    small = {"de", "del", "la", "las", "los", "y"}
    parts = []
    for i, word in enumerate(raw.split()):
        lower = word.lower()
        parts.append(lower if i and lower in small else lower.capitalize())
    return " ".join(parts)


def cache_shapefiles() -> dict[str, Path]:
    CACHE.mkdir(parents=True, exist_ok=True)
    found = {}
    for stem in ("v_departamentos_2023", "v_provincias_2023", "v_distritos_2023"):
        src_dir = next(DOWNLOADS.glob(f"{stem}_*"), None)
        dest = CACHE / f"{stem}.shp"
        if src_dir and (src_dir / f"{stem}.shp").exists():
            for ext in (".shp", ".shx", ".dbf", ".prj"):
                shutil.copy2(src_dir / f"{stem}{ext}", CACHE / f"{stem}{ext}")
        if not dest.exists():
            raise SystemExit(f"missing shapefile {dest}")
        found[stem] = dest
    return found


def clean(geom):
    polygon = make_valid(shape(geom))
    if polygon.is_empty:
        return None
    if polygon.geom_type == "GeometryCollection":
        polygon = unary_union_polygons(polygon)
    return polygon


def unary_union_polygons(collection):
    from shapely.ops import unary_union

    parts = [g for g in collection.geoms if g.geom_type in ("Polygon", "MultiPolygon")]
    return unary_union(parts) if parts else None


def feature(geom, props: dict) -> dict:
    return {"type": "Feature", "properties": props, "geometry": mapping(geom)}


def write(path: Path, features: list) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(
        json.dumps({"type": "FeatureCollection", "features": features}, separators=(",", ":")),
        encoding="utf-8",
    )
    print(f"{path.name}: {len(features)} features, {path.stat().st_size / 1e6:.2f} MB")


def records(path: Path):
    reader = shapefile.Reader(str(path), encoding="latin-1")
    for item in reader.iterShapeRecords():
        yield {key: value for key, value in item.record.as_dict().items()}, item.shape.__geo_interface__


def main() -> None:
    paths = cache_shapefiles()

    departments = []
    for rec, geom in records(paths["v_departamentos_2023"]):
        polygon = clean(geom)
        if polygon is None:
            continue
        simplified = polygon.simplify(0.0012, preserve_topology=True)
        departments.append(feature(simplified, {"name": title_place(rec["nombdep"]), "level": "department"}))

    provinces = []
    for rec, geom in records(paths["v_provincias_2023"]):
        polygon = clean(geom)
        if polygon is None:
            continue
        name = title_place(rec["nombprov"])
        simplified = polygon.simplify(0.0016, preserve_topology=True)
        provinces.append(
            feature(
                simplified,
                {
                    "name": name,
                    "department": title_place(rec["nombdep"]),
                    "level": "province",
                    "coffee": fold(name) in COFFEE_PROVINCES,
                },
            )
        )

    seen_gems = set()
    districts = []
    for rec, geom in records(paths["v_distritos_2023"]):
        province_key = fold(rec["nombprov"])
        if province_key not in COFFEE_PROVINCES:
            continue
        polygon = clean(geom)
        if polygon is None:
            continue
        district_key = fold(rec["nombdist"])
        role = "district"
        if district_key in HUBS and province_key in {"jaen", "leoncio prado", "la convencion", "utcubamba"}:
            role = "hub"
        elif district_key in GEMS:
            role = "gem"
            seen_gems.add(district_key)
        label = title_place(rec["nombdist"])
        if district_key == "santa ana" and province_key == "la convencion":
            label = "Quillabamba"
        simplified = polygon.simplify(0.00028, preserve_topology=True)
        districts.append(
            feature(
                simplified,
                {
                    "name": title_place(rec["nombdist"]),
                    "label": label,
                    "province": title_place(rec["nombprov"]),
                    "department": title_place(rec["nombdep"]),
                    "region": (rec.get("region_nat") or "").title(),
                    "level": "district",
                    "role": role,
                },
            )
        )

    missing = GEMS - seen_gems
    if missing:
        print("gems not found:", ", ".join(sorted(missing)))

    write(OUT / "peru-departments.geojson", departments)
    write(OUT / "peru-provinces.geojson", provinces)
    write(OUT / "peru-districts.geojson", districts)
    print("districts", len(districts), "gems", len(seen_gems))


if __name__ == "__main__":
    main()
