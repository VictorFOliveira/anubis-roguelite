using System.Collections.Generic;

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
            if (_pool.Count == 0 || length <= 0)
            {
                return result;
            }

            var random = new System.Random(seed);
            var previousIndex = -1;

            for (var i = 0; i < length; i++)
            {
                var index = random.Next(_pool.Count);
                if (_pool.Count > 1 && index == previousIndex)
                {
                    index = (index + 1 + random.Next(_pool.Count - 1)) % _pool.Count;
                }

                result.Add(_pool[index]);
                previousIndex = index;
            }

            return result;
        }
    }
}
