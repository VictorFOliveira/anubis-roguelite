using System;
using UnityEngine;
using UnityEngine.UI;

namespace Anubis.UI
{
    public sealed class EndPanelView : MonoBehaviour
    {
        public event Action RestartRequested;

        GameObject _root;
        Text _title;
        Text _body;

        public void Build()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 250;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            gameObject.AddComponent<GraphicRaycaster>();

            _root = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            _root.transform.SetParent(transform, false);
            var rect = _root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            _root.GetComponent<Image>().color = new Color(0.02f, 0.01f, 0.01f, 0.78f);

            _title = CreateText("Title", 48, new Vector2(0, 80));
            _body = CreateText("Body", 24, new Vector2(0, 0));

            var buttonGo = new GameObject("Restart", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGo.transform.SetParent(_root.transform, false);
            var buttonRect = buttonGo.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.sizeDelta = new Vector2(280, 64);
            buttonRect.anchoredPosition = new Vector2(0, -120);
            buttonGo.GetComponent<Image>().color = new Color(0.72f, 0.52f, 0.16f);
            var button = buttonGo.GetComponent<Button>();
            button.onClick.AddListener(() => RestartRequested?.Invoke());
            var label = CreateText("RestartLabel", 26, Vector2.zero);
            label.transform.SetParent(buttonGo.transform, false);
            label.text = "Reiniciar";
            label.rectTransform.anchoredPosition = Vector2.zero;

            Hide();
        }

        public void Show(string title, string body)
        {
            _title.text = title;
            _body.text = body;
            _root.SetActive(true);
        }

        public void Hide()
        {
            _root.SetActive(false);
        }

        Text CreateText(string name, int size, Vector2 offset)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(_root != null ? _root.transform : transform, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(900, 80);
            rect.anchoredPosition = offset;
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.96f, 0.9f, 0.74f);
            return text;
        }
    }
}
