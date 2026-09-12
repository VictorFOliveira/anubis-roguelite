namespace Anubis.Platform
{
    public interface IPlatformServices
    {
        string PlatformId { get; }
        bool IsInitialized { get; }
        void Initialize();
        bool TrySaveCloud(string key, string payload);
        bool TryLoadCloud(string key, out string payload);
    }
}
