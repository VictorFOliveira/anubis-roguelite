using System;
using System.Collections.Generic;
using Anubis.Progression;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Anubis.UI
{
    public sealed class BlessingChoiceView : MonoBehaviour
    {
        public event Action<BlessingDefinition> Chosen;

        readonly List<Button> _buttons = new();
        GameObject _root;
        int _selected;

        public void Build()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            gameObject.AddComponent<GraphicRaycaster>();

            _root = new GameObject("Offer", typeof(RectTransform), typeof(Image));
            _root.transform.SetParent(transform, false);
            var overlay = _root.GetComponent<RectTransform>();
            overlay.anchorMin = Vector2.zero;
            overlay.anchorMax = Vector2.one;
            overlay.offsetMin = Vector2.zero;
            overlay.offsetMax = Vector2.zero;
            _root.GetComponent<Image>().color = new Color(0.02f, 0.02f, 0.03f, 0.72f);

            var titleGo = new GameObject("Title", typeof(RectTransform), typeof(Text));
            titleGo.transform.SetParent(_root.transform, false);
            var titleRect = titleGo.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.82f);
            titleRect.anchorMax = new Vector2(0.5f, 0.82f);
            titleRect.sizeDelta = new Vector2(900, 64);
            var title = titleGo.GetComponent<Text>();
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 40;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = new Color(0.96f, 0.86f, 0.45f);
            title.text = "Escolha uma bênção";

            Hide();
        }

        public void Show(IReadOnlyList<BlessingDefinition> options)
        {
            ClearCards();
            _root.SetActive(true);
            _selected = 0;
            for (var i = 0; i < options.Count; i++)
            {
                CreateCard(options[i], i, options.Count);
            }

            if (_buttons.Count > 0)
            {
                EventSystem.current?.SetSelectedGameObject(_buttons[0].gameObject);
            }
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.SetActive(false);
            }
        }

        public void MoveSelection(int delta)
        {
            if (_buttons.Count == 0 || !_root.activeSelf)
            {
                return;
            }

            _selected = (_selected + delta + _buttons.Count) % _buttons.Count;
            _buttons[_selected].Select();
        }

        public void ConfirmSelection()
        {
            ConfirmIndex(_selected);
        }

        public void ConfirmIndex(int index)
        {
            if (_buttons.Count == 0 || !_root.activeSelf || index < 0 || index >= _buttons.Count)
            {
                return;
            }

            _selected = index;
            _buttons[_selected].onClick.Invoke();
        }

        void CreateCard(BlessingDefinition definition, int index, int total)
        {
            var card = new GameObject(definition.Id, typeof(RectTransform), typeof(Image), typeof(Button));
            card.transform.SetParent(_root.transform, false);
            var rect = card.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.45f);
            rect.anchorMax = new Vector2(0.5f, 0.45f);
            rect.sizeDelta = new Vector2(360, 420);
            rect.anchoredPosition = new Vector2((index - (total - 1) * 0.5f) * 400f, 0f);
            card.GetComponent<Image>().color = new Color(0.12f, 0.09f, 0.06f, 0.95f);

            var button = card.GetComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = definition.Accent;
            colors.selectedColor = definition.Accent;
            button.colors = colors;
            button.onClick.AddListener(() => Chosen?.Invoke(definition));
            _buttons.Add(button);

            AddLabel(card.transform, definition.DisplayName, 28, new Vector2(0, 120), definition.Accent);
            AddLabel(card.transform, definition.Description, 20, new Vector2(0, -20), Color.white);
            AddLabel(card.transform, $"{index + 1}", 18, new Vector2(0, -160), new Color(0.8f, 0.75f, 0.6f));
        }

        static void AddLabel(Transform parent, string value, int size, Vector2 offset, Color color)
        {
            var go = new GameObject("Label", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(300, 160);
            rect.anchoredPosition = offset;
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.text = value;
        }

        void ClearCards()
        {
            foreach (var button in _buttons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }

            _buttons.Clear();
        }
    }
}
