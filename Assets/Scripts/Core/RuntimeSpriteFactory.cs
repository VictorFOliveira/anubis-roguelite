using UnityEngine;

namespace Anubis.Core
{
    public static class RuntimeSpriteFactory
    {
        public static Sprite CreateCircle(Color color, int size = 64, float fill = 0.92f)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = $"Circle_{ColorUtility.ToHtmlStringRGB(color)}"
            };

            var center = (size - 1) * 0.5f;
            var outer = center * fill;
            var innerShade = center * (fill * 0.55f);

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    if (distance > outer)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    var shade = distance < innerShade ? 1.15f : 0.75f;
                    var pixel = color * shade;
                    pixel.a = 1f;
                    if (distance > outer - 2f)
                    {
                        pixel *= 0.45f;
                        pixel.a = 1f;
                    }

                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        public static Sprite CreateDiamond(Color color, int size = 48)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            var center = (size - 1) * 0.5f;
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var manhattan = Mathf.Abs(x - center) + Mathf.Abs(y - center);
                    texture.SetPixel(x, y, manhattan <= center * 0.9f ? color : Color.clear);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        public static Sprite CreateRect(Color color, int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var edge = x == 0 || y == 0 || x == width - 1 || y == height - 1;
                    texture.SetPixel(x, y, edge ? color * 0.55f : color);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 64f);
        }
    }
}
