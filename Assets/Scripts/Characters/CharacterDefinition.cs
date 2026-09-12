using UnityEngine;

namespace Anubis.Characters
{
    [CreateAssetMenu(menuName = "Anubis/Characters/Character Definition", fileName = "Character")]
    public class CharacterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string Id = "anubis";
        public string DisplayName = "Anúbis";
        [TextArea] public string Description = "Guardião das almas. Combate próximo com o Khopesh e dash pelas areias.";

        [Header("Presentation")]
        public Color BodyColor = new(0.12f, 0.08f, 0.08f, 1f);
        public Color AccentColor = new(0.85f, 0.67f, 0.18f, 1f);
        public float VisualScale = 1.05f;

        [Header("Movement")]
        public float MoveSpeed = 5.4f;
        public float Acceleration = 48f;
        public float Deceleration = 56f;

        [Header("Vitality")]
        public float MaxHealth = 100f;
        public float HitInvulnerability = 0.45f;

        [Header("Dash")]
        public float DashDistance = 3.4f;
        public float DashDuration = 0.14f;
        public float DashCooldown = 0.85f;
        public float DashIFrames = 0.14f;

        [Header("Khopesh")]
        public float AttackDamage = 18f;
        public float AttackRange = 2.1f;
        public float AttackRadius = 0.85f;
        public float AttackArcDegrees = 140f;
        public float AttackDuration = 0.36f;
        public float AttackCooldown = 0.28f;
        public float AttackKnockback = 5.5f;
    }
}
