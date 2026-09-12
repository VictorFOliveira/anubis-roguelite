using UnityEngine;

namespace Anubis.Combat
{
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly Vector2 Origin;
        public readonly Vector2 Direction;
        public readonly float Knockback;
        public readonly TeamId SourceTeam;
        public readonly bool IsCritical;

        public DamageInfo(float amount, Vector2 origin, Vector2 direction, float knockback, TeamId sourceTeam, bool isCritical = false)
        {
            Amount = amount;
            Origin = origin;
            Direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.zero;
            Knockback = knockback;
            SourceTeam = sourceTeam;
            IsCritical = isCritical;
        }
    }
}
