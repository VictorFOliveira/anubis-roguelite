using System.IO;
using Anubis.Platform;
using UnityEngine;

namespace Anubis.Save
{
    public sealed class SaveService
    {
        const string FileName = "anubis_save.json";
        readonly IPlatformServices _platform;
        readonly string _path;

        public SaveData Current { get; private set; }

        public SaveService(IPlatformServices platform)
        {
            _platform = platform;
            _path = Path.Combine(Application.persistentDataPath, FileName);
            Current = Load();
        }

        public SaveData Load()
        {
            if (_platform.TryLoadCloud(FileName, out var cloud) && !string.IsNullOrEmpty(cloud))
            {
                Current = JsonUtility.FromJson<SaveData>(cloud) ?? new SaveData();
                return Current;
            }

            if (!File.Exists(_path))
            {
                Current = new SaveData();
                return Current;
            }

            var json = File.ReadAllText(_path);
            Current = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            return Current;
        }

        public void Write()
        {
            var json = JsonUtility.ToJson(Current, true);
            File.WriteAllText(_path, json);
            _platform.TrySaveCloud(FileName, json);
        }
    }
}
