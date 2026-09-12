using UnityEngine;

namespace Anubis.AI
{
    public sealed class EnemyKeepDistanceState : IState
    {
        readonly EnemyController _owner;
        public string Id => "keep_distance";

        public EnemyKeepDistanceState(EnemyController owner)
        {
            _owner = owner;
        }

        public void Enter() { }

        public void Tick(float deltaTime)
        {
            if (!_owner.TargetAlive)
            {
                _owner.Machine.ChangeState(_owner.Idle);
                return;
            }

            var distance = _owner.ToTarget.magnitude;
            var preferred = _owner.Definition.PreferredDistance;
            if (distance <= _owner.Definition.AttackRange && _owner.CanAttack)
            {
                _owner.Machine.ChangeState(_owner.Attack);
                return;
            }

            if (distance < preferred - 0.6f)
            {
                _owner.Motor.SetDesiredVelocity(-_owner.ToTarget.normalized);
            }
            else if (distance > preferred + 0.8f)
            {
                _owner.Motor.SetDesiredVelocity(_owner.ToTarget.normalized);
            }
            else
            {
                var tangent = Vector2.Perpendicular(_owner.ToTarget).normalized;
                _owner.Motor.SetDesiredVelocity(tangent * 0.45f);
            }

            _owner.Motor.Face(_owner.ToTarget);
        }

        public void Exit()
        {
            _owner.Motor.SetDesiredVelocity(Vector2.zero);
        }
    }
}
