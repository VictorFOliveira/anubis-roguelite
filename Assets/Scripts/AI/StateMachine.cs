using UnityEngine;

namespace Anubis.AI
{
    public sealed class StateMachine
    {
        public IState Current { get; private set; }

        public void ChangeState(IState next)
        {
            if (next == null || ReferenceEquals(Current, next))
            {
                return;
            }

            Current?.Exit();
            Current = next;
            Current.Enter();
        }

        public void Tick(float deltaTime)
        {
            Current?.Tick(deltaTime);
        }
    }
}
