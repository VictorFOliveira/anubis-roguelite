using Anubis.Core;
using UnityEngine;

namespace Anubis.Characters
{
    public interface IPrimaryAttack
    {
        bool IsAttacking { get; }
        float DamageMultiplier { get; set; }

        void Configure(CharacterDefinition definition, TopDownMotor motor, ObjectPool pool);
        bool TryAttack(Vector2 aim);
    }
}
