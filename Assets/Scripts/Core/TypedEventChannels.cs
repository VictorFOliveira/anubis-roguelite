using Anubis.Combat;
using UnityEngine;

namespace Anubis.Core
{
    [CreateAssetMenu(menuName = "Anubis/Events/Damage Channel", fileName = "DamageDealtEvent")]
    public class DamageEventChannel : GameEventChannel<DamageInfo> { }

    [CreateAssetMenu(menuName = "Anubis/Events/Health Channel", fileName = "HealthChangedEvent")]
    public class HealthEventChannel : GameEventChannel<HealthChangeInfo> { }

    [CreateAssetMenu(menuName = "Anubis/Events/Enemy Channel", fileName = "EnemyKilledEvent")]
    public class EnemyEventChannel : GameEventChannel<EnemyKillInfo> { }

    [CreateAssetMenu(menuName = "Anubis/Events/Blessing Channel", fileName = "BlessingChosenEvent")]
    public class BlessingEventChannel : GameEventChannel<string> { }

    [CreateAssetMenu(menuName = "Anubis/Events/Room Channel", fileName = "RoomEvent")]
    public class RoomEventChannel : GameEventChannel<string> { }

    public readonly struct HealthChangeInfo
    {
        public readonly float Current;
        public readonly float Max;
        public readonly TeamId Team;

        public HealthChangeInfo(float current, float max, TeamId team)
        {
            Current = current;
            Max = max;
            Team = team;
        }
    }

    public readonly struct EnemyKillInfo
    {
        public readonly string EnemyId;
        public readonly Vector2 Position;

        public EnemyKillInfo(string enemyId, Vector2 position)
        {
            EnemyId = enemyId;
            Position = position;
        }
    }
}
