using UnityEngine;

namespace Anubis.Progression
{
    [CreateAssetMenu(menuName = "Anubis/Blessings/Blessing Definition", fileName = "Blessing")]
    public class BlessingDefinition : ScriptableObject
    {
        public string Id = "blade_of_ra";
        public string DisplayName = "Lâmina de Rá";
        [TextArea] public string Description = "+25% de dano com o Khopesh.";
        public BlessingKind Kind = BlessingKind.AttackDamage;
        public float Value = 0.25f;
        public Color Accent = new(0.95f, 0.72f, 0.2f);
    }
}
