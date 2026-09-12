using UnityEngine;

namespace Anubis.AI
{
    public sealed class EnemyDeadState : IState
    {
        readonly EnemyController _owner;
        public string Id => "dead";

        public EnemyDeadState(EnemyController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
            _owner.Motor.SetDesiredVelocity(Vector2.zero);
            _owner.Motor.SetLocked(true);
            foreach (var collider in _owner.GetComponents<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}
