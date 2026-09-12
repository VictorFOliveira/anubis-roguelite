using Anubis.Core;
using UnityEngine;

namespace Anubis.Combat
{
    public sealed class PooledVfx : MonoBehaviour, IPoolable
    {
        [SerializeField] float lifetime = 0.18f;
        ObjectPool _pool;
        SpriteRenderer _renderer;
        float _age;

        public void Initialize(ObjectPool pool)
        {
            _pool = pool;
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void OnSpawnFromPool()
        {
            _pool ??= ObjectPool.Current;
            _renderer ??= GetComponent<SpriteRenderer>();
            _age = 0f;
        }

        public void Play(Vector2 position, Vector2 direction, Color color, float scale = 1f)
        {
            transform.position = position;
            transform.right = direction.sqrMagnitude > 0.01f ? (Vector3)direction : Vector3.right;
            transform.localScale = Vector3.one * scale;
            if (_renderer != null)
            {
                _renderer.color = color;
            }

            _age = 0f;
        }

        public void OnReturnToPool() { }

        void Update()
        {
            _age += Time.deltaTime;
            if (_renderer != null)
            {
                var color = _renderer.color;
                color.a = Mathf.Lerp(1f, 0f, _age / lifetime);
                _renderer.color = color;
            }

            if (_age >= lifetime)
            {
                _pool?.Return(gameObject);
            }
        }
    }
}
