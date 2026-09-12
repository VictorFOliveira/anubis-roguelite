namespace Anubis.Platform
{
    /// <summary>
    /// Ponto único de integração futura com Steamworks.NET / com.rlabrecque.steamworks.net.
    /// O vertical slice usa <see cref="LocalPlatformServices"/>. Quando o AppID existir,
    /// troque a factory abaixo sem alterar SaveService ou o restante do jogo.
    /// </summary>
    public static class SteamworksGate
    {
        public const bool SteamEnabled = false;

        public static IPlatformServices Create()
        {
            if (SteamEnabled)
            {
                return new SteamPlatformServicesStub();
            }

            return new LocalPlatformServices();
        }
    }

    public sealed class SteamPlatformServicesStub : IPlatformServices
    {
        public string PlatformId => "steam-stub";
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            // Futuro: SteamAPI.Init(), SteamUserStats, SteamRemoteStorage.
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
