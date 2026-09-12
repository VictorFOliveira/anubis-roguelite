using UnityEngine;

namespace Anubis.Characters
{
    [CreateAssetMenu(menuName = "Anubis/Characters/Character Form", fileName = "Form")]
    public class CharacterFormDefinition : ScriptableObject
    {
        public CharacterFormId Id = CharacterFormId.Classic;
        public string DisplayName = "Anúbis Clássico";
        [TextArea] public string Description = "Guardião do Julgamento.";
        public string SpriteResourcePath = "Characters/Anubis";
        public Color Accent = new(0.86f, 0.68f, 0.18f);
    }
}
