using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anubis.Progression
{
    public sealed class BlessingOfferService
    {
        readonly List<BlessingDefinition> _library;

        public BlessingOfferService(IEnumerable<BlessingDefinition> library)
        {
            _library = new List<BlessingDefinition>(library);
        }

        public List<BlessingDefinition> Roll(int count, IReadOnlyList<string> excludedIds, int seed)
        {
            var candidates = new List<BlessingDefinition>();
            foreach (var blessing in _library)
            {
                if (blessing != null && !excludedIds.Contains(blessing.Id))
                {
                    candidates.Add(blessing);
                }
            }

            var random = new System.Random(seed);
            var result = new List<BlessingDefinition>(count);
            count = Mathf.Min(count, candidates.Count);
            for (var i = 0; i < count; i++)
            {
                var index = random.Next(candidates.Count);
                result.Add(candidates[index]);
                candidates.RemoveAt(index);
            }

            return result;
        }
    }
}
