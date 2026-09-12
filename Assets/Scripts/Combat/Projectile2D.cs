using Anubis.Core;
using UnityEngine;

namespace Anubis.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Projectile2D : MonoBehaviour, IPoolable
    {
        public const string EnemyPoolKey = "enemy_projectile";
        public const string SlashPoolKey = "slash_vfx";

        [SerializeField] float lifetime = 3f;
        [SerializeField] float radius = 0.18f;

        ObjectPool _pool;
        Rigidbody2D _body;
        SpriteRenderer _renderer;
        Vector2 _velocity;
        float _damage;
        float _knockback;
        float _age;
        TeamId _team;
        LayerMask _mask;
        bool _alive;

        public void Initialize(ObjectPool pool)
        {
            _pool = pool;
            EnsureComponents();
        }

        void EnsureComponents()
        {
            _pool ??= ObjectPool.Current;
            _body ??= GetComponent<Rigidbody2D>();
            _renderer ??= GetComponent<SpriteRenderer>();
            if (_body == null)
            {
                return;
            }

            _body.bodyType = RigidbodyType2D.Kinematic;
            _body.gravityScale = 0f;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void Launch(Vector2 position, Vector2 direction, float speed, float damage, float knockback, TeamId team, LayerMask mask, Color color)
        {
            transform.position = position;
            _velocity = direction.normalized * speed;
            _damage = damage;
            _knockback = knockback;
            _team = team;
            _mask = mask;
            _age = 0f;
            _alive = true;
            if (_renderer != null)
            {
                _renderer.color = color;
            }
        }

        public void OnSpawnFromPool()
        {
            EnsureComponents();
            _alive = true;
            _age = 0f;
        }

        public void OnReturnToPool()
        {
            _alive = false;
            _velocity = Vector2.zero;
        }

        void Update()
        {
            if (!_alive)
            {
                return;
            }

            _age += Time.deltaTime;
            if (_age >= lifetime)
            {
                Despawn();
            }
        }

        void FixedUpdate()
        {
            if (!_alive)
            {
                return;
            }

            var next = _body.position + _velocity * Time.fixedDeltaTime;
            var hit = Physics2D.OverlapCircle(next, radius, _mask);
            if (hit != null)
            {
                var damageable = hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>();
                if (damageable != null && damageable.Team != _team)
                {
                    damageable.TryApplyDamage(new DamageInfo(_damage, _body.position, _velocity, _knockback, _team));
                }

                Despawn();
                return;
            }

            _body.MovePosition(next);
        }

        void Despawn()
        {
            if (!_alive)
            {
                return;
            }

            _alive = false;
            _pool?.Return(gameObject);
        }
    }
}
