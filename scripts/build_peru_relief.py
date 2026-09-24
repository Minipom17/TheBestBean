"""Shaded relief of Peru from AWS terrarium elevation tiles, clipped to the 2023 departments."""
import json
import math
import sys
import urllib.request
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "wwwroot" / "brand" / "peru-relief.png"
GEO = ROOT / "wwwroot" / "data" / "talk" / "peru-departments.geojson"
Z = 7
X0, X1 = 35, 39
Y0, Y1 = 64, 70
TILE = 256


def slippy(lon, lat):
    n = 2 ** Z
    x = (lon + 180.0) / 360.0 * n
    lat_r = math.radians(lat)
    y = (1.0 - math.log(math.tan(lat_r) + 1.0 / math.cos(lat_r)) / math.pi) / 2.0 * n
    return x, y


def download():
    folder = ROOT / "data" / "boundaries" / "terrarium-z7"
    folder.mkdir(parents=True, exist_ok=True)
    tiles = []
    for y in range(Y0, Y1 + 1):
        row = []
        for x in range(X0, X1 + 1):
            path = folder / f"{x}_{y}.png"
            if not path.exists() or path.stat().st_size < 1000:
                url = f"https://s3.amazonaws.com/elevation-tiles-prod/terrarium/{Z}/{x}/{y}.png"
                urllib.request.urlretrieve(url, path)
                print("got", x, y)
            row.append(path)
        tiles.append(row)
    return tiles


def mosaic(tiles):
    rows = []
    for row in tiles:
        strips = []
        for path in row:
            im = np.asarray(Image.open(path).convert("RGB"), dtype=np.float32)
            elev = im[:, :, 0] * 256.0 + im[:, :, 1] + im[:, :, 2] / 256.0 - 32768.0
            strips.append(elev)
        rows.append(np.concatenate(strips, axis=1))
    return np.concatenate(rows, axis=0)


def colorize(elev):
    stops = np.array([0, 250, 900, 1800, 3200, 5200], dtype=np.float32)
    colors = np.array([
        [244, 246, 248],
        [186, 206, 224],
        [112, 146, 186],
        [92, 96, 168],
        [62, 42, 112],
        [36, 22, 64],
    ], dtype=np.float32)
    h, w = elev.shape
    flat = np.clip(elev, stops[0], stops[-1]).ravel()
    idx = np.searchsorted(stops, flat, side="right") - 1
    idx = np.clip(idx, 0, len(stops) - 2)
    span = stops[idx + 1] - stops[idx]
    t = ((flat - stops[idx]) / span)[:, None]
    rgb = colors[idx] * (1 - t) + colors[idx + 1] * t
    return rgb.reshape(h, w, 3)


def hillshade(elev):
    mpp = (360.0 / (2 ** Z) / TILE) * 111320.0 * math.cos(math.radians(-9))
    ve = 7.0
    dy, dx = np.gradient(elev)
    dx = dx / mpp * ve
    dy = dy / mpp * ve
    slope = np.arctan(np.hypot(dx, dy))
    aspect = np.arctan2(-dx, dy)
    zenith = math.radians(48)
    azimuth = math.radians(315)
    shade = (
        math.cos(zenith) * np.cos(slope)
        + math.sin(zenith) * np.sin(slope) * np.cos(azimuth - aspect)
    )
    shade = np.clip(shade, 0, 1)
    return 0.42 + 0.72 * shade


def exteriors(geom):
    gtype = geom["type"]
    coords = geom["coordinates"]
    if gtype == "Polygon":
        yield coords[0]
    elif gtype == "MultiPolygon":
        for poly in coords:
            yield poly[0]


def mask(shape):
    h, w = shape
    im = Image.new("L", (w, h), 0)
    draw = ImageDraw.Draw(im)
    data = json.loads(GEO.read_text(encoding="utf-8"))
    for feature in data["features"]:
        for ring in exteriors(feature["geometry"]):
            pts = []
            for lon, lat in ring:
                xf, yf = slippy(lon, lat)
                pts.append(((xf - X0) * TILE, (yf - Y0) * TILE))
            if len(pts) > 2:
                draw.polygon(pts, fill=255)
    return np.asarray(im)


def sample(elev, land, lon, lat):
    xf, yf = slippy(lon, lat)
    px = (xf - X0) * TILE
    py = (yf - Y0) * TILE
    ix, iy = int(px), int(py)
    if iy < 0 or ix < 0 or iy >= elev.shape[0] or ix >= elev.shape[1]:
        return 0.0
    if land[iy, ix] < 128:
        return 0.0
    return max(0.0, float(elev[iy, ix]))


def land_span(vals):
    idx = np.where(vals > 40)[0]
    if len(idx) < 12:
        return vals
    a = max(0, int(idx[0]) - 1)
    b = min(len(vals), int(idx[-1]) + 2)
    return vals[a:b]


