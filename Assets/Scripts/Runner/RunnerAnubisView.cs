using Anubis.Core;
using UnityEngine;

namespace Anubis.Runner
{
    public sealed class RunnerAnubisView : MonoBehaviour
    {
        Transform _root;
        SpriteRenderer _body;
        SpriteRenderer _shadow;
        Transform _dustA;
        Transform _dustB;
        Vector3 _baseScale = Vector3.one;
        bool _dead;

        public void Build()
        {
            _root = new GameObject("RunnerView").transform;
            _root.SetParent(transform, false);

            _shadow = CreateRenderer("Shadow", GameSorting.Entities - 1);
            _shadow.sprite = RuntimeSpriteFactory.CreateCircle(new Color(0f, 0f, 0f, 0.32f), 64, 0.95f);
            _shadow.transform.localPosition = new Vector3(0f, -0.86f, 0f);
            _shadow.transform.localScale = new Vector3(0.9f, 0.18f, 1f);

            _body = CreateRenderer("Anubis", GameSorting.Entities + 2);
            _body.sprite = LoadAnubis();
            _body.spriteSortPoint = SpriteSortPoint.Pivot;

            var spriteHeight = _body.sprite != null ? _body.sprite.bounds.size.y : 2f;
            var scale = 2.25f / Mathf.Max(0.2f, spriteHeight);
            _baseScale = Vector3.one * scale;
            _body.transform.localScale = _baseScale;

            _dustA = CreateDust("DustA", -0.45f);
            _dustB = CreateDust("DustB", -0.75f);
        }

        public void Tick(float verticalVelocity, bool grounded, bool invulnerable, bool boosted)
        {
            if (_dead || _body == null)
            {
                return;
            }

            var cadence = boosted ? 18f : 13f;
            var phase = Time.time * cadence;

            if (grounded)
            {
                var stride = Mathf.Sin(phase);
                var bounce = Mathf.Abs(Mathf.Sin(phase)) * 0.075f;
                _body.transform.localPosition = new Vector3(0f, bounce, 0f);
                _body.transform.localRotation = Quaternion.Euler(0f, 0f, -7f + stride * 2.4f);
                var sx = 1f + Mathf.Abs(stride) * 0.035f;
                var sy = 1f - Mathf.Abs(stride) * 0.03f;
                _body.transform.localScale = Vector3.Scale(_baseScale, new Vector3(sx, sy, 1f));
                AnimateDust(_dustA, phase, 0f);
                AnimateDust(_dustB, phase + Mathf.PI, 0.22f);
            }
            else
            {
                var tilt = Mathf.Clamp(-verticalVelocity * 1.15f, -13f, 17f);
                _body.transform.localPosition = new Vector3(0f, 0.04f, 0f);
                _body.transform.localRotation = Quaternion.Euler(0f, 0f, tilt);
                var stretch = verticalVelocity > 1f ? new Vector3(0.92f, 1.08f, 1f) : new Vector3(1.08f, 0.92f, 1f);
                _body.transform.localScale = Vector3.Scale(_baseScale, stretch);
                _dustA.gameObject.SetActive(false);
                _dustB.gameObject.SetActive(false);
            }

            _body.color = invulnerable && Mathf.FloorToInt(Time.time * 12f) % 2 == 0
                ? new Color(1f, 1f, 1f, 0.35f)
                : Color.white;

            var shadowScale = grounded ? 1f : 0.75f;
            _shadow.transform.localScale = new Vector3(0.9f * shadowScale, 0.18f * shadowScale, 1f);
        }

        public void SetDead()
        {
            _dead = true;
            if (_body != null)
            {
                _body.transform.localRotation = Quaternion.Euler(0f, 0f, 82f);
                _body.color = new Color(0.75f, 0.75f, 0.75f, 1f);
            }

            if (_dustA != null) _dustA.gameObject.SetActive(false);
            if (_dustB != null) _dustB.gameObject.SetActive(false);
        }

        Transform CreateDust(string name, float x)
        {
            var renderer = CreateRenderer(name, GameSorting.Entities - 2);
            renderer.sprite = RuntimeSpriteFactory.CreateCircle(new Color(0.84f, 0.66f, 0.37f, 0.55f), 24, 0.85f);
            renderer.transform.localPosition = new Vector3(x, -0.72f, 0f);
            renderer.transform.localScale = Vector3.one * 0.28f;
            return renderer.transform;
        }

        void AnimateDust(Transform dust, float phase, float offset)
        {
            dust.gameObject.SetActive(true);
            var t = Mathf.Repeat(phase * 0.16f + offset, 1f);
            dust.localPosition = new Vector3(-0.35f - t * 0.65f, -0.72f + t * 0.15f, 0f);
            dust.localScale = Vector3.one * Mathf.Lerp(0.26f, 0.08f, t);
        }

        SpriteRenderer CreateRenderer(string name, int order)
        {
            var child = new GameObject(name);
            child.transform.SetParent(_root, false);
            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = order;
            return renderer;
        }

        static Sprite LoadAnubis()
        {
            var sprites = Resources.LoadAll<Sprite>("Characters/Anubis");
            if (sprites != null && sprites.Length > 0)
            {
                return sprites[0];
            }

            return Resources.Load<Sprite>("Characters/Anubis")
                ?? RuntimeSpriteFactory.CreateRect(new Color(0.12f, 0.08f, 0.08f), 48, 96);
        }
    }
}
