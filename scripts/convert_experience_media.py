#!/usr/bin/env python3
"""Convert experience photos under wwwroot/Media/experiences to WebP.

Usage:
  python scripts/convert_experience_media.py tour-26
  python scripts/convert_experience_media.py tour-25 tour-26
  python scripts/convert_experience_media.py --all

Requires: pip install Pillow
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

try:
    from PIL import Image
except ImportError:
    print("Install Pillow: pip install Pillow", file=sys.stderr)
    sys.exit(1)

ROOT = Path(__file__).resolve().parents[1]
MEDIA = ROOT / "wwwroot" / "Media" / "experiences"
QUALITY = 82
MAX_WIDTH = 2400


def convert_file(src: Path, remove_source: bool = True) -> Path:
    dst = src.with_suffix(".webp")
    with Image.open(src) as im:
        im = im.convert("RGB")
        w, h = im.size
        if w > MAX_WIDTH:
            nh = int(h * (MAX_WIDTH / w))
            im = im.resize((MAX_WIDTH, nh), Image.Resampling.LANCZOS)
        im.save(dst, "WEBP", quality=QUALITY, method=6)
    if remove_source and src.suffix.lower() in {".jpg", ".jpeg", ".png"}:
        src.unlink(missing_ok=True)
    print(f"  {src.name} -> {dst.name} ({dst.stat().st_size // 1024} KB)")
    return dst


def convert_dir(folder: Path) -> int:
    if not folder.is_dir():
        print(f"skip (missing): {folder}")
        return 0
    count = 0
    for src in sorted(folder.iterdir()):
        if src.suffix.lower() not in {".jpg", ".jpeg", ".png"}:
            continue
        convert_file(src)
        count += 1
    return count


def main() -> None:
    parser = argparse.ArgumentParser(description="Convert experience JPEG/PNG to WebP")
    parser.add_argument("folders", nargs="*", help="e.g. tour-25 tour-26")
    parser.add_argument("--all", action="store_true", help="Every folder under Media/experiences")
    args = parser.parse_args()

    targets: list[Path] = []
    if args.all:
        targets = [p for p in MEDIA.iterdir() if p.is_dir()]
    else:
        for name in args.folders:
            targets.append(MEDIA / name)

    if not targets:
        parser.print_help()
        sys.exit(1)

    total = 0
    for folder in targets:
        print(folder.relative_to(ROOT))
        total += convert_dir(folder)
    print(f"Converted {total} file(s).")


if __name__ == "__main__":
    main()
