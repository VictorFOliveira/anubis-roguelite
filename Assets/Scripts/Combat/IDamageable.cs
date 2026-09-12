namespace Anubis.Combat
{
    public interface IDamageable
    {
        TeamId Team { get; }
        bool IsAlive { get; }
        bool TryApplyDamage(in DamageInfo damage);
    }
}
