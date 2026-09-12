using System.Collections.Generic;
using Anubis.Combat;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Characters
{
    public sealed class KhopeshAttack : MonoBehaviour, IPrimaryAttack
    {
        static readonly Collider2D[] Hits = new Collider2D[24];

        CharacterDefinition _definition;
        TopDownMotor _motor;
        float _cooldown;
        float _activeTimer;
        float _elapsed;
        readonly HashSet<GameObject> _alreadyHit = new();

        public bool IsAttacking => _activeTimer > 0f;
        public float DamageMultiplier { get; set; } = 1f;

        public void Configure(CharacterDefinition definition, TopDownMotor motor, ObjectPool pool)
        {
            _definition = definition;
            _motor = motor;
        }

        public bool TryAttack(Vector2 aim)
        {
            if (IsAttacking || _cooldown > 0f || _definition == null)
            {
                return false;
            }

            _motor.Face(aim);
            _activeTimer = _definition.AttackDuration;
            _elapsed = 0f;
            _cooldown = _definition.AttackCooldown;
            _alreadyHit.Clear();
            return true;
        }

        void Update()
        {
            if (_cooldown > 0f)
            {
                _cooldown -= Time.deltaTime;
            }

            if (!IsAttacking)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            _activeTimer -= Time.deltaTime;

            var normalized = _definition.AttackDuration <= 0f ? 1f : _elapsed / _definition.AttackDuration;
            if (normalized < 0.38f || normalized > 0.82f)
            {
                return;
            }

            StrikeArc();
        }

        void StrikeArc()
        {
            var origin = (Vector2)transform.position;
            var facing = _motor.Facing.sqrMagnitude > 0.01f ? _motor.Facing.normalized : Vector2.right;
            var range = _definition.AttackRange;
            var halfAngle = _definition.AttackArcDegrees * 0.5f;
            var count = Physics2D.OverlapCircleNonAlloc(origin, range, Hits, GameLayers.EnemyMask);

            for (var i = 0; i < count; i++)
            {
                var collider = Hits[i];
                if (collider == null)
                {
                    continue;
                }

                var target = (Vector2)collider.transform.position;
                var toTarget = target - origin;
                var distance = toTarget.magnitude;
                if (distance < 0.05f || distance > range)
                {
                    continue;
                }

                if (Vector2.Angle(facing, toTarget) > halfAngle)
                {
                    continue;
                }

                var damageable = collider.GetComponent<IDamageable>() ?? collider.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive)
                {
                    continue;
                }

                var targetObject = damageable is Component component ? component.gameObject : collider.gameObject;
                if (_alreadyHit.Contains(targetObject))
                {
                    continue;
                }

                _alreadyHit.Add(targetObject);
                damageable.TryApplyDamage(new DamageInfo(
                    _definition.AttackDamage * DamageMultiplier,
                    origin,
                    toTarget,
                    _definition.AttackKnockback,
                    TeamId.Player));
            }
        }
    }
}
