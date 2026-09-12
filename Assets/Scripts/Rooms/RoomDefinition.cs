using Anubis.AI;
using UnityEngine;

namespace Anubis.Rooms
{
    [CreateAssetMenu(menuName = "Anubis/Rooms/Room Definition", fileName = "Room")]
    public class RoomDefinition : ScriptableObject
    {
        public string Id = "anubis_arena";
        public string DisplayName = "Arena de Anúbis";
        public Vector2 Size = new(18f, 12f);
        public bool LockDoorsOnEnter = true;
        public EncounterWave[] Waves = System.Array.Empty<EncounterWave>();
    }

    [System.Serializable]
    public class EncounterWave
    {
        public string Label = "Onda 1";
        public EnemySpawn[] Spawns = System.Array.Empty<EnemySpawn>();
    }

    [System.Serializable]
    public class EnemySpawn
    {
        public EnemyDefinition Enemy;
        public Vector2 LocalPosition;
    }
}
