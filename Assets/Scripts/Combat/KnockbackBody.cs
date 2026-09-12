using UnityEngine;

namespace Anubis.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class KnockbackBody : MonoBehaviour
    {
        [SerializeField] float damping = 12f;
        Vector2 _velocity;
        Rigidbody2D _body;

        public void Initialize(Rigidbody2D body)
        {
            _body = body;
        }

        public void Apply(Vector2 direction, float force)
        {
            if (force <= 0f)
            {
                return;
            }

            _velocity = direction.normalized * force;
        }

        public Vector2 Consume()
        {
            if (_velocity.sqrMagnitude < 0.01f)
            {
                _velocity = Vector2.zero;
                return Vector2.zero;
            }

            var current = _velocity;
            _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, damping * Time.deltaTime);
            return current;
        }

        void Awake()
        {
            _body ??= GetComponent<Rigidbody2D>();
        }
    }
}
