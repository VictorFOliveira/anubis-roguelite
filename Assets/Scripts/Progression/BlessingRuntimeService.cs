using System.Collections.Generic;
using Anubis.Characters;
using Anubis.Core;

namespace Anubis.Progression
{
    public sealed class BlessingRuntimeService
    {
        readonly GameSignals _signals;
        readonly PlayerController _player;
        readonly List<IBlessingEffect> _active = new();
        readonly List<string> _chosenIds = new();

        public IReadOnlyList<string> ChosenIds => _chosenIds;

        public BlessingRuntimeService(PlayerController player, GameSignals signals)
        {
            _player = player;
            _signals = signals;
        }

        public void Apply(BlessingDefinition definition)
        {
            if (definition == null || _chosenIds.Contains(definition.Id))
            {
                return;
            }

            var effect = CreateEffect(definition);
            effect.Apply(_player, definition, _signals);
            _active.Add(effect);
            _chosenIds.Add(definition.Id);
            _signals.BlessingChosen.Raise(definition.Id);
        }

        public void Clear()
        {
            foreach (var effect in _active)
            {
                effect.Remove(_player, _signals);
            }

            _active.Clear();
            _chosenIds.Clear();
        }

        public static IBlessingEffect CreateEffect(BlessingDefinition definition)
        {
            return definition.Kind switch
            {
                BlessingKind.AttackDamage => new AttackDamageBlessing(definition.Id, definition.Value),
                BlessingKind.DashDistance => new DashDistanceBlessing(definition.Id, definition.Value),
                BlessingKind.MaxHealth => new MaxHealthBlessing(definition.Id, definition.Value),
                BlessingKind.HealOnKill => new HealOnKillBlessing(definition.Id, definition.Value),
                _ => new AttackDamageBlessing(definition.Id, definition.Value)
            };
        }
    }
}
