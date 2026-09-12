using Anubis.Core;
using UnityEngine;

namespace Anubis.Runner
{
    public sealed class RunnerSkyGodController : MonoBehaviour
    {
        RunnerPlayerController _player;
        Camera _camera;
        float _timer = 2.4f;
        Transform _icon;

        public void Configure(RunnerPlayerController player, Camera camera)
        {
            _player = player;
            _camera = camera;
            BuildRaIcon();
        }

        void Update()
        {
            if (_player == null || _player.IsDead)
            {
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                var distanceFactor = Mathf.Clamp01(_player.DistanceMeters / 3500f);
                _timer = Mathf.Lerp(2.7f, 1.25f, distanceFactor);
                var strikeX = _player.transform.position.x + Random.Range(4.2f, 9.5f);
                SpawnStrike(strikeX);
            }

            if (_icon != null && _camera != null)
            {
                _icon.position = _camera.transform.position + new Vector3(6.5f, 3.6f, 10f);
            }
        }

        void BuildRaIcon()
        {
            var root = new GameObject("RaSunDisc");
            root.transform.localScale = Vector3.one * 1.25f;
            _icon = root.transform;

            var disc = root.AddComponent<SpriteRenderer>();
            disc.sprite = RuntimeSpriteFactory.CreateCircle(new Color(1f, 0.72f, 0.13f), 64, 0.94f);
            disc.sortingOrder = GameSorting.Vfx - 2;

            var eye = new GameObject("Eye");
            eye.transform.SetParent(root.transform, false);
            eye.transform.localScale = new Vector3(0.7f, 0.22f, 1f);
            var eyeRenderer = eye.AddComponent<SpriteRenderer>();
            eyeRenderer.sprite = RuntimeSpriteFactory.CreateDiamond(new Color(0.24f, 0.08f, 0.04f), 32);
            eyeRenderer.sortingOrder = GameSorting.Vfx - 1;
        }

        void SpawnStrike(float x)
        {
            var go = new GameObject("RaSolarStrike");
            go.transform.position = new Vector3(x, 1.2f, 0f);
            go.AddComponent<RunnerRaStrike>().Configure(_player);
        }
    }

    public sealed class RunnerRaStrike : MonoBehaviour
    {
        RunnerPlayerController _player;
        SpriteRenderer _beam;
        float _timer;
        bool _struck;

        public void Configure(RunnerPlayerController player)
        {
            _player = player;
            _beam = gameObject.AddComponent<SpriteRenderer>();
            _beam.sprite = RuntimeSpriteFactory.CreateRect(new Color(1f, 0.82f, 0.2f), 64, 64);
            _beam.sortingOrder = GameSorting.Vfx + 5;
            transform.localScale = new Vector3(0.14f, 8.5f, 1f);
            _beam.color = new Color(1f, 0.85f, 0.25f, 0.22f);
            _timer = 0.72f;
        }

        void Update()
        {
            _timer -= Time.deltaTime;
            if (!_struck)
            {
                var pulse = 0.13f + Mathf.Abs(Mathf.Sin(Time.time * 18f)) * 0.18f;
                _beam.color = new Color(1f, 0.78f, 0.18f, pulse);
                if (_timer <= 0f)
                {
                    Strike();
                }
                return;
            }

            var alpha = Mathf.Clamp01((_timer + 0.24f) / 0.24f);
            _beam.color = new Color(1f, 0.93f, 0.55f, alpha);
            if (_timer <= -0.24f)
            {
                Destroy(gameObject);
            }
        }

        void Strike()
        {
            _struck = true;
            _beam.color = new Color(1f, 0.95f, 0.6f, 1f);
            transform.localScale = new Vector3(0.82f, 8.5f, 1f);

            if (_player != null && Mathf.Abs(_player.transform.position.x - transform.position.x) < 0.9f)
            {
                _player.TakeHit();
            }
        }
    }
}
