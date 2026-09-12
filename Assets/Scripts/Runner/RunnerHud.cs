using UnityEngine;
using UnityEngine.UI;

namespace Anubis.Runner
{
    public sealed class RunnerHud : MonoBehaviour
    {
        RunnerPlayerController _player;
        RunnerWorldGenerator _world;
        Text _distance;
        Text _status;
        Text _biome;
        Text _upgrade;
        Text _gameOver;
        float _upgradeTimer;

        public void Bind(RunnerPlayerController player, RunnerWorldGenerator world)
        {
            _player = player;
            _world = world;
            Build();
            _world.BiomeChanged += OnBiomeChanged;
            _player.UpgradeCollected += OnUpgradeCollected;
            OnBiomeChanged(_world.CurrentBiome);
        }

        void Update()
        {
            if (_player == null)
            {
                return;
            }

            var meters = _player.DistanceMeters;
            _distance.text = meters < 1000f
                ? $"{Mathf.FloorToInt(meters)} m"
                : $"{meters / 1000f:0.00} km";

            _status.text = $"ESCUDO x{_player.ShieldCharges}   SALTO x{_player.JumpMultiplier:0.00}";

            if (_upgradeTimer > 0f)
            {
                _upgradeTimer -= Time.deltaTime;
                if (_upgradeTimer <= 0f)
                {
                    _upgrade.text = string.Empty;
                }
            }
        }

        public void ShowGameOver(float distance)
        {
            var value = distance < 1000f ? $"{Mathf.FloorToInt(distance)} m" : $"{distance / 1000f:0.00} km";
            _gameOver.text = $"ANÚBIS CAIU\nDISTÂNCIA: {value}\n\nR / START para correr novamente";
            _gameOver.gameObject.SetActive(true);
        }

        void OnBiomeChanged(RunnerBiome biome)
        {
            _biome.text = biome switch
            {
                RunnerBiome.Giza => "DUNAS DE GIZÉ",
                RunnerBiome.Luxor => "TEMPLOS DE LUXOR",
                RunnerBiome.AbuSimbel => "ABU SIMBEL",
                RunnerBiome.Ur => "UR • MESOPOTÂMIA",
                _ => biome.ToString().ToUpperInvariant()
            };
        }

        void OnUpgradeCollected(string message)
        {
            _upgrade.text = message;
            _upgradeTimer = 2.4f;
        }

        void Build()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            gameObject.AddComponent<GraphicRaycaster>();

            _distance = CreateText("Distance", new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(520f, 76f), 46, FontStyle.Bold, TextAnchor.MiddleCenter);
            _biome = CreateText("Biome", new Vector2(0.5f, 1f), new Vector2(0f, -112f), new Vector2(600f, 44f), 22, FontStyle.Bold, TextAnchor.MiddleCenter);
            _status = CreateText("Status", new Vector2(0f, 1f), new Vector2(34f, -46f), new Vector2(650f, 44f), 22, FontStyle.Bold, TextAnchor.MiddleLeft);
            _upgrade = CreateText("Upgrade", new Vector2(0.5f, 0.18f), new Vector2(0f, 0f), new Vector2(900f, 60f), 30, FontStyle.Bold, TextAnchor.MiddleCenter);

            var god = CreateText("Ra", new Vector2(1f, 1f), new Vector2(-42f, -42f), new Vector2(360f, 48f), 24, FontStyle.Bold, TextAnchor.MiddleRight);
            god.text = "RÁ • JULGAMENTO SOLAR";

            _gameOver = CreateText("GameOver", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 300f), 42, FontStyle.Bold, TextAnchor.MiddleCenter);
            _gameOver.gameObject.SetActive(false);
        }

        Text CreateText(string name, Vector2 anchor, Vector2 anchored, Vector2 size, int fontSize, FontStyle style, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(transform, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchored;
            rect.sizeDelta = size;

            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = new Color(1f, 0.9f, 0.63f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        void OnDestroy()
        {
            if (_world != null) _world.BiomeChanged -= OnBiomeChanged;
            if (_player != null) _player.UpgradeCollected -= OnUpgradeCollected;
        }
    }
}
