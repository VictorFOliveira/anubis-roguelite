using Anubis.AI;
using Anubis.Characters;
using Anubis.Progression;
using Anubis.Rooms;
using UnityEngine;

namespace Anubis.Core
{
    public static class VerticalSliceCatalog
    {
        public static CharacterDefinition CreateAnubis()
        {
            var definition = ScriptableObject.CreateInstance<CharacterDefinition>();
            definition.Id = "anubis";
            definition.DisplayName = "Anúbis";
            definition.Description = "Guardião das almas. Khopesh no curto alcance e dash pelas areias.";
            definition.BodyColor = new Color(0.1f, 0.08f, 0.09f);
            definition.AccentColor = new Color(0.86f, 0.68f, 0.18f);
            definition.VisualScale = 1f;
            definition.MoveSpeed = 5.4f;
            definition.MaxHealth = 100f;
            definition.DashDistance = 3.4f;
            definition.DashDuration = 0.14f;
            definition.DashCooldown = 0.85f;
            definition.AttackDamage = 18f;
            definition.AttackRange = 2.1f;
            definition.AttackRadius = 0.85f;
            definition.AttackArcDegrees = 140f;
            definition.AttackDuration = 0.36f;
            definition.AttackCooldown = 0.28f;
            return definition;
        }

        public static EnemyDefinition CreateScarab()
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            definition.Id = "scarab";
            definition.DisplayName = "Escaravelho";
            definition.Archetype = EnemyArchetype.Rusher;
            definition.BodyColor = new Color(0.2f, 0.45f, 0.18f);
            definition.VisualScale = 0.62f;
            definition.MaxHealth = 20f;
            definition.MoveSpeed = 4.4f;
            definition.Damage = 8f;
            definition.AttackRange = 0.65f;
            definition.AttackCooldown = 0.65f;
            definition.AttackWindup = 0.12f;
            definition.ColliderRadius = 0.24f;
            return definition;
        }

        public static EnemyDefinition CreateGuardian()
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            definition.Id = "tomb_guardian";
            definition.DisplayName = "Guardião da Tumba";
            definition.Archetype = EnemyArchetype.Bruiser;
            definition.BodyColor = new Color(0.62f, 0.5f, 0.28f);
            definition.VisualScale = 1.15f;
            definition.MaxHealth = 58f;
            definition.MoveSpeed = 2.3f;
            definition.Damage = 16f;
            definition.AttackRange = 1.05f;
            definition.AttackCooldown = 1.1f;
            definition.AttackWindup = 0.28f;
            definition.Knockback = 6f;
            definition.ColliderRadius = 0.4f;
            return definition;
        }

        public static EnemyDefinition CreateArcher()
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            definition.Id = "jackal_archer";
            definition.DisplayName = "Arqueiro Chacal";
            definition.Archetype = EnemyArchetype.Ranged;
            definition.BodyColor = new Color(0.55f, 0.18f, 0.16f);
            definition.VisualScale = 0.85f;
            definition.MaxHealth = 28f;
            definition.MoveSpeed = 3.3f;
            definition.Damage = 10f;
            definition.AttackRange = 6.2f;
            definition.PreferredDistance = 5.2f;
            definition.AttackCooldown = 1.25f;
            definition.AttackWindup = 0.22f;
            definition.ProjectileSpeed = 8f;
            definition.ColliderRadius = 0.3f;
            return definition;
        }

        public static BlessingDefinition[] CreateBlessings()
        {
            return new[]
            {
                CreateBlessing("blade_of_ra", "Lâmina de Rá", "+25% de dano com o Khopesh.", BlessingKind.AttackDamage, 0.25f, new Color(0.95f, 0.72f, 0.2f)),
                CreateBlessing("sandstep", "Passo das Areias", "+35% de distância no dash e recarga mais rápida.", BlessingKind.DashDistance, 0.35f, new Color(0.82f, 0.7f, 0.38f)),
                CreateBlessing("ankh_vital", "Ankh Vital", "+25 de vida máxima, curado imediatamente.", BlessingKind.MaxHealth, 25f, new Color(0.35f, 0.78f, 0.55f)),
                CreateBlessing("ankh_pulse", "Pulso de Ankh", "Cura 8 de vida ao eliminar um inimigo.", BlessingKind.HealOnKill, 8f, new Color(0.45f, 0.82f, 0.86f))
            };
        }

        public static RoomDefinition CreateArena(EnemyDefinition scarab, EnemyDefinition guardian, EnemyDefinition archer)
        {
            var room = ScriptableObject.CreateInstance<RoomDefinition>();
            room.Id = "anubis_arena";
            room.DisplayName = "Arena de Anúbis";
            room.Size = new Vector2(18f, 12f);
            room.LockDoorsOnEnter = true;
            room.Waves = new[]
            {
                new EncounterWave
                {
                    Label = "Onda 1",
                    Spawns = new[]
                    {
                        new EnemySpawn { Enemy = scarab, LocalPosition = new Vector2(-2.5f, 2.2f) },
                        new EnemySpawn { Enemy = scarab, LocalPosition = new Vector2(2.5f, 2.4f) },
                        new EnemySpawn { Enemy = scarab, LocalPosition = new Vector2(0f, 3.4f) }
                    }
                },
                new EncounterWave
                {
                    Label = "Onda 2",
                    Spawns = new[]
                    {
                        new EnemySpawn { Enemy = guardian, LocalPosition = new Vector2(0f, 2.8f) },
                        new EnemySpawn { Enemy = archer, LocalPosition = new Vector2(4.5f, 3.6f) },
                        new EnemySpawn { Enemy = scarab, LocalPosition = new Vector2(-4.2f, 2.6f) }
                    }
                }
            };
            return room;
        }

        static BlessingDefinition CreateBlessing(string id, string name, string description, BlessingKind kind, float value, Color accent)
        {
            var definition = ScriptableObject.CreateInstance<BlessingDefinition>();
            definition.Id = id;
            definition.DisplayName = name;
            definition.Description = description;
            definition.Kind = kind;
            definition.Value = value;
            definition.Accent = accent;
            return definition;
        }
    }
}
