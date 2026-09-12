using UnityEngine;

namespace Anubis.Combat
{
    public sealed class Hitbox2D : MonoBehaviour
    {
        [SerializeField] float radius = 0.8f;
        [SerializeField] float damage = 10f;
        [SerializeField] float knockback = 4f;
        [SerializeField] TeamId team = TeamId.Player;
        [SerializeField] LayerMask mask;

        readonly Collider2D[] _hits = new Collider2D[16];

        public void Configure(TeamId newTeam, float newRadius, float newDamage, float newKnockback, LayerMask newMask)
        {
            team = newTeam;
            radius = newRadius;
            damage = newDamage;
            knockback = newKnockback;
            mask = newMask;
        }

        public int Strike(Vector2 origin, Vector2 direction, float damageOverride = -1f)
        {
            var amount = damageOverride > 0f ? damageOverride : damage;
            var count = Physics2D.OverlapCircleNonAlloc(origin, radius, _hits, mask);
            var applied = 0;

            for (var i = 0; i < count; i++)
            {
                var hit = _hits[i];
                if (hit == null)
                {
                    continue;
                }

                var damageable = hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>();

                if (damageable == null || !damageable.IsAlive || damageable.Team == team)
                {
                    continue;
                }

                var info = new DamageInfo(amount, origin, direction, knockback, team);
                if (damageable.TryApplyDamage(info))
                {
                    applied++;
                }
            }

            return applied;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.8f, 0.1f, 0.35f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
