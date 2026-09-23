"""Build WhatsApp/Facebook share cards from the brand lockup."""
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
BRAND = ROOT / "wwwroot" / "brand"
CREAM = (243, 238, 230)
INK = (39, 24, 37)
MUTED = (114, 46, 168)


def font(size: int) -> ImageFont.FreeTypeFont:
    for name in ("segoeui.ttf", "SegoeUI.ttf", "calibri.ttf", "arial.ttf"):
        path = Path(r"C:\Windows\Fonts") / name
        if path.exists():
            return ImageFont.truetype(str(path), size)
    return ImageFont.load_default()


def compose(size: tuple[int, int], logo_width: int, subtitle: str, out: Path) -> None:
    canvas = Image.new("RGB", size, CREAM)
    logo = Image.open(BRAND / "logo-lockup.png").convert("RGBA")
    scale = logo_width / logo.width
    logo = logo.resize((int(logo.width * scale), int(logo.height * scale)), Image.Resampling.LANCZOS)
    x = (size[0] - logo.width) // 2
    y = (size[1] - logo.height) // 2 - (36 if size[1] < 800 else 20)
    canvas.paste(logo, (x, y), logo)

    draw = ImageDraw.Draw(canvas)
    f = font(28 if size[1] < 800 else 36)
    bbox = draw.textbbox((0, 0), subtitle, font=f)
    tw = bbox[2] - bbox[0]
    draw.text(((size[0] - tw) / 2, y + logo.height + 28), subtitle, font=f, fill=MUTED)

    bar_h = 10
    draw.rectangle((0, size[1] - bar_h, size[0], size[1]), fill=INK)
    canvas.save(out, "JPEG", quality=90, optimize=True, progressive=True)
    print(out.name, canvas.size, out.stat().st_size)


def main() -> None:
    compose((1200, 630), 860, "Specialty coffee  ·  Cusco, Peru", BRAND / "og-share.jpg")
    compose((1200, 1200), 920, "Specialty coffee  ·  Cusco, Peru", BRAND / "og-share-square.jpg")


if __name__ == "__main__":
    main()
