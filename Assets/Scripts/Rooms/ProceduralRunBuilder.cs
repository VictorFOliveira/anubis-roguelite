using System.Collections.Generic;
using UnityEngine;

namespace Anubis.Rooms
{
    public sealed class ProceduralRunBuilder
    {
        readonly List<RoomDefinition> _pool;

        public ProceduralRunBuilder(IEnumerable<RoomDefinition> pool)
        {
            _pool = new List<RoomDefinition>(pool);
        }

        public List<RoomDefinition> BuildLinearRun(int length, int seed)
        {
            var result = new List<RoomDefinition>(length);
            if (_pool.Count == 0)
            {
                return result;
            }

            var random = new System.Random(seed);
            for (var i = 0; i < length; i++)
            {
                result.Add(_pool[random.Next(_pool.Count)]);
            }

            return result;
        }
    }
}
