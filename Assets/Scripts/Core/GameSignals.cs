using UnityEngine;

namespace Anubis.Core
{
    [CreateAssetMenu(menuName = "Anubis/Events/Game Signals", fileName = "GameSignals")]
    public class GameSignals : ScriptableObject
    {
        public GameEventChannel RunStarted;
        public GameEventChannel EncounterStarted;
        public GameEventChannel EncounterCleared;
        public GameEventChannel RewardReady;
        public GameEventChannel BlessingOfferOpened;
        public GameEventChannel PlayerDied;
        public GameEventChannel SliceCompleted;

        public DamageEventChannel DamageDealt;
        public HealthEventChannel PlayerHealthChanged;
        public EnemyEventChannel EnemyKilled;
        public BlessingEventChannel BlessingChosen;
        public RoomEventChannel RoomStateChanged;

        public static GameSignals CreateRuntime()
        {
            var signals = CreateInstance<GameSignals>();
            signals.RunStarted = CreateInstance<GameEventChannel>();
            signals.EncounterStarted = CreateInstance<GameEventChannel>();
            signals.EncounterCleared = CreateInstance<GameEventChannel>();
            signals.RewardReady = CreateInstance<GameEventChannel>();
            signals.BlessingOfferOpened = CreateInstance<GameEventChannel>();
            signals.PlayerDied = CreateInstance<GameEventChannel>();
            signals.SliceCompleted = CreateInstance<GameEventChannel>();
            signals.DamageDealt = CreateInstance<DamageEventChannel>();
            signals.PlayerHealthChanged = CreateInstance<HealthEventChannel>();
            signals.EnemyKilled = CreateInstance<EnemyEventChannel>();
            signals.BlessingChosen = CreateInstance<BlessingEventChannel>();
            signals.RoomStateChanged = CreateInstance<RoomEventChannel>();
            return signals;
        }
    }
}
