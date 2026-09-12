namespace Anubis.Platform
{
    public sealed class LocalPlatformServices : IPlatformServices
    {
        public string PlatformId => "local";
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public bool TrySaveCloud(string key, string payload)
        {
            return false;
        }

        public bool TryLoadCloud(string key, out string payload)
        {
            payload = null;
            return false;
        }
    }
}
