using UnityEngine;

namespace Anubis.AI
{
    public sealed class EnemyIdleState : IState
    {
        readonly EnemyController _owner;
        public string Id => "idle";

        public EnemyIdleState(EnemyController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
            _owner.Motor.SetDesiredVelocity(Vector2.zero);
        }

        public void Tick(float deltaTime)
        {
            if (_owner.HasTargetInDetectRange)
            {
                _owner.Machine.ChangeState(_owner.ResolveCombatState());
            }
        }

        public void Exit() { }
    }
}
