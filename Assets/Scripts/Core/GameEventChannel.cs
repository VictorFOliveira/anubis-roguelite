using System;
using UnityEngine;

namespace Anubis.Core
{
    [CreateAssetMenu(menuName = "Anubis/Events/Void Channel", fileName = "VoidEvent")]
    public class GameEventChannel : ScriptableObject, IGameEvent
    {
        event Action Raised;

        public void Raise()
        {
            Raised?.Invoke();
        }

        public void Subscribe(Action listener)
        {
            Raised += listener;
        }

        public void Unsubscribe(Action listener)
        {
            Raised -= listener;
        }

        void OnDisable()
        {
            Raised = null;
        }
    }

    public abstract class GameEventChannel<T> : ScriptableObject, IGameEvent<T>
    {
        event Action<T> Raised;

        public void Raise(T payload)
        {
            Raised?.Invoke(payload);
        }

        public void Subscribe(Action<T> listener)
        {
            Raised += listener;
        }

        public void Unsubscribe(Action<T> listener)
        {
            Raised -= listener;
        }

        void OnDisable()
        {
            Raised = null;
        }
    }
}
