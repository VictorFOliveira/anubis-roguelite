namespace Anubis.Characters
{
    /// <summary>
    /// Compatibility wrapper kept so existing scenes or prefabs that reference AnubisView
    /// continue to deserialize. New characters use SpriteCharacterView through
    /// CharacterRuntimeFactory.
    /// </summary>
    public sealed class AnubisView : SpriteCharacterView
    {
    }
}
