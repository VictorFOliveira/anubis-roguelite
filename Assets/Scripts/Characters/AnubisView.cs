using Anubis.Core;
using UnityEngine;

namespace Anubis.Characters
{
    public sealed class AnubisView : MonoBehaviour
    {
        const string IdlePath = "Characters/Anubis";
        const string AttackPath = "Characters/AnubisAttack";

        Transform _root;
        SpriteRenderer _body;
        SpriteRenderer _shadow;
        Sprite _idle;
        Sprite[] _attackFrames = System.Array.Empty<Sprite>();
        Vector3 _baseScale = Vector3.one;
        bool _flip;
        bool _playingAttack;
        float _attackTime;
        float _attackDuration = 0.32f;

        public SpriteRenderer Body => _body;

        public void Build(CharacterDefinition definition)
        {
            _root = new GameObject("View").transform;
            _root.SetParent(transform, false);
            _attackDuration = Mathf.Max(0.22f, definition.AttackDuration);

            _shadow = CreateChild("Shadow", _root, GameSorting.Entities - 1);
            _shadow.sprite = RuntimeSpriteFactory.CreateCircle(new Color(0f, 0f, 0f, 0.35f), 64, 0.95f);
            _shadow.transform.localPosition = new Vector3(0f, -0.02f, 0f);
            _shadow.transform.localScale = new Vector3(0.85f, 0.28f, 1f);
            _shadow.color = new Color(0f, 0f, 0f, 0.4f);

            _body = CreateChild("Body", _root, GameSorting.Entities);
            _idle = LoadIdle();
            _attackFrames = LoadAttackFrames();
            ApplyFrame(_idle);
            _body.spriteSortPoint = SpriteSortPoint.Pivot;

            var height = _body.sprite != null ? _body.sprite.bounds.size.y : 1.8f;
            var targetHeight = 2.15f * definition.VisualScale;
            var scale = targetHeight / Mathf.Max(0.2f, height);
            _baseScale = Vector3.one * scale;
            _root.localScale = _baseScale;
        }

        public void PlayAttack()
        {
            if (_attackFrames.Length == 0)
            {
                return;
            }

            _playingAttack = true;
            _attackTime = 0f;
            ApplyFrame(_attackFrames[0]);
        }

        public void SetFacing(Vector2 facing)
        {
            if (Mathf.Abs(facing.x) > 0.05f)
            {
                _flip = facing.x < 0f;
            }

            if (_body != null)
            {
                _body.flipX = _flip;
            }
        }

        public void SetDashing(bool dashing)
        {
            if (_root == null)
            {
                return;
            }

            var squash = dashing ? new Vector3(1.15f, 0.82f, 1f) : Vector3.one;
            _root.localScale = Vector3.Scale(_baseScale, squash);
        }

        void Update()
        {
            if (_root == null || _body == null)
            {
                return;
            }

            if (_playingAttack)
            {
                _root.localPosition = Vector3.zero;
                _attackTime += Time.deltaTime;
                var t = Mathf.Clamp01(_attackTime / _attackDuration);
                var index = Mathf.Min(_attackFrames.Length - 1, Mathf.FloorToInt(t * _attackFrames.Length));
                ApplyFrame(_attackFrames[index]);
                if (_attackTime >= _attackDuration)
                {
                    _playingAttack = false;
                    ApplyFrame(_idle);
                }

                return;
            }

            var bob = Mathf.Sin(Time.time * 3.4f) * 0.035f;
            _root.localPosition = new Vector3(0f, bob, 0f);
        }

        void ApplyFrame(Sprite sprite)
        {
            if (_body == null || sprite == null)
            {
                return;
            }

            _body.sprite = sprite;
            if (_idle == null)
            {
                return;
            }

            var idleHeight = Mathf.Max(0.01f, _idle.bounds.size.y);
            var frameHeight = Mathf.Max(0.01f, sprite.bounds.size.y);
            var match = idleHeight / frameHeight;
            _body.transform.localScale = new Vector3(match, match, 1f);
        }

        static SpriteRenderer CreateChild(string name, Transform parent, int order)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent, false);
            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = order;
            return renderer;
        }

        static Sprite LoadIdle()
        {
            return LoadFirst(IdlePath) ?? RuntimeSpriteFactory.CreateCircle(new Color(0.08f, 0.06f, 0.07f));
        }

        static Sprite[] LoadAttackFrames()
        {
            var loaded = Resources.LoadAll<Sprite>(AttackPath);
            if (loaded == null || loaded.Length == 0)
            {
                return System.Array.Empty<Sprite>();
            }

            System.Array.Sort(loaded, (a, b) => string.CompareOrdinal(a.name, b.name));
            return loaded;
        }

        static Sprite LoadFirst(string path)
        {
            var sprites = Resources.LoadAll<Sprite>(path);
            if (sprites != null && sprites.Length > 0)
            {
                return sprites[0];
            }

            var single = Resources.Load<Sprite>(path);
            if (single != null)
            {
                return single;
            }

            var texture = Resources.Load<Texture2D>(path);
            return texture == null
                ? null
                : Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.07f), 420f);
        }
    }
}
