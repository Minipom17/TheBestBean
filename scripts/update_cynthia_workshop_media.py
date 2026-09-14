#!/usr/bin/env python3
"""Sync DB paths for Cynthia 1-hour workshops (WebP + cupping hero). Run from repo root."""

import json
import sqlite3
from pathlib import Path

DB = Path("coffee.db")
V = "?v=2"

cupping_gallery = [
    f"/Media/experiences/tour-26/cynthia-cupping-0{i}.webp{V}"
    for i in range(1, 7)
]
cupping_card = cupping_gallery[0]
cupping_hero = cupping_gallery[5]

brew_gallery = [
    f"/Media/experiences/tour-25/cynthia-brew-01.webp{V}",
    f"/Media/experiences/tour-25/cynthia-brew-02.webp{V}",
    f"/Media/experiences/tour-25/cynthia-brew-03.webp{V}",
]
brew_card = brew_gallery[0]

site_rows = [
    ("Tour_Hero_Image_26", cupping_hero, "Tour"),
    ("Tour_Hero_Pos_26", "50% 40%", "Tour"),
]


def upsert_site(cur, key: str, value: str, page: str) -> None:
    cur.execute("SELECT Key FROM SiteContent WHERE Key = ?", (key,))
    if cur.fetchone():
        cur.execute("UPDATE SiteContent SET Value = ?, Page = ? WHERE Key = ?", (value, page, key))
    else:
        cur.execute(
            "INSERT INTO SiteContent (Key, Value, Page) VALUES (?, ?, ?)",
            (key, value, page),
        )


def main() -> None:
    conn = sqlite3.connect(DB)
    cur = conn.cursor()

    cur.execute(
        "UPDATE Experiences SET ImageUrl = ?, GalleryImages = ? WHERE Title = ?",
        (cupping_card, json.dumps(cupping_gallery), "Introduction to Cupping"),
    )
    print("Introduction to Cupping:", cur.rowcount)

    cur.execute(
        "UPDATE Experiences SET ImageUrl = ?, GalleryImages = ? WHERE Title = ?",
        (brew_card, json.dumps(brew_gallery), "Brew Your Own"),
    )
    print("Brew Your Own:", cur.rowcount)

    for key, value, page in site_rows:
        upsert_site(cur, key, value, page)
    print("SiteContent hero keys updated")

    conn.commit()
    conn.close()


if __name__ == "__main__":
    main()
