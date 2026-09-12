using UnityEngine;

namespace Anubis.Characters
{
    public interface ICharacterView
    {
        SpriteRenderer Body { get; }

        void Build(CharacterDefinition definition);
        void PlayPrimaryAttack();
        void SetFacing(Vector2 facing);
        void SetDashing(bool dashing);
    }
}
