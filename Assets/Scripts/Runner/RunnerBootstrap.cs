using Anubis.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Anubis.Runner
{
    public sealed class RunnerBootstrap : MonoBehaviour
    {
        RunnerPlayerController _player;
        RunnerHud _hud;
        bool _ended;

        public void Begin()
        {
            Physics2D.gravity = new Vector2(0f, -32f);
            Physics2D.IgnoreLayerCollision(GameLayers.Player, GameLayers.Enemy, false);
            Physics2D.IgnoreLayerCollision(GameLayers.Player, GameLayers.Environment, false);

            var camera = EnsureCamera();

            var backdrop = new GameObject("PixelBackdrop").AddComponent<RunnerBackdrop>();
            backdrop.Configure(camera);

            var world = new GameObject("RunnerWorld").AddComponent<RunnerWorldGenerator>();
            world.Configure(backdrop);
            world.BuildInitial();

            var playerObject = new GameObject("AnubisRunner");
            playerObject.transform.position = new Vector3(0f, -1.65f, 0f);
            _player = playerObject.AddComponent<RunnerPlayerController>();
            _player.Configure();

            world.Track(_player.transform);

            var follow = camera.gameObject.AddComponent<RunnerCameraFollow>();
            follow.Configure(_player.transform);

            var ra = new GameObject("RaSkyGod").AddComponent<RunnerSkyGodController>();
            ra.Configure(_player, camera);

            var hudObject = new GameObject("RunnerHUD");
            _hud = hudObject.AddComponent<RunnerHud>();
            _hud.Bind(_player, world);

            _player.Died += OnPlayerDied;
        }

        void Update()
        {
            if (!_ended)
            {
                return;
            }

            var restart = Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
            restart |= Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;
            if (restart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        void OnPlayerDied()
        {
            if (_ended)
            {
                return;
            }

            _ended = true;
            _hud.ShowGameOver(_player.DistanceMeters);
        }

        static Camera EnsureCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 5.4f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.78f, 0.52f, 0.24f);
            camera.transform.position = new Vector3(4f, 0f, -10f);
            return camera;
        }

        void OnDestroy()
        {
            if (_player != null)
            {
                _player.Died -= OnPlayerDied;
            }
        }
    }
}
