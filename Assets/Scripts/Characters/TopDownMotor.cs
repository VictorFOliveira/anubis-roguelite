using Anubis.Combat;
using UnityEngine;

namespace Anubis.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class TopDownMotor : MonoBehaviour
    {
        Rigidbody2D _body;
        KnockbackBody _knockback;
        float _speed;
        float _acceleration;
        float _deceleration;
        Vector2 _desired;
        bool _locked;

        public Vector2 Velocity { get; private set; }
        public Vector2 Facing { get; private set; } = Vector2.right;

        public void Configure(Rigidbody2D body, float speed, float acceleration, float deceleration)
        {
            _body = body;
            _speed = speed;
            _acceleration = acceleration;
            _deceleration = deceleration;
            _knockback = GetComponent<KnockbackBody>();
            _knockback?.Initialize(body);
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public void SetDesiredVelocity(Vector2 input)
        {
            _desired = Vector2.ClampMagnitude(input, 1f);
            if (_desired.sqrMagnitude > 0.01f)
            {
                Facing = _desired.normalized;
            }
        }

        public void Face(Vector2 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                Facing = direction.normalized;
            }
        }

        public void SetLocked(bool locked)
        {
            _locked = locked;
            if (locked)
            {
                _desired = Vector2.zero;
            }
        }

        void FixedUpdate()
        {
            if (_body == null || _locked)
            {
                return;
            }

            var target = _desired * _speed;
            var rate = target.sqrMagnitude > Velocity.sqrMagnitude ? _acceleration : _deceleration;
            Velocity = Vector2.MoveTowards(Velocity, target, rate * Time.fixedDeltaTime);

            var knockback = _knockback != null ? _knockback.Consume() : Vector2.zero;
            _body.linearVelocity = Velocity + knockback;
        }
    }
}
