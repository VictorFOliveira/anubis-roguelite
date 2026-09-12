using System;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Combat
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] TeamId team = TeamId.Enemy;
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float invulnerabilityDuration;

        public TeamId Team => team;
        public float Current { get; private set; }
        public float Max { get; private set; }
        public bool IsAlive => Current > 0f;
        public bool IsInvulnerable { get; private set; }

        public event Action<HealthChangeInfo> Changed;
        public event Action<DamageInfo> Damaged;
        public event Action Died;

        float _invulnerabilityTimer;
        GameSignals _signals;

        public void Configure(TeamId newTeam, float newMax, float newInvulnerability, GameSignals signals)
        {
            team = newTeam;
            Max = newMax;
            Current = newMax;
            invulnerabilityDuration = newInvulnerability;
            _signals = signals;
            RaiseChanged();
        }

        public void SetInvulnerable(bool value)
        {
            IsInvulnerable = value;
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            Current = Mathf.Min(Max, Current + amount);
            RaiseChanged();
        }

        public void AddMaxHealth(float amount, bool healBySameAmount)
        {
            Max += amount;
            if (healBySameAmount)
            {
                Current += amount;
            }

            Current = Mathf.Min(Current, Max);
            RaiseChanged();
        }

        public bool TryApplyDamage(in DamageInfo damage)
        {
            if (!IsAlive || damage.SourceTeam == team || damage.Amount <= 0f)
            {
                return false;
            }

            if (IsInvulnerable || _invulnerabilityTimer > 0f)
            {
                return false;
            }

            Current = Mathf.Max(0f, Current - damage.Amount);
            if (invulnerabilityDuration > 0f)
            {
                _invulnerabilityTimer = invulnerabilityDuration;
            }

            Damaged?.Invoke(damage);
            RaiseChanged();
            _signals?.DamageDealt.Raise(damage);

            if (Current <= 0f)
            {
                Died?.Invoke();
            }

            return true;
        }

        void Update()
        {
            if (_invulnerabilityTimer > 0f)
            {
                _invulnerabilityTimer -= Time.deltaTime;
            }
        }

        void RaiseChanged()
        {
            var info = new HealthChangeInfo(Current, Max, team);
            Changed?.Invoke(info);
            if (team == TeamId.Player)
            {
                _signals?.PlayerHealthChanged.Raise(info);
            }
        }
    }
}
