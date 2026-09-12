using System;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Characters
{
    public static class CharacterRuntimeIds
    {
        public const string KhopeshAttack = "khopesh";
        public const string SpriteView = "sprite";
    }

    public static class CharacterRuntimeFactory
    {
        public static IPrimaryAttack AddPrimaryAttack(
            GameObject host,
            CharacterDefinition definition,
            TopDownMotor motor,
            ObjectPool pool)
        {
            var id = string.IsNullOrWhiteSpace(definition.PrimaryAttackId)
                ? CharacterRuntimeIds.KhopeshAttack
                : definition.PrimaryAttackId.Trim();

            switch (id.ToLowerInvariant())
            {
                case CharacterRuntimeIds.KhopeshAttack:
                {
                    var attack = host.AddComponent<KhopeshAttack>();
                    attack.Configure(definition, motor, pool);
                    return attack;
                }
                default:
                    throw new InvalidOperationException(
                        $"Unknown primary attack '{definition.PrimaryAttackId}' for character '{definition.Id}'.");
            }
        }

        public static ICharacterView AddView(GameObject host, CharacterDefinition definition)
        {
            var id = string.IsNullOrWhiteSpace(definition.ViewId)
                ? CharacterRuntimeIds.SpriteView
                : definition.ViewId.Trim();

            switch (id.ToLowerInvariant())
            {
                case CharacterRuntimeIds.SpriteView:
                {
                    var view = host.AddComponent<SpriteCharacterView>();
                    view.Build(definition);
                    return view;
                }
                default:
                    throw new InvalidOperationException(
                        $"Unknown character view '{definition.ViewId}' for character '{definition.Id}'.");
            }
        }
    }
}
