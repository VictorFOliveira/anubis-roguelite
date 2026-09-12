using Anubis.Characters;
using Anubis.Core;
using Anubis.Save;
using UnityEngine;
using UnityEngine.UI;

namespace Anubis.UI
{
    public sealed class HudView : MonoBehaviour
    {
        Text _health;
        Text _meta;
        Text _hint;
        Image _healthFill;
        GameSignals _signals;
        PlayerController _player;

        public void Bind(PlayerController player, GameSignals signals, MetaProgression meta)
        {
            _player = player;
            _signals = signals;
            Build();
            RefreshHealth(new HealthChangeInfo(player.Health.Current, player.Health.Max, Combat.TeamId.Player));
            _meta.text = $"Arenas: {meta.Data.ArenasCleared}   Runs: {meta.Data.RunsStarted}   Bônus permanente: +{meta.Data.PermanentHealthBonus} HP";
            signals.PlayerHealthChanged.Subscribe(RefreshHealth);
            signals.EncounterStarted.Subscribe(() => _hint.text = "Elimine os inimigos.  Ataque: mouse esquerdo / X   Dash: espaço / B");
            signals.RewardReady.Subscribe(() => _hint.text = "Sala limpa. Aproxime-se do santuário ou pressione E / Y.");
            signals.BlessingOfferOpened.Subscribe(() => _hint.text = "Escolha uma bênção.");
        }

        void RefreshHealth(HealthChangeInfo info)
        {
            if (_health == null)
            {
                return;
            }

            _health.text = $"Anúbis  {Mathf.CeilToInt(info.Current)}/{Mathf.CeilToInt(info.Max)}";
            if (_healthFill != null && info.Max > 0f)
            {
                _healthFill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(info.Current / info.Max), 1f);
            }
        }

        void Build()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            gameObject.AddComponent<GraphicRaycaster>();

            var barRoot = CreatePanel("HealthBar", new Vector2(0f, 1f), new Vector2(40, -40), new Vector2(420, 36), new Color(0.08f, 0.06f, 0.04f, 0.85f));
            var fill = CreatePanel("Fill", new Vector2(0f, 0.5f), Vector2.zero, new Vector2(420, 36), new Color(0.72f, 0.18f, 0.16f, 1f), barRoot.transform);
            fill.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            fill.GetComponent<RectTransform>().anchorMax = Vector2.one;
            fill.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            fill.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            _healthFill = fill.GetComponent<Image>();

            _health = CreateText("HealthLabel", new Vector2(0f, 1f), new Vector2(40, -82), 28, FontStyle.Bold);
            _meta = CreateText("Meta", new Vector2(0f, 1f), new Vector2(40, -118), 20, FontStyle.Normal);
            _hint = CreateText("Hint", new Vector2(0.5f, 0f), new Vector2(0, 48), 22, FontStyle.Normal);
            _hint.alignment = TextAnchor.MiddleCenter;
            _hint.rectTransform.sizeDelta = new Vector2(1400, 40);
        }

        GameObject CreatePanel(string name, Vector2 anchor, Vector2 anchored, Vector2 size, Color color, Transform parent = null)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent == null ? transform : parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchored;
            rect.sizeDelta = size;
            go.GetComponent<Image>().color = color;
            return go;
        }

        Text CreateText(string name, Vector2 anchor, Vector2 anchored, int size, FontStyle style)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(transform, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchored;
            rect.sizeDelta = new Vector2(720, 36);
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.color = new Color(0.96f, 0.9f, 0.74f);
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            return text;
        }

        void OnDestroy()
        {
            if (_signals == null)
            {
                return;
            }

            _signals.PlayerHealthChanged.Unsubscribe(RefreshHealth);
        }
    }
}
