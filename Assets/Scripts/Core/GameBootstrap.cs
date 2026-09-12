using Anubis.Runner;
using UnityEngine;

namespace Anubis.Core
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            if (FindFirstObjectByType<GameBootstrap>() != null)
            {
                return;
            }

            var host = new GameObject("AnubisRunnerBootstrap");
            host.AddComponent<GameBootstrap>();
        }

        void Start()
        {
            if (FindFirstObjectByType<RunnerBootstrap>() != null)
            {
                return;
            }

            var runner = gameObject.AddComponent<RunnerBootstrap>();
            runner.Begin();
        }
    }
}
