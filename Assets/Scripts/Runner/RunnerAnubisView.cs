using System.Collections.Generic;
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

        Sprite _idleSprite;
        Sprite _walkSprite;
        Sprite _runSprite;
        Sprite _jumpSprite;
        Sprite _slideSprite;
        Sprite[] _runFrames = System.Array.Empty<Sprite>();

        float _landingTimer;
        float _jumpKickTimer;
        float _slideKickTimer;
        float _stompTimer;
        float _runFrameTimer;
        int _runFrameIndex;
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
            _body.spriteSortPoint = SpriteSortPoint.Pivot;

            LoadMovementSprites();
            ApplySprite(_idleSprite ?? _runSprite, 2.25f);

            _dustA = CreateDust("DustA", -0.45f);
            _dustB = CreateDust("DustB", -0.75f);
        }

        public void Tick(
            float verticalVelocity,
            bool grounded,
            bool invulnerable,
            bool boosted,
            bool sliding,
            float runSpeed)
        {
            if (_dead || _body == null)
            {
                return;
            }

            TickMotionTimers();

            if (sliding && grounded)
            {
                AnimateSlide(boosted);
            }
            else if (!grounded)
            {
                AnimateAir(verticalVelocity, boosted);
            }
            else
            {
                AnimateRun(runSpeed, boosted);
            }

            _body.color = invulnerable && Mathf.FloorToInt(Time.time * 14f) % 2 == 0
                ? new Color(1f, 1f, 1f, 0.30f)
                : Color.white;

            AnimateShadow(grounded, sliding, verticalVelocity);
        }

        public void TriggerLanding()
        {
            _landingTimer = 0.16f;
        }

        public void TriggerJump()
        {
            _jumpKickTimer = 0.15f;
        }

        public void TriggerSlide()
        {
            _slideKickTimer = 0.16f;
        }

        public void TriggerStompBounce()
        {
            _stompTimer = 0.18f;
        }

        void TickMotionTimers()
        {
            if (_landingTimer > 0f) _landingTimer -= Time.deltaTime;
            if (_jumpKickTimer > 0f) _jumpKickTimer -= Time.deltaTime;
            if (_slideKickTimer > 0f) _slideKickTimer -= Time.deltaTime;
            if (_stompTimer > 0f) _stompTimer -= Time.deltaTime;
        }

        void AnimateRun(float runSpeed, bool boosted)
        {
            var speed01 = Mathf.InverseLerp(7f, 12f, runSpeed);
            var cadence = Mathf.Lerp(9.5f, 14.5f, speed01);
            if (boosted) cadence *= 1.18f;

            _runFrameTimer += Time.deltaTime * cadence;
            if (_runFrames.Length > 0)
            {
                var index = Mathf.FloorToInt(_runFrameTimer) % _runFrames.Length;
                if (index != _runFrameIndex || _body.sprite == null)
                {
                    _runFrameIndex = index;
                    ApplySprite(_runFrames[_runFrameIndex], 2.22f);
                }
            }
            else
            {
                ApplySprite(_runSprite ?? _walkSprite ?? _idleSprite, 2.22f);
            }

            var phase = Time.time * cadence;
            var stride = Mathf.Sin(phase);
            var footPlant = Mathf.Abs(Mathf.Sin(phase));
            var bounce = footPlant * Mathf.Lerp(0.038f, 0.072f, speed01);
            var lean = boosted ? -10f : Mathf.Lerp(-4.5f, -7.5f, speed01);

            var landingAmount = _landingTimer > 0f
                ? Mathf.Sin((1f - _landingTimer / 0.16f) * Mathf.PI)
                : 0f;

            var sx = 1f + footPlant * 0.025f + landingAmount * 0.10f;
            var sy = 1f - footPlant * 0.020f - landingAmount * 0.13f;

            _body.transform.localPosition = new Vector3(0f, bounce - landingAmount * 0.05f, 0f);
            _body.transform.localRotation = Quaternion.Euler(0f, 0f, lean + stride * 1.8f);
            MultiplyCurrentScale(sx, sy);

            AnimateDust(_dustA, phase, 0f, boosted ? 1.25f : 1f);
            AnimateDust(_dustB, phase + Mathf.PI, 0.22f, boosted ? 1.15f : 0.9f);
        }

        void AnimateAir(float verticalVelocity, bool boosted)
        {
            ApplySprite(_jumpSprite ?? _runSprite ?? _idleSprite, 2.18f);

            var ascending = verticalVelocity > 0.35f;
            var velocity01 = Mathf.Clamp(verticalVelocity / 12f, -1f, 1f);
            var tilt = ascending
                ? Mathf.Lerp(-5f, -14f, Mathf.Clamp01(velocity01))
                : Mathf.Lerp(5f, 16f, Mathf.Clamp01(-velocity01));

            if (_stompTimer > 0f)
            {
                tilt -= 7f;
            }

            var jumpKick = _jumpKickTimer > 0f
                ? Mathf.Sin((1f - _jumpKickTimer / 0.15f) * Mathf.PI)
                : 0f;

            var stretchX = ascending ? 0.94f : 1.04f;
            var stretchY = ascending ? 1.07f : 0.96f;
            stretchX -= jumpKick * 0.035f;
            stretchY += jumpKick * 0.055f;

            if (boosted)
            {
                stretchX *= 0.95f;
                stretchY *= 1.06f;
                tilt -= 5f;
            }

            _body.transform.localPosition = new Vector3(0.02f, 0.08f + jumpKick * 0.05f, 0f);
            _body.transform.localRotation = Quaternion.Euler(0f, 0f, tilt);
            MultiplyCurrentScale(stretchX, stretchY);

            _dustA.gameObject.SetActive(false);
            _dustB.gameObject.SetActive(false);
        }

        void AnimateSlide(bool boosted)
        {
            ApplySprite(_slideSprite ?? _runSprite ?? _idleSprite, 1.52f);

            var phase = Time.time * (boosted ? 22f : 17f);
            var vibration = Mathf.Sin(phase) * 0.018f;
            var entry = _slideKickTimer > 0f
                ? Mathf.Sin((1f - _slideKickTimer / 0.16f) * Mathf.PI)
                : 0f;

            _body.transform.localPosition = new Vector3(0.18f, -0.38f + vibration, 0f);
            _body.transform.localRotation = Quaternion.Euler(0f, 0f, -3f - entry * 4f);
            MultiplyCurrentScale(1.06f + entry * 0.08f, 0.94f - entry * 0.04f);

            AnimateDust(_dustA, phase, 0.05f, 1.55f);
            AnimateDust(_dustB, phase + 1.6f, 0.34f, 1.35f);
        }

        void AnimateShadow(bool grounded, bool sliding, float verticalVelocity)
        {
            if (_shadow == null)
            {
                return;
            }

            if (sliding && grounded)
            {
                _shadow.transform.localPosition = new Vector3(0.20f, -0.84f, 0f);
                _shadow.transform.localScale = new Vector3(1.22f, 0.15f, 1f);
                return;
            }

            _shadow.transform.localPosition = new Vector3(0f, -0.86f, 0f);
            var airborne = grounded ? 0f : Mathf.Clamp01(Mathf.Abs(verticalVelocity) / 14f);
            var x = Mathf.Lerp(0.9f, 0.58f, airborne);
            var y = Mathf.Lerp(0.18f, 0.11f, airborne);
            _shadow.transform.localScale = new Vector3(x, y, 1f);
        }

        void LoadMovementSprites()
        {
            var legacy = LoadFirstSprite("Characters/Anubis");
            _idleSprite = LoadFirstSprite("Runner/anubis_idle", "Runner/Anubis/anubis_idle") ?? legacy;
            _walkSprite = LoadFirstSprite("Runner/anubis_walk", "Runner/Anubis/anubis_walk");
            _runSprite = LoadFirstSprite("Runner/anubis_run", "Runner/Anubis/anubis_run") ?? _walkSprite ?? _idleSprite;
            _jumpSprite = LoadFirstSprite("Runner/anubis_jump", "Runner/Anubis/anubis_jump") ?? _runSprite;
            _slideSprite = LoadFirstSprite("Runner/anubis_slide", "Runner/Anubis/anubis_slide") ?? _runSprite;

            var authoredRun = LoadFrames("Runner/AnubisRun");
            if (authoredRun.Length > 0)
            {
                _runFrames = authoredRun;
                return;
            }

            var fallback = new List<Sprite>();
            if (_walkSprite != null) fallback.Add(_walkSprite);
            if (_runSprite != null && _runSprite != _walkSprite) fallback.Add(_runSprite);
            if (fallback.Count == 0 && _idleSprite != null) fallback.Add(_idleSprite);
            _runFrames = fallback.ToArray();
        }

        void ApplySprite(Sprite sprite, float targetHeight)
        {
            if (_body == null || sprite == null)
            {
                return;
            }

            _body.sprite = sprite;
            var height = Mathf.Max(0.05f, sprite.bounds.size.y);
            var scale = targetHeight / height;
            _body.transform.localScale = Vector3.one * scale;
        }

        void MultiplyCurrentScale(float x, float y)
        {
            if (_body == null || _body.sprite == null)
            {
                return;
            }

            var stateHeight = _body.sprite == _slideSprite ? 1.52f : 2.22f;
            if (_body.sprite == _jumpSprite) stateHeight = 2.18f;
            var height = Mathf.Max(0.05f, _body.sprite.bounds.size.y);
            var baseScale = stateHeight / height;
            _body.transform.localScale = new Vector3(baseScale * x, baseScale * y, 1f);
        }

        public void SetDead()
        {
            _dead = true;
            if (_body != null)
            {
                _body.transform.localRotation = Quaternion.Euler(0f, 0f, 82f);
                _body.color = new Color(0.72f, 0.72f, 0.72f, 1f);
                MultiplyCurrentScale(1.08f, 0.92f);
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

        void AnimateDust(Transform dust, float phase, float offset, float strength)
        {
            dust.gameObject.SetActive(true);
            var t = Mathf.Repeat(phase * 0.16f + offset, 1f);
            dust.localPosition = new Vector3(-0.34f - t * 0.78f * strength, -0.72f + t * 0.16f, 0f);
            var scale = Mathf.Lerp(0.28f * strength, 0.06f, t);
            dust.localScale = new Vector3(scale * 1.25f, scale * 0.72f, 1f);
        }

        SpriteRenderer CreateRenderer(string name, int order)
        {
            var child = new GameObject(name);
            child.transform.SetParent(_root, false);
            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = order;
            return renderer;
        }

        static Sprite LoadFirstSprite(params string[] paths)
        {
            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                var single = Resources.Load<Sprite>(path);
                if (single != null)
                {
                    return single;
                }

                var sprites = Resources.LoadAll<Sprite>(path);
                if (sprites != null && sprites.Length > 0)
                {
                    System.Array.Sort(sprites, (a, b) => string.CompareOrdinal(a.name, b.name));
                    return sprites[0];
                }
            }

            return null;
        }

        static Sprite[] LoadFrames(string path)
        {
            var sprites = Resources.LoadAll<Sprite>(path);
            if (sprites == null || sprites.Length == 0)
            {
                return System.Array.Empty<Sprite>();
            }

            System.Array.Sort(sprites, (a, b) => string.CompareOrdinal(a.name, b.name));
            return sprites;
        }
    }
}
