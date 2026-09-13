using UnityEngine;

namespace Anubis.Runner
{
    public static class RunnerSpriteLoader
    {
        public static Sprite Load(string spritePath, string rawPath, Vector2 pivot, float pixelsPerUnit = 100f)
        {
            var raw = Resources.Load<TextAsset>(rawPath);
            if (raw != null && raw.bytes != null && raw.bytes.Length > 0)
            {
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
                if (ImageConversion.LoadImage(texture, raw.bytes, false))
                {
                    texture.filterMode = FilterMode.Point;
                    return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect);
                }
            }

            var sprite = Resources.Load<Sprite>(spritePath);
            if (sprite != null) return sprite;

            var sprites = Resources.LoadAll<Sprite>(spritePath);
            if (sprites != null && sprites.Length > 0) return sprites[0];

            var importedTexture = Resources.Load<Texture2D>(spritePath);
            if (importedTexture != null)
            {
                importedTexture.filterMode = FilterMode.Point;
                return Sprite.Create(importedTexture, new Rect(0f, 0f, importedTexture.width, importedTexture.height), pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect);
            }

            Debug.LogWarning($"[Runner] Asset não carregado: {spritePath} / {rawPath}");
            return null;
        }
    }
}
