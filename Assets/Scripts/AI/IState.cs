namespace Anubis.AI
{
    public interface IState
    {
        string Id { get; }
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
