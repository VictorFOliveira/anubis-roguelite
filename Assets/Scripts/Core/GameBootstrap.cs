using System.Collections.Generic;
using Anubis.Characters;
using Anubis.Combat;
using Anubis.Core;
using Anubis.Platform;
using Anubis.Progression;
using Anubis.Rooms;
using Anubis.Save;
using Anubis.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

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

            var host = new GameObject("GameBootstrap");
            host.AddComponent<GameBootstrap>();
        }

        GameSignals _signals;
        GameInputReader _input;
        ObjectPool _pool;
        PlayerController _player;
        ArenaController _arena;
        BlessingRuntimeService _blessings;
        BlessingOfferService _offers;
        BlessingChoiceView _choiceView;
        EndPanelView _endPanel;
        MetaProgression _meta;
        BlessingDefinition[] _library;
        bool _offerOpen;
        bool _ended;

        void Start()
        {
            Physics2D.gravity = Vector2.zero;
            ConfigureLayerCollisions();

            var platform = SteamworksGate.Create();
            platform.Initialize();

            var save = new SaveService(platform);
            _meta = new MetaProgression(save);
            _meta.RegisterRunStarted();

            _signals = GameSignals.CreateRuntime();
            var actions = GameInputFactory.Create();
            _input = new GameInputReader(actions);

            _pool = gameObject.AddComponent<ObjectPool>();
            _pool.Initialize();
            RegisterPooledPrefabs();

            var anubis = VerticalSliceCatalog.CreateAnubis();
            var scarab = VerticalSliceCatalog.CreateScarab();
            var guardian = VerticalSliceCatalog.CreateGuardian();
            var archer = VerticalSliceCatalog.CreateArcher();
            _library = VerticalSliceCatalog.CreateBlessings();
            var room = VerticalSliceCatalog.CreateArena(scarab, guardian, archer);

            var camera = EnsureCamera();
            EnsureEventSystem();

            var playerObject = new GameObject("Anubis");
            playerObject.transform.position = new Vector3(0f, -3.6f, 0f);
            _player = playerObject.AddComponent<PlayerController>();
            _player.Bind(anubis, _input, camera, _signals, _pool);
            if (_meta.Data.PermanentHealthBonus > 0)
            {
                _player.Health.AddMaxHealth(_meta.Data.PermanentHealthBonus, true);
            }

            camera.GetComponent<FollowCamera2D>().Configure(_player.transform, room.Size);

            var arenaObject = new GameObject("Arena");
            _arena = arenaObject.AddComponent<ArenaController>();
            _arena.Bind(room, _player, _signals, _pool, _input);

            _blessings = new BlessingRuntimeService(_player, _signals);
            _offers = new BlessingOfferService(_library);

            BuildUi();
            BindFlow();
            _arena.Begin();
            _signals.RunStarted.Raise();
        }

        void Update()
        {
            if (!_offerOpen || _ended)
            {
                return;
            }

            if (Keyboard.current != null)
            {
                if (Keyboard.current.digit1Key.wasPressedThisFrame) _choiceView.ConfirmIndex(0);
                if (Keyboard.current.digit2Key.wasPressedThisFrame) _choiceView.ConfirmIndex(1);
                if (Keyboard.current.digit3Key.wasPressedThisFrame) _choiceView.ConfirmIndex(2);
                if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
                {
                    _choiceView.MoveSelection(-1);
                }

                if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
                {
                    _choiceView.MoveSelection(1);
                }

                if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    _choiceView.ConfirmSelection();
                }
            }

            if (Gamepad.current != null)
            {
                if (Gamepad.current.dpad.left.wasPressedThisFrame || Gamepad.current.leftStick.left.wasPressedThisFrame)
                {
                    _choiceView.MoveSelection(-1);
                }

                if (Gamepad.current.dpad.right.wasPressedThisFrame || Gamepad.current.leftStick.right.wasPressedThisFrame)
                {
                    _choiceView.MoveSelection(1);
                }

                if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                {
                    _choiceView.ConfirmSelection();
                }
            }
        }

        void BindFlow()
        {
            _signals.BlessingOfferOpened.Subscribe(OpenBlessingOffer);
            _signals.PlayerDied.Subscribe(OnPlayerDied);
            _choiceView.Chosen += OnBlessingChosen;
            _endPanel.RestartRequested += Restart;
        }

        void OpenBlessingOffer()
        {
            if (_offerOpen || _ended)
            {
                return;
            }

            var roll = _offers.Roll(3, _blessings.ChosenIds, Time.frameCount + _meta.Data.RunsStarted);
            var seen = new List<string>();
            foreach (var blessing in roll)
            {
                seen.Add(blessing.Id);
            }

            _meta.RegisterBlessingsSeen(seen);
            _player.SetControlEnabled(false);
            _offerOpen = true;
            _choiceView.Show(roll);
        }

        void OnBlessingChosen(BlessingDefinition definition)
        {
            if (!_offerOpen)
            {
                return;
            }

            _blessings.Apply(definition);
            _meta.RegisterBlessing(definition.Id);
            _meta.RegisterArenaCleared();
            _choiceView.Hide();
            _offerOpen = false;
            _ended = true;
            _signals.SliceCompleted.Raise();
            _endPanel.Show("Vertical Slice completo", $"{definition.DisplayName} foi selada.\nA arena e as portas já estão funcionais. Reinicie para outra run.");
        }

        void OnPlayerDied()
        {
            if (_ended)
            {
                return;
            }

            _ended = true;
            _meta.RegisterDeath();
            _endPanel.Show("Anúbis caiu", "A progressão permanente foi salva. Tente outra run.");
        }

        void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void BuildUi()
        {
            var hud = new GameObject("HUD");
            hud.AddComponent<HudView>().Bind(_player, _signals, _meta);

            var choice = new GameObject("BlessingChoice");
            _choiceView = choice.AddComponent<BlessingChoiceView>();
            _choiceView.Build();

            var end = new GameObject("EndPanel");
            _endPanel = end.AddComponent<EndPanelView>();
            _endPanel.Build();
        }

        void RegisterPooledPrefabs()
        {
            _pool.RegisterPrefab(Projectile2D.EnemyPoolKey, CreateProjectilePrefab(), 16);
        }

        GameObject CreateProjectilePrefab()
        {
            var prefab = new GameObject("EnemyProjectile");
            prefab.SetActive(false);
            prefab.layer = GameLayers.Projectile;
            var renderer = prefab.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.CreateCircle(new Color(0.85f, 0.25f, 0.18f), 32);
            renderer.sortingOrder = GameSorting.Vfx;
            var hit = prefab.AddComponent<CircleCollider2D>();
            hit.radius = 0.12f;
            hit.isTrigger = true;
            GetOrAddDisabled(prefab);
            var projectile = prefab.AddComponent<Projectile2D>();
            projectile.Initialize(_pool);
            return prefab;
        }

        static void GetOrAddDisabled(GameObject prefab)
        {
            var body = prefab.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
        }

        static Camera EnsureCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 7f;
            camera.backgroundColor = new Color(0.05f, 0.04f, 0.03f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 50f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            if (camera.GetComponent<AudioListener>() == null)
            {
                camera.gameObject.AddComponent<AudioListener>();
            }

            if (camera.GetComponent<FollowCamera2D>() == null)
            {
                camera.gameObject.AddComponent<FollowCamera2D>();
            }

            return camera;
        }

        static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        static void ConfigureLayerCollisions()
        {
            Physics2D.IgnoreLayerCollision(GameLayers.Player, GameLayers.Enemy, true);
            Physics2D.IgnoreLayerCollision(GameLayers.Player, GameLayers.Projectile, false);
            Physics2D.IgnoreLayerCollision(GameLayers.Enemy, GameLayers.Projectile, true);
            Physics2D.IgnoreLayerCollision(GameLayers.Projectile, GameLayers.Environment, true);
        }

        void OnDestroy()
        {
            if (_signals != null)
            {
                _signals.BlessingOfferOpened.Unsubscribe(OpenBlessingOffer);
                _signals.PlayerDied.Unsubscribe(OnPlayerDied);
            }

            if (_choiceView != null)
            {
                _choiceView.Chosen -= OnBlessingChosen;
            }

            if (_endPanel != null)
            {
                _endPanel.RestartRequested -= Restart;
            }

            _input?.Dispose();
        }
    }
}
