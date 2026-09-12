#if UNITY_EDITOR
using Anubis.AI;
using Anubis.Characters;
using Anubis.Core;
using Anubis.Progression;
using Anubis.Rooms;
using UnityEditor;
using UnityEngine;

namespace Anubis.EditorTools
{
    public static class VerticalSliceAssetsMenu
    {
        const string Root = "Assets/ScriptableObjects";

        [MenuItem("Anubis/Create Default Data Assets")]
        public static void CreateDefaultAssets()
        {
            var anubis = VerticalSliceCatalog.CreateAnubis();
            Save(anubis, $"{Root}/Characters/Anubis.asset");

            var scarab = VerticalSliceCatalog.CreateScarab();
            var guardian = VerticalSliceCatalog.CreateGuardian();
            var archer = VerticalSliceCatalog.CreateArcher();
            Save(scarab, $"{Root}/Enemies/Scarab.asset");
            Save(guardian, $"{Root}/Enemies/TombGuardian.asset");
            Save(archer, $"{Root}/Enemies/JackalArcher.asset");

            var blessings = VerticalSliceCatalog.CreateBlessings();
            foreach (var blessing in blessings)
            {
                Save(blessing, $"{Root}/Blessings/{ToPascal(blessing.Id)}.asset");
            }

            var room = VerticalSliceCatalog.CreateArena(
                AssetDatabase.LoadAssetAtPath<EnemyDefinition>($"{Root}/Enemies/Scarab.asset"),
                AssetDatabase.LoadAssetAtPath<EnemyDefinition>($"{Root}/Enemies/TombGuardian.asset"),
                AssetDatabase.LoadAssetAtPath<EnemyDefinition>($"{Root}/Enemies/JackalArcher.asset"));
            Save(room, $"{Root}/Rooms/AnubisArena.asset");

            var relic = ScriptableObject.CreateInstance<RelicDefinition>();
            relic.Id = "placeholder";
            relic.DisplayName = "Relíquia (em breve)";
            relic.Description = "Reservado para o loop completo.";
            Save(relic, $"{Root}/Relics/PlaceholderRelic.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Anubis: assets de dados do vertical slice criados em ScriptableObjects.");
        }

        static void Save(Object asset, string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(asset, existing);
                Object.DestroyImmediate(asset);
                return;
            }

            AssetDatabase.CreateAsset(asset, path);
        }

        static string ToPascal(string id)
        {
            var parts = id.Split('_');
            var result = string.Empty;
            foreach (var part in parts)
            {
                if (part.Length == 0)
                {
                    continue;
                }

                result += char.ToUpperInvariant(part[0]) + part.Substring(1);
            }

            return result;
        }
    }
}
#endif
