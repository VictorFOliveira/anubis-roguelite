namespace Anubis.AI
{
    public sealed class EnemyChaseState : IState
    {
        readonly EnemyController _owner;
        public string Id => "chase";

        public EnemyChaseState(EnemyController owner)
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

            var toTarget = _owner.ToTarget;
            if (toTarget.magnitude <= _owner.Definition.AttackRange)
            {
                _owner.Machine.ChangeState(_owner.Attack);
                return;
            }

            _owner.Motor.SetDesiredVelocity(toTarget.normalized);
            _owner.Motor.Face(toTarget);
        }

        public void Exit()
        {
            _owner.Motor.SetDesiredVelocity(UnityEngine.Vector2.zero);
        }
    }
}
