using Anubis.Combat;
using UnityEngine;

namespace Anubis.Characters
{
    public sealed class DashController : MonoBehaviour
    {
        CharacterDefinition _definition;
        TopDownMotor _motor;
        Health _health;
        Rigidbody2D _body;
        float _cooldown;
        float _dashTimer;
        Vector2 _dashVelocity;
        bool _dashing;

        public bool IsDashing => _dashing;
        public float CooldownNormalized => _definition == null ? 0f : 1f - Mathf.Clamp01(_cooldown / Mathf.Max(0.01f, EffectiveCooldown));

        public float DistanceMultiplier { get; set; } = 1f;
        public float CooldownMultiplier { get; set; } = 1f;

        float EffectiveCooldown => _definition.DashCooldown * CooldownMultiplier;

        public void Configure(CharacterDefinition definition, TopDownMotor motor, Health health, Rigidbody2D body)
        {
            _definition = definition;
            _motor = motor;
            _health = health;
            _body = body;
        }

        public bool TryDash(Vector2 fallbackDirection)
        {
            if (_dashing || _cooldown > 0f || _definition == null)
            {
                return false;
            }

            var direction = fallbackDirection.sqrMagnitude > 0.01f ? fallbackDirection.normalized : _motor.Facing;
            var distance = _definition.DashDistance * DistanceMultiplier;
            _dashVelocity = direction * (distance / Mathf.Max(0.01f, _definition.DashDuration));
            _dashTimer = _definition.DashDuration;
            _cooldown = EffectiveCooldown;
            _dashing = true;
            _motor.SetLocked(true);
            _motor.Face(direction);
            _health.SetInvulnerable(true);
            return true;
        }

        void Update()
        {
            if (_cooldown > 0f)
            {
                _cooldown -= Time.deltaTime;
            }
        }

        void FixedUpdate()
        {
            if (!_dashing)
            {
                return;
            }

            _dashTimer -= Time.fixedDeltaTime;
            _body.linearVelocity = _dashVelocity;
            if (_dashTimer <= 0f)
            {
                _dashing = false;
                _motor.SetLocked(false);
                _health.SetInvulnerable(false);
                _body.linearVelocity = Vector2.zero;
            }
        }
    }
}
