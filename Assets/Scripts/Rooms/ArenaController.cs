using Anubis.Characters;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Rooms
{
    public sealed class ArenaController : MonoBehaviour
    {
        ModularRoom _room;
        EncounterDirector _encounter;
        RewardShrine _shrine;
        GameSignals _signals;

        public ModularRoom Room => _room;
        public EncounterDirector Encounter => _encounter;

        public void Bind(RoomDefinition definition, PlayerController player, GameSignals signals, ObjectPool pool, GameInputReader input)
        {
            _signals = signals;
            _room = gameObject.AddComponent<ModularRoom>();
            _room.Build(definition);

            _encounter = gameObject.AddComponent<EncounterDirector>();
            _encounter.Configure(definition, player, signals, pool);

            var shrineObject = new GameObject("RewardShrine");
            shrineObject.transform.SetParent(transform, false);
            _shrine = shrineObject.AddComponent<RewardShrine>();
            _shrine.Bind(signals, input, player.transform);

            signals.EncounterCleared.Subscribe(OnCleared);
            signals.EncounterStarted.Subscribe(OnStarted);
        }

        public void Begin()
        {
            _room.SetDoorsOpen(false);
            _encounter.StartEncounter();
            _signals.RoomStateChanged.Raise("combat");
        }

        void OnStarted()
        {
            _room.SetDoorsOpen(false);
        }

        void OnCleared()
        {
            _room.SetDoorsOpen(true);
            _shrine.Reveal(Vector2.zero);
            _signals.RoomStateChanged.Raise("reward");
        }

        void OnDestroy()
        {
            if (_signals == null)
            {
                return;
            }

            _signals.EncounterCleared.Unsubscribe(OnCleared);
            _signals.EncounterStarted.Unsubscribe(OnStarted);
        }
    }
}
