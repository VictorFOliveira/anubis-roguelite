using UnityEngine;

namespace Anubis.Progression
{
    [CreateAssetMenu(menuName = "Anubis/Relics/Relic Definition", fileName = "Relic")]
    public class RelicDefinition : ScriptableObject
    {
        public string Id = "relic";
        public string DisplayName = "Relíquia";
        [TextArea] public string Description = "Reservado para o loop completo.";
        public Color Accent = new(0.55f, 0.75f, 0.82f);
    }
}
