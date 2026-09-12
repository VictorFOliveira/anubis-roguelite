from collections import deque
from pathlib import Path
from PIL import Image

src = Path(r"C:\Users\victor-work-netbr\.cursor\projects\c-Users-victor-work-netbr-Documents-pessoal-Jogo\assets\anubis-attack1-strip.png")
im = Image.open(src).convert("RGBA")
pix = im.load()
w, h = im.size


def is_backdrop(r, g, b, a):
    if a < 10:
        return True
    return r >= 160 and b >= 90 and g <= 150 and (r - g) >= 45


visited = [[False] * w for _ in range(h)]
queue = deque([(0, 0), (w - 1, 0), (0, h - 1), (w - 1, h - 1)])
for x, y in list(queue):
    visited[y][x] = True

while queue:
    x, y = queue.popleft()
    r, g, b, a = pix[x, y]
    if not is_backdrop(r, g, b, a):
        continue
    pix[x, y] = (0, 0, 0, 0)
    for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
        if 0 <= nx < w and 0 <= ny < h and not visited[ny][nx]:
            visited[ny][nx] = True
            queue.append((nx, ny))

for y in range(h):
    for x in range(w):
        r, g, b, a = pix[x, y]
        if a and is_backdrop(r, g, b, a):
            pix[x, y] = (0, 0, 0, 0)

bbox = im.getbbox()
if bbox:
    im = im.crop(bbox)
    w, h = im.size

frames_dir = Path(r"c:\Users\victor-work-netbr\Documents\pessoal\Jogo\Assets\Resources\Characters\AnubisAttack")
art_dir = Path(r"c:\Users\victor-work-netbr\Documents\pessoal\Jogo\Assets\Art\Characters\AnubisAttack")
frames_dir.mkdir(parents=True, exist_ok=True)
art_dir.mkdir(parents=True, exist_ok=True)

count = 4
frame_w = w // count
for i in range(count):
    left = i * frame_w
    right = w if i == count - 1 else (i + 1) * frame_w
    frame = im.crop((left, 0, right, h))
    box = frame.getbbox()
    if box:
        pad = 8
        frame = frame.crop((
            max(0, box[0] - pad),
            max(0, box[1] - pad),
            min(frame.width, box[2] + pad),
            min(frame.height, box[3] + pad),
        ))
    name = f"Attack1_{i}.png"
    frame.save(frames_dir / name, "PNG")
    frame.save(art_dir / name, "PNG")
    print(f"saved {name} {frame.size}")

im.save(art_dir / "Attack1_Strip.png", "PNG")
print(f"strip {im.size}")
