using System.Collections.Generic;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Runner
{
    public sealed class RunnerBackdrop : MonoBehaviour
    {
        readonly List<GameObject> _generated = new();
        Camera _camera;
        Transform _farLayer;
        Transform _midLayer;
        Sprite _unit;
        RunnerBiome _biome;

        public void Configure(Camera camera)
        {
            _camera = camera;
            transform.SetParent(camera.transform, false);
            transform.localPosition = Vector3.zero;
            _unit = RuntimeSpriteFactory.CreateRect(Color.white, 64, 64);

            _farLayer = new GameObject("FarLayer").transform;
            _farLayer.SetParent(transform, false);
            _midLayer = new GameObject("MidLayer").transform;
            _midLayer.SetParent(transform, false);

            SetBiome(RunnerBiome.Giza);
        }

        public void SetBiome(RunnerBiome biome)
        {
            if (_camera == null)
            {
                return;
            }

            _biome = biome;
            foreach (var go in _generated)
            {
                if (go != null) Destroy(go);
            }
            _generated.Clear();

            switch (biome)
            {
                case RunnerBiome.Giza:
                    _camera.backgroundColor = new Color(0.86f, 0.59f, 0.30f);
                    BuildGiza();
                    break;
                case RunnerBiome.Luxor:
                    _camera.backgroundColor = new Color(0.76f, 0.44f, 0.20f);
                    BuildLuxor();
                    break;
                case RunnerBiome.AbuSimbel:
                    _camera.backgroundColor = new Color(0.69f, 0.36f, 0.18f);
                    BuildAbuSimbel();
                    break;
                case RunnerBiome.Ur:
                    _camera.backgroundColor = new Color(0.55f, 0.35f, 0.25f);
                    BuildUr();
                    break;
            }
        }

        void Update()
        {
            if (_camera == null)
            {
                return;
            }

            var x = _camera.transform.position.x;
            _farLayer.localPosition = new Vector3(-Mathf.Repeat(x * 0.08f, 8f), 0f, 0f);
            _midLayer.localPosition = new Vector3(-Mathf.Repeat(x * 0.17f, 10f), 0f, 0f);
        }

        void BuildGiza()
        {
            AddSun(new Vector2(5.9f, 2.8f), new Color(1f, 0.78f, 0.27f));
            AddSteppedPyramid(_farLayer, -7.5f, -1.75f, 5.8f, 3.2f, new Color(0.48f, 0.29f, 0.16f), -26);
            AddSteppedPyramid(_farLayer, 0.3f, -1.9f, 4.4f, 2.6f, new Color(0.54f, 0.33f, 0.17f), -25);
            AddSteppedPyramid(_farLayer, 7.4f, -1.95f, 3.4f, 2.1f, new Color(0.58f, 0.36f, 0.18f), -25);
            AddDuneStrip(_midLayer, new Color(0.72f, 0.46f, 0.22f));
        }

        void BuildLuxor()
        {
            AddSun(new Vector2(6.2f, 2.9f), new Color(1f, 0.69f, 0.20f));
            for (var i = -8; i <= 8; i += 2)
            {
                AddRect(_farLayer, $"Column_{i}", new Vector2(i, -0.6f), new Vector2(0.45f, 4f), new Color(0.39f, 0.23f, 0.14f), -25);
                AddRect(_farLayer, $"Cap_{i}", new Vector2(i, 1.25f), new Vector2(0.78f, 0.28f), new Color(0.45f, 0.27f, 0.15f), -24);
            }
            AddObelisk(_midLayer, 4.7f, -0.4f, new Color(0.54f, 0.31f, 0.16f));
            AddDuneStrip(_midLayer, new Color(0.61f, 0.35f, 0.17f));
        }

        void BuildAbuSimbel()
        {
            AddSun(new Vector2(-6.1f, 2.8f), new Color(1f, 0.61f, 0.18f));
            AddRect(_farLayer, "Cliff", new Vector2(0f, -0.2f), new Vector2(22f, 4.4f), new Color(0.34f, 0.20f, 0.13f), -28);
            for (var i = -6; i <= 6; i += 4)
            {
                AddStatue(_midLayer, i, -1.0f, new Color(0.48f, 0.28f, 0.16f));
            }
            AddDuneStrip(_midLayer, new Color(0.57f, 0.32f, 0.17f));
        }

        void BuildUr()
        {
            AddSun(new Vector2(5.8f, 2.7f), new Color(0.95f, 0.55f, 0.18f));
            AddZiggurat(_farLayer, -5.5f, -1.85f, new Color(0.31f, 0.23f, 0.18f));
            AddZiggurat(_farLayer, 5.0f, -1.95f, new Color(0.37f, 0.26f, 0.18f));
            for (var i = -8; i <= 8; i += 3)
            {
                AddRect(_midLayer, $"UrWall_{i}", new Vector2(i, -1.8f), new Vector2(2.1f, 1.15f), new Color(0.43f, 0.30f, 0.20f), -21);
            }
        }

        void AddSteppedPyramid(Transform parent, float x, float y, float width, float height, Color color, int order)
        {
            const int steps = 7;
            for (var i = 0; i < steps; i++)
            {
                var t = i / (float)steps;
                var stepWidth = Mathf.Lerp(width, width * 0.12f, t);
                var stepHeight = height / steps;
                AddRect(parent, $"PyramidStep_{x}_{i}", new Vector2(x, y + i * stepHeight), new Vector2(stepWidth, stepHeight + 0.04f), color * Mathf.Lerp(1f, 0.83f, t), order);
            }
        }

        void AddZiggurat(Transform parent, float x, float y, Color color)
        {
            AddRect(parent, "ZigguratBase", new Vector2(x, y), new Vector2(6.5f, 1.0f), color, -25);
            AddRect(parent, "Ziggurat2", new Vector2(x, y + 0.9f), new Vector2(5.0f, 0.85f), color * 1.08f, -24);
            AddRect(parent, "Ziggurat3", new Vector2(x, y + 1.65f), new Vector2(3.4f, 0.72f), color * 1.14f, -23);
            AddRect(parent, "ZigguratTop", new Vector2(x, y + 2.25f), new Vector2(1.7f, 0.55f), color * 1.2f, -22);
        }

        void AddStatue(Transform parent, float x, float y, Color color)
        {
            AddRect(parent, "StatueBody", new Vector2(x, y), new Vector2(1.45f, 2.7f), color, -22);
            AddRect(parent, "StatueShoulders", new Vector2(x, y + 1.0f), new Vector2(2.0f, 0.55f), color * 1.08f, -21);
            AddCircle(parent, "StatueHead", new Vector2(x, y + 1.9f), 0.82f, color * 1.12f, -20);
        }

        void AddObelisk(Transform parent, float x, float y, Color color)
        {
            AddRect(parent, "Obelisk", new Vector2(x, y), new Vector2(0.62f, 4.2f), color, -20);
            AddRect(parent, "ObeliskTip", new Vector2(x, y + 2.25f), new Vector2(0.38f, 0.45f), color * 1.15f, -19);
        }

        void AddDuneStrip(Transform parent, Color color)
        {
            for (var i = -10; i <= 10; i += 2)
            {
                AddRect(parent, $"Dune_{i}", new Vector2(i, -2.3f + Mathf.Sin(i * 0.7f) * 0.18f), new Vector2(2.5f, 0.65f), color, -18);
            }
        }

        void AddSun(Vector2 position, Color color)
        {
            AddCircle(_farLayer, "SunDisc", position, 1.2f, color, -30);
        }

        void AddRect(Transform parent, string name, Vector2 position, Vector2 scale, Color color, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = _unit;
            renderer.color = color;
            renderer.sortingOrder = order;
            _generated.Add(go);
        }

        void AddCircle(Transform parent, string name, Vector2 position, float scale, Color color, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = Vector3.one * scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.CreateCircle(color, 32, 0.94f);
            renderer.sortingOrder = order;
            _generated.Add(go);
        }
    }
}