def write_cuts(elev, land):
    cuts = [
        ("North", -5.5, "#A3B396"),
        ("Huánuco", -9.35, "#2B2B2B"),
        ("Junín", -11.45, "#8D8D8D"),
        ("Cusco", -13.05, "#E15A3A"),
    ]
    lons = np.linspace(-81.2, -68.8, 280)
    series = []
    for name, lat, color in cuts:
        vals = np.array([sample(elev, land, lon, lat) for lon in lons], dtype=np.float32)
        span = land_span(vals)
        kernel = np.array([1, 2, 3, 2, 1], dtype=np.float32)
        kernel /= kernel.sum()
        smooth = np.convolve(span, kernel, mode="same")
        # Stretch every cut across the same width. West is left, Amazon is right.
        x = np.linspace(0, 1, 160)
        src = np.linspace(0, 1, len(smooth))
        stretched = np.interp(x, src, smooth)
        series.append((name, color, stretched))
        print(name, "max", int(stretched.max()), "samples", len(span))

    left, right = 168, 940
    # Shared scale so a higher ridge is actually higher, offset so the ranges stack.
    # Tallest sample is about 4,800 m. Keep that peak well inside the viewBox.
    px_per_m = 0.032
    base = [210, 300, 390, 478]

    def xy(i, meters, b, n):
        x = left + (right - left) * (i / (n - 1))
        y = b - meters * px_per_m
        return x, y

    parts = [
        '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 980 560" role="img">',
        '<rect width="980" height="560" fill="#ffffff"/>',
    ]
    for (name, color, smooth), b in zip(series, base):
        n = len(smooth)
        pts = [xy(i, m, b, n) for i, m in enumerate(smooth)]
        d = f"M {pts[0][0]:.1f} {b:.1f} " + " ".join(f"L {x:.1f} {y:.1f}" for x, y in pts) + f" L {pts[-1][0]:.1f} {b:.1f} Z"
        parts.append(f'<path d="{d}" fill="{color}"/>')
        parts.append(
            f'<text x="24" y="{b - 6:.0f}" fill="#271825" font-family="ui-sans-serif, sans-serif" font-size="14" font-weight="650">{name}</text>'
        )
        parts.append(f'<rect x="24" y="{b + 6:.0f}" width="16" height="8" fill="{color}"/>')

    front_name, front_color, front = series[-1]
    n = len(front)
    front_pts = [xy(i, m, base[-1], n) for i, m in enumerate(front)]
    step = 14
    picks = [front_pts[i] for i in range(0, n, step)]
    if picks[-1] != front_pts[-1]:
        picks.append(front_pts[-1])
    line = " ".join(f"{x:.1f},{y:.1f}" for x, y in picks)
    parts.append(f'<polyline points="{line}" fill="none" stroke="#ffffff" stroke-width="1.75"/>')
    for x, y in picks:
        parts.append(f'<circle cx="{x:.1f}" cy="{y:.1f}" r="4.5" fill="#ffffff"/>')

    peak_i = int(np.argmax(front))
    # Coffee sits on the eastern side of the crest, not on the summit.
    labels = [
        (front_pts[6][0], "Pacific"),
        (front_pts[peak_i][0], "Andes"),
        (front_pts[min(n - 8, peak_i + 22)][0], "Coffee"),
        (front_pts[-8][0], "Amazon"),
    ]
    for x, word in labels:
        parts.append(
            f'<text x="{x:.0f}" y="530" text-anchor="middle" fill="#271825" font-family="ui-sans-serif, sans-serif" font-size="14">{word}</text>'
        )
    parts.append("</svg>")
    path = ROOT / "wwwroot" / "brand" / "peru-cuts.svg"
    path.write_text("\n".join(parts), encoding="utf-8")
    print("wrote", path)


def main():
    elev = mosaic(download())
    land = mask(elev.shape)
    write_cuts(elev, land)
    if "--cuts-only" in sys.argv:
        return
    rgb = colorize(elev) * hillshade(elev)[:, :, None]
    rgb = np.clip(rgb, 0, 255).astype(np.uint8)
    alpha = land
    # Soft edge, still a crisp country.
    edge = Image.fromarray(alpha).filter(ImageFilter.GaussianBlur(0.6))
    out = np.dstack([rgb, np.asarray(edge)])
    ys, xs = np.where(alpha > 8)
    pad = 18
    y0, y1 = max(0, ys.min() - pad), min(out.shape[0], ys.max() + pad)
    x0, x1 = max(0, xs.min() - pad), min(out.shape[1], xs.max() + pad)
    image = Image.fromarray(out[y0:y1, x0:x1], "RGBA")
    flat = Image.new("RGB", image.size, (255, 255, 255))
    flat.paste(image, mask=image.getchannel("A"))
    flat.save(OUT, optimize=True)
    print("wrote", OUT, flat.size)


if __name__ == "__main__":
    main()
