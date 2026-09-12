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
            definition.PrimaryAttackId = CharacterRuntimeIds.KhopeshAttack;
            definition.ViewId = CharacterRuntimeIds.SpriteView;
            definition.IdleSpriteResourcePath = "Characters/Anubis";
            definition.PrimaryAttackSpriteResourcePath = "Characters/AnubisAttack";
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

        public static EnemyDefinition CreateTombChampion()
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            definition.Id = "tomb_champion";
            definition.DisplayName = "Campeão da Necrópole";
            definition.Archetype = EnemyArchetype.Bruiser;
            definition.BodyColor = new Color(0.44f, 0.12f, 0.1f);
            definition.VisualScale = 1.7f;
            definition.MaxHealth = 240f;
            definition.MoveSpeed = 2.65f;
            definition.Damage = 24f;
            definition.AttackRange = 1.25f;
            definition.AttackCooldown = 0.95f;
            definition.AttackWindup = 0.34f;
            definition.Knockback = 8f;
            definition.ColliderRadius = 0.55f;
            return definition;
        }

        public static BlessingDefinition[] CreateBlessings()
        {
            return new[]
            {
                CreateBlessing("blade_of_ra", "Lâmina de Rá", "+25% de dano no ataque primário.", BlessingKind.AttackDamage, 0.25f, new Color(0.95f, 0.72f, 0.2f)),
                CreateBlessing("fang_of_set", "Presa de Set", "+15% de dano no ataque primário.", BlessingKind.AttackDamage, 0.15f, new Color(0.72f, 0.3f, 0.24f)),
                CreateBlessing("sandstep", "Passo das Areias", "+35% de distância no dash e recarga mais rápida.", BlessingKind.DashDistance, 0.35f, new Color(0.82f, 0.7f, 0.38f)),
                CreateBlessing("jackal_stride", "Passo do Chacal", "+20% de distância no dash e recarga mais rápida.", BlessingKind.DashDistance, 0.20f, new Color(0.7f, 0.56f, 0.3f)),
                CreateBlessing("ankh_vital", "Ankh Vital", "+25 de vida máxima, curado imediatamente.", BlessingKind.MaxHealth, 25f, new Color(0.35f, 0.78f, 0.55f)),
                CreateBlessing("maat_scales", "Escamas de Ma'at", "+18 de vida máxima, curado imediatamente.", BlessingKind.MaxHealth, 18f, new Color(0.62f, 0.78f, 0.82f)),
                CreateBlessing("ankh_pulse", "Pulso de Ankh", "Cura 8 de vida ao eliminar um inimigo.", BlessingKind.HealOnKill, 8f, new Color(0.45f, 0.82f, 0.86f)),
                CreateBlessing("embalmer_grace", "Graça do Embalsamador", "Cura 5 de vida ao eliminar um inimigo.", BlessingKind.HealOnKill, 5f, new Color(0.54f, 0.68f, 0.62f))
            };
        }

        public static RoomDefinition[] CreateRunRoomPool(
            EnemyDefinition scarab,
            EnemyDefinition guardian,
            EnemyDefinition archer)
        {
            return new[]
            {
                CreateArena(scarab, guardian, archer),
                CreateCrossfireArena(scarab, archer),
                CreateGuardianArena(scarab, guardian)
            };
        }

        public static RoomDefinition CreateArena(EnemyDefinition scarab, EnemyDefinition guardian, EnemyDefinition archer)
        {
            return CreateRoom(
                "anubis_arena",
                "Pátio das Areias",
                new Vector2(18f, 12f),
                new[]
                {
                    Wave("Onda 1",
                        Spawn(scarab, -2.5f, 2.2f),
                        Spawn(scarab, 2.5f, 2.4f),
                        Spawn(scarab, 0f, 3.4f)),
                    Wave("Onda 2",
                        Spawn(guardian, 0f, 2.8f),
                        Spawn(archer, 4.5f, 3.6f),
                        Spawn(scarab, -4.2f, 2.6f))
                });
        }

        public static RoomDefinition CreateCrossfireArena(EnemyDefinition scarab, EnemyDefinition archer)
        {
            return CreateRoom(
                "crossfire_court",
                "Galeria dos Chacais",
                new Vector2(20f, 12f),
                new[]
                {
                    Wave("Emboscada",
                        Spawn(archer, -5.6f, 3.5f),
                        Spawn(archer, 5.6f, 3.5f),
                        Spawn(scarab, -1.8f, 1.8f),
                        Spawn(scarab, 1.8f, 1.8f)),
                    Wave("Cerco",
                        Spawn(archer, 0f, 4.2f),
                        Spawn(scarab, -4.5f, 2.3f),
                        Spawn(scarab, 4.5f, 2.3f),
                        Spawn(scarab, 0f, 2.2f))
                });
        }

        public static RoomDefinition CreateGuardianArena(EnemyDefinition scarab, EnemyDefinition guardian)
        {
            return CreateRoom(
                "guardian_hall",
                "Salão dos Guardiões",
                new Vector2(17f, 13f),
                new[]
                {
                    Wave("Sentinelas",
                        Spawn(guardian, -3.1f, 2.8f),
                        Spawn(guardian, 3.1f, 2.8f)),
                    Wave("Ruptura",
                        Spawn(guardian, 0f, 3.1f),
                        Spawn(scarab, -4f, 2f),
                        Spawn(scarab, 4f, 2f))
                });
        }

        public static RoomDefinition CreateBossArena(
            EnemyDefinition scarab,
            EnemyDefinition archer,
            EnemyDefinition champion)
        {
            return CreateRoom(
                "necropolis_champion",
                "Câmara do Campeão",
                new Vector2(20f, 14f),
                new[]
                {
                    Wave("Guarda Final",
                        Spawn(scarab, -4.2f, 2.8f),
                        Spawn(scarab, 4.2f, 2.8f),
                        Spawn(archer, 0f, 4.6f)),
                    Wave("Campeão da Necrópole",
                        Spawn(champion, 0f, 3.4f))
                });
        }

        static RoomDefinition CreateRoom(string id, string name, Vector2 size, EncounterWave[] waves)
        {
            var room = ScriptableObject.CreateInstance<RoomDefinition>();
            room.Id = id;
            room.DisplayName = name;
            room.Size = size;
            room.LockDoorsOnEnter = true;
            room.Waves = waves;
            return room;
        }

        static EncounterWave Wave(string label, params EnemySpawn[] spawns)
        {
            return new EncounterWave
            {
                Label = label,
                Spawns = spawns
            };
        }

        static EnemySpawn Spawn(EnemyDefinition enemy, float x, float y)
        {
            return new EnemySpawn
            {
                Enemy = enemy,
                LocalPosition = new Vector2(x, y)
            };
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
