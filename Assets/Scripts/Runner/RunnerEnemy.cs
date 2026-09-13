using Anubis.Core;
using UnityEngine;

namespace Anubis.Runner
{
    public enum RunnerEnemyStyle
    {
        Scarab,
        Guard
    }

    public sealed class RunnerEnemy : MonoBehaviour
    {
        Rigidbody2D _body;
        float _speed;
        bool _dead;

        public void Configure(RunnerEnemyStyle style)
        {
            gameObject.layer = GameLayers.Enemy;
            _speed = style == RunnerEnemyStyle.Scarab ? 1.6f : 0.85f;

            _body = gameObject.AddComponent<Rigidbody2D>();
            _body.bodyType = RigidbodyType2D.Kinematic;
            _body.gravityScale = 0f;

            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = style == RunnerEnemyStyle.Scarab ? new Vector2(0.85f, 0.62f) : new Vector2(0.78f, 1.45f);

            var visual = new GameObject("Visual");
            visual.transform.SetParent(transform, false);
            var renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = GameSorting.Entities;

            if (style == RunnerEnemyStyle.Scarab)
            {
                renderer.sprite = RuntimeSpriteFactory.CreateCircle(new Color(0.11f, 0.18f, 0.17f), 40, 0.88f);
                visual.transform.localScale = new Vector3(0.95f, 0.6f, 1f);
            }
            else
            {
                renderer.sprite = RuntimeSpriteFactory.CreateRect(new Color(0.37f, 0.18f, 0.12f), 36, 74);
                visual.transform.localScale = new Vector3(0.75f, 1.4f, 1f);
            }
        }

        void Update()
        {
            if (_dead)
            {
                return;
            }

            transform.position += Vector3.left * (_speed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 8f) * 2.5f);
        }

        public void Stomp()
        {
            if (_dead)
            {
                return;
            }

            _dead = true;
            foreach (var collider in GetComponents<Collider2D>()) collider.enabled = false;
            transform.localScale = new Vector3(1.15f, 0.25f, 1f);
            Destroy(gameObject, 0.18f);
        }
    }
}
