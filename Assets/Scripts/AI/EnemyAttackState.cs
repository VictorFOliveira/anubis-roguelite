using Anubis.Combat;
using Anubis.Core;
using UnityEngine;

namespace Anubis.AI
{
    public sealed class EnemyAttackState : IState
    {
        readonly EnemyController _owner;
        float _windup;
        bool _fired;
        public string Id => "attack";

        public EnemyAttackState(EnemyController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
            _windup = _owner.Definition.AttackWindup;
            _fired = false;
            _owner.Motor.SetDesiredVelocity(Vector2.zero);
            _owner.Motor.Face(_owner.ToTarget);
        }

        public void Tick(float deltaTime)
        {
            _windup -= deltaTime;
            if (_fired || _windup > 0f)
            {
                if (_fired && _windup <= -0.08f)
                {
                    _owner.Machine.ChangeState(_owner.ResolveCombatState());
                }

                return;
            }

            _fired = true;
            _owner.NotifyAttackUsed();
            if (_owner.Definition.Archetype == EnemyArchetype.Ranged)
            {
                FireProjectile();
            }
            else
            {
                MeleeHit();
            }
        }

        public void Exit() { }

        void MeleeHit()
        {
            if (!_owner.TargetAlive)
            {
                return;
            }

            if (_owner.ToTarget.magnitude > _owner.Definition.AttackRange + 0.25f)
            {
                return;
            }

            var origin = (Vector2)_owner.transform.position;
            var direction = _owner.ToTarget.normalized;
            _owner.TargetHealth.TryApplyDamage(new DamageInfo(
                _owner.Definition.Damage,
                origin,
                direction,
                _owner.Definition.Knockback,
                TeamId.Enemy));
        }

        void FireProjectile()
        {
            var direction = _owner.ToTarget.normalized;
            var spawn = (Vector2)_owner.transform.position + direction * 0.45f;
            var projectile = _owner.Pool.Get<Projectile2D>(Projectile2D.EnemyPoolKey, spawn, Quaternion.identity);
            projectile.Launch(
                spawn,
                direction,
                _owner.Definition.ProjectileSpeed,
                _owner.Definition.Damage,
                _owner.Definition.Knockback * 0.6f,
                TeamId.Enemy,
                GameLayers.PlayerMask,
                new Color(0.85f, 0.25f, 0.18f));
        }
    }
}
