using UnityEngine;

namespace Anubis.AI
{
    [CreateAssetMenu(menuName = "Anubis/Enemies/Enemy Definition", fileName = "Enemy")]
    public class EnemyDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string Id = "scarab";
        public string DisplayName = "Escaravelho";
        public EnemyArchetype Archetype = EnemyArchetype.Rusher;

        [Header("Presentation")]
        public Color BodyColor = new(0.18f, 0.42f, 0.16f, 1f);
        public float VisualScale = 0.7f;

        [Header("Stats")]
        public float MaxHealth = 22f;
        public float MoveSpeed = 4.2f;
        public float Acceleration = 28f;
        public float Damage = 8f;
        public float AttackRange = 0.7f;
        public float AttackCooldown = 0.7f;
        public float AttackWindup = 0.18f;
        public float Knockback = 3.5f;
        public float DetectRange = 12f;
        public float PreferredDistance = 0.5f;
        public float ColliderRadius = 0.28f;

        [Header("Ranged")]
        public float ProjectileSpeed = 7.5f;
        public float ProjectileLifetime = 2.4f;
    }
}
