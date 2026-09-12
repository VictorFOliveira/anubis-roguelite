using Anubis.Characters;
using Anubis.Core;

namespace Anubis.Progression
{
    public sealed class AttackDamageBlessing : IBlessingEffect
    {
        public string BlessingId { get; }
        readonly float _multiplier;

        public AttackDamageBlessing(string id, float multiplier)
        {
            BlessingId = id;
            _multiplier = multiplier;
        }

        public void Apply(PlayerController player, BlessingDefinition definition, GameSignals signals)
        {
            if (player.PrimaryAttack != null)
            {
                player.PrimaryAttack.DamageMultiplier += _multiplier;
            }
        }

        public void Remove(PlayerController player, GameSignals signals)
        {
            if (player.PrimaryAttack != null)
            {
                player.PrimaryAttack.DamageMultiplier -= _multiplier;
            }
        }
    }

    public sealed class DashDistanceBlessing : IBlessingEffect
    {
        public string BlessingId { get; }
        readonly float _multiplier;

        public DashDistanceBlessing(string id, float multiplier)
        {
            BlessingId = id;
            _multiplier = multiplier;
        }

        public void Apply(PlayerController player, BlessingDefinition definition, GameSignals signals)
        {
            player.Dash.DistanceMultiplier += _multiplier;
            player.Dash.CooldownMultiplier = 0.85f;
        }

        public void Remove(PlayerController player, GameSignals signals)
        {
            player.Dash.DistanceMultiplier -= _multiplier;
            player.Dash.CooldownMultiplier = 1f;
        }
    }

    public sealed class MaxHealthBlessing : IBlessingEffect
    {
        public string BlessingId { get; }
        readonly float _amount;

        public MaxHealthBlessing(string id, float amount)
        {
            BlessingId = id;
            _amount = amount;
        }

        public void Apply(PlayerController player, BlessingDefinition definition, GameSignals signals)
        {
            player.Health.AddMaxHealth(_amount, true);
        }

        public void Remove(PlayerController player, GameSignals signals) { }
    }

    public sealed class HealOnKillBlessing : IBlessingEffect
    {
        public string BlessingId { get; }
        readonly float _heal;
        PlayerController _player;

        public HealOnKillBlessing(string id, float heal)
        {
            BlessingId = id;
            _heal = heal;
        }

        public void Apply(PlayerController player, BlessingDefinition definition, GameSignals signals)
        {
            _player = player;
            signals.EnemyKilled.Subscribe(OnEnemyKilled);
        }

        public void Remove(PlayerController player, GameSignals signals)
        {
            signals.EnemyKilled.Unsubscribe(OnEnemyKilled);
        }

        void OnEnemyKilled(EnemyKillInfo _)
        {
            _player?.Health.Heal(_heal);
        }
    }
}
