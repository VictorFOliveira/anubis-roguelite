using System;

namespace Anubis.Core
{
    public interface IGameEvent
    {
        void Raise();
        void Subscribe(Action listener);
        void Unsubscribe(Action listener);
    }

    public interface IGameEvent<T>
    {
        void Raise(T payload);
        void Subscribe(Action<T> listener);
        void Unsubscribe(Action<T> listener);
    }
}
