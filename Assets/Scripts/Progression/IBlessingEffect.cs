using Anubis.Characters;
using Anubis.Core;

namespace Anubis.Progression
{
    public interface IBlessingEffect
    {
        string BlessingId { get; }
        void Apply(PlayerController player, BlessingDefinition definition, GameSignals signals);
        void Remove(PlayerController player, GameSignals signals);
    }
}
