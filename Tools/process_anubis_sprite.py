from collections import deque
from pathlib import Path
from PIL import Image

src = Path(r"C:\Users\victor-work-netbr\.cursor\projects\c-Users-victor-work-netbr-Documents-pessoal-Jogo\assets\anubis-classic-ingame.png")
im = Image.open(src).convert("RGBA")
pix = im.load()
w, h = im.size


def is_backdrop(r, g, b, a):
    if a < 10:
        return True
    # hot pink / magenta: red high, green low, blue mid-high
    return r >= 160 and b >= 90 and g <= 140 and (r - g) >= 50


visited = [[False] * w for _ in range(h)]
queue = deque()
for x, y in [(0, 0), (w - 1, 0), (0, h - 1), (w - 1, h - 1)]:
    queue.append((x, y))
    visited[y][x] = True

removed = 0
while queue:
    x, y = queue.popleft()
    r, g, b, a = pix[x, y]
    if not is_backdrop(r, g, b, a):
        continue
    pix[x, y] = (0, 0, 0, 0)
    removed += 1
    for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
        if 0 <= nx < w and 0 <= ny < h and not visited[ny][nx]:
            visited[ny][nx] = True
            queue.append((nx, ny))

# clean leftover fringe near transparent pixels
for y in range(h):
    for x in range(w):
        r, g, b, a = pix[x, y]
        if a == 0 or not is_backdrop(r, g, b, a):
            continue
        pix[x, y] = (0, 0, 0, 0)
        removed += 1

bbox = im.getbbox()
if bbox:
    pad = 10
    im = im.crop((
        max(0, bbox[0] - pad),
        max(0, bbox[1] - pad),
        min(w, bbox[2] + pad),
        min(h, bbox[3] + pad),
    ))

dests = [
    Path(r"c:\Users\victor-work-netbr\Documents\pessoal\Jogo\Assets\Art\Characters\Anubis.png"),
    Path(r"c:\Users\victor-work-netbr\Documents\pessoal\Jogo\Assets\Resources\Characters\Anubis.png"),
]
for dest in dests:
    dest.parent.mkdir(parents=True, exist_ok=True)
    im.save(dest, "PNG")
    print(f"saved {dest} {im.size} removed={removed}")
