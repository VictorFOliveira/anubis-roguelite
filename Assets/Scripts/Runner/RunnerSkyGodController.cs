using Anubis.Core;
using UnityEngine;

namespace Anubis.Runner
{
    public sealed class RunnerSkyGodController : MonoBehaviour
    {
        const float GroundSurfaceY = -2.65f;
        const float RaTargetWidth = 2.30f;

        RunnerPlayerController _player;
        Camera _camera;
        Transform _root;
        Transform _muzzle;
        Transform _fallbackArm;
        SpriteRenderer _raRenderer;
        Sprite _idleSprite;
        Sprite _castSprite;
        Sprite _orbSprite;
        AudioSource _audio;
        AudioClip _orbCastClip;
        AudioClip _orbImpactClip;
        AudioClip _specialChargeClip;
        AudioClip _specialStrikeClip;

        float _idleTime;
        float _attackCooldown = 1.35f;
        float _specialCooldown = 6.5f;
        float _shotDelay;
        float _castTimer;
        int _burstShotsRemaining;
        bool _specialCasting;

        public void Configure(RunnerPlayerController player, Camera camera)
        {
            _player = player;
            _camera = camera;

            _idleSprite = LoadFirstSprite("Runner/ra_cloud_idle", "Runner/ra_on_cloud");
            _castSprite = LoadFirstSprite("Runner/ra_cloud_cast", "Runner/ra_cast");
            _orbSprite = LoadFirstSprite("Runner/ra_orb", "Runner/ra_orb_projectile");

            BuildAudio();
            BuildVisual();
        }

        void Update()
        {
            if (_player == null || _camera == null || _root == null)
            {
                return;
            }

            UpdateFloatingMotion();
            UpdateCastAnimation();

            if (_player.IsDead)
            {
                return;
            }

            UpdateAttackPattern();
        }

        void UpdateFloatingMotion()
        {
            _idleTime += Time.deltaTime;

            var basePosition = _camera.transform.position + new Vector3(5.9f, 3.45f, 10f);
            var bobX = Mathf.Sin(_idleTime * 1.25f) * 0.20f;
            var bobY = Mathf.Sin(_idleTime * 2.05f) * 0.13f;
            _root.position = basePosition + new Vector3(bobX, bobY, 0f);

            if (_castTimer <= 0f)
            {
                _root.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(_idleTime * 1.35f) * 1.4f);
            }
        }

        void UpdateAttackPattern()
        {
            var difficulty = Mathf.Clamp01(_player.DistanceMeters / 3200f);
            _specialCooldown -= Time.deltaTime;

            if (_burstShotsRemaining > 0)
            {
                _shotDelay -= Time.deltaTime;
                if (_shotDelay <= 0f)
                {
                    FireOrb(difficulty);
                    _burstShotsRemaining--;
                    _shotDelay = Mathf.Lerp(0.34f, 0.18f, difficulty);
                }

                return;
            }

            _attackCooldown -= Time.deltaTime;
            if (_attackCooldown > 0f)
            {
                return;
            }

            var specialUnlocked = _player.DistanceMeters >= 450f;
            if (specialUnlocked && _specialCooldown <= 0f)
            {
                FireSpecialStrike(difficulty);
                _specialCooldown = Mathf.Lerp(8.5f, 5.2f, difficulty);
                _attackCooldown = 2.2f;
                return;
            }

            _burstShotsRemaining = Random.Range(2, 5);
            _shotDelay = 0f;
            _attackCooldown = Mathf.Lerp(2.25f, 1.05f, difficulty);
        }

        void FireOrb(float difficulty)
        {
            BeginCast(false);
            _audio.PlayOneShot(_orbCastClip, 0.35f);

            var spawnPosition = _muzzle != null ? _muzzle.position : _root.position;
            var lead = Mathf.Lerp(0.7f, 2.1f, difficulty);
            var target = _player.transform.position + new Vector3(lead, Random.Range(0.15f, 0.75f), 0f);
            var direction = (target - spawnPosition).normalized;
            var speed = Mathf.Lerp(6.2f, 9.0f, difficulty);

            var orbObject = new GameObject("RaOrbProjectile");
            orbObject.AddComponent<RunnerRaOrbProjectile>().Configure(
                _player,
                _orbSprite,
                spawnPosition,
                direction * speed,
                _orbImpactClip,
                GroundSurfaceY);
        }

        void FireSpecialStrike(float difficulty)
        {
            BeginCast(true);
            _audio.PlayOneShot(_specialChargeClip, 0.62f);

            var prediction = Mathf.Lerp(2.5f, 4.6f, difficulty);
            var strikeX = _player.transform.position.x + prediction + Random.Range(-0.55f, 0.75f);
            var strikeObject = new GameObject("RaSolarJudgement");
            strikeObject.transform.position = new Vector3(strikeX, 1.0f, 0f);
            strikeObject.AddComponent<RunnerRaStrike>().Configure(
                _player,
                _specialStrikeClip,
                _orbImpactClip,
                GroundSurfaceY);
        }

        void BeginCast(bool special)
        {
            _specialCasting = special;
            _castTimer = special ? 0.72f : 0.30f;

            if (_raRenderer != null && _castSprite != null)
            {
                ApplyRaSprite(_castSprite);
            }
        }

        void UpdateCastAnimation()
        {
            if (_castTimer <= 0f)
            {
                if (_raRenderer != null && _idleSprite != null && _raRenderer.sprite != _idleSprite)
                {
                    ApplyRaSprite(_idleSprite);
                }

                if (_fallbackArm != null)
                {
                    _fallbackArm.localRotation = Quaternion.Euler(0f, 0f, 22f);
                }

                _specialCasting = false;
                return;
            }

            _castTimer -= Time.deltaTime;
            var duration = _specialCasting ? 0.72f : 0.30f;
            var t = 1f - Mathf.Clamp01(_castTimer / duration);
            var punch = Mathf.Sin(t * Mathf.PI);
            _root.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-8f, 4f, punch));

            if (_fallbackArm != null)
            {
                var castAngle = _specialCasting ? 88f : 68f;
                _fallbackArm.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(22f, castAngle, punch));
            }
        }

        void BuildAudio()
        {
            _audio = gameObject.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.spatialBlend = 0f;
            _audio.volume = 0.75f;

            _orbCastClip = RunnerRaAudioFactory.CreateSweep("RaOrbCast", 0.13f, 520f, 1180f, 0.30f);
            _orbImpactClip = RunnerRaAudioFactory.CreateImpact("RaOrbImpact", 0.18f, 170f, 0.34f);
            _specialChargeClip = RunnerRaAudioFactory.CreateSweep("RaSolarCharge", 0.42f, 180f, 860f, 0.28f);
            _specialStrikeClip = RunnerRaAudioFactory.CreateImpact("RaSolarStrike", 0.34f, 82f, 0.52f);
        }

        void BuildVisual()
        {
            _root = new GameObject("RaOnCloud").transform;
            _root.localScale = Vector3.one;

            if (_idleSprite != null)
            {
                _raRenderer = _root.gameObject.AddComponent<SpriteRenderer>();
                _raRenderer.sortingOrder = 130;
                ApplyRaSprite(_idleSprite);

                _muzzle = new GameObject("Muzzle").transform;
                _muzzle.SetParent(_root, false);
                _muzzle.localPosition = new Vector3(-0.70f, 0.48f, 0f);
                return;
            }

            Debug.LogWarning("[Runner] Sprite de Rá não foi carregado; usando fallback procedural.");
            BuildFallbackVisual();
        }

        void ApplyRaSprite(Sprite sprite)
        {
            if (_raRenderer == null || sprite == null)
            {
                return;
            }

            _raRenderer.sprite = sprite;
            var width = Mathf.Max(0.05f, sprite.bounds.size.x);
            var scale = RaTargetWidth / width;
            _raRenderer.transform.localScale = Vector3.one * scale;
        }

        void BuildFallbackVisual()
        {
            var cloud = new GameObject("Cloud").transform;
            cloud.SetParent(_root, false);
            cloud.localPosition = new Vector3(0f, -0.48f, 0f);
            CreateCircle(cloud, new Vector2(-0.62f, 0f), 0.78f, new Color(0.92f, 0.95f, 1f), 126);
            CreateCircle(cloud, new Vector2(-0.05f, 0.16f), 0.92f, new Color(0.98f, 0.99f, 1f), 128);
            CreateCircle(cloud, new Vector2(0.62f, 0f), 0.72f, new Color(0.90f, 0.94f, 1f), 126);
            CreateCircle(cloud, new Vector2(0.05f, -0.20f), 1.05f, new Color(0.82f, 0.88f, 0.96f), 125);

            var torso = new GameObject("RaTorso");
            torso.transform.SetParent(_root, false);
            torso.transform.localPosition = new Vector3(0f, 0.18f, 0f);
            torso.transform.localScale = new Vector3(0.72f, 0.95f, 1f);
            var torsoRenderer = torso.AddComponent<SpriteRenderer>();
            torsoRenderer.sprite = RuntimeSpriteFactory.CreateRect(new Color(0.12f, 0.34f, 0.72f), 42, 58);
            torsoRenderer.sortingOrder = 131;

            var head = new GameObject("RaSunHead");
            head.transform.SetParent(_root, false);
            head.transform.localPosition = new Vector3(0f, 0.98f, 0f);
            var headRenderer = head.AddComponent<SpriteRenderer>();
            headRenderer.sprite = RuntimeSpriteFactory.CreateCircle(new Color(0.86f, 0.31f, 0.16f), 48, 0.95f);
            headRenderer.sortingOrder = 133;
            head.transform.localScale = Vector3.one * 0.72f;

            _fallbackArm = new GameObject("CastingArm").transform;
            _fallbackArm.SetParent(_root, false);
            _fallbackArm.localPosition = new Vector3(-0.43f, 0.54f, 0f);
            _fallbackArm.localRotation = Quaternion.Euler(0f, 0f, 22f);
            var armRenderer = _fallbackArm.gameObject.AddComponent<SpriteRenderer>();
            armRenderer.sprite = RuntimeSpriteFactory.CreateRect(new Color(0.54f, 0.34f, 0.24f), 38, 14);
            armRenderer.sortingOrder = 134;
            _fallbackArm.localScale = new Vector3(0.95f, 0.22f, 1f);

            var staff = new GameObject("Staff");
            staff.transform.SetParent(_root, false);
            staff.transform.localPosition = new Vector3(0.58f, 0.12f, 0f);
            staff.transform.localScale = new Vector3(0.10f, 1.42f, 1f);
            var staffRenderer = staff.AddComponent<SpriteRenderer>();
            staffRenderer.sprite = RuntimeSpriteFactory.CreateRect(new Color(0.97f, 0.72f, 0.13f), 8, 64);
            staffRenderer.sortingOrder = 132;

            _muzzle = new GameObject("Muzzle").transform;
            _muzzle.SetParent(_root, false);
            _muzzle.localPosition = new Vector3(-0.92f, 0.95f, 0f);
        }

        static Sprite LoadFirstSprite(params string[] paths)
        {
            foreach (var path in paths)
            {
                var sprite = Resources.Load<Sprite>(path);
                if (sprite != null)
                {
                    return sprite;
                }

                var sprites = Resources.LoadAll<Sprite>(path);
                if (sprites != null && sprites.Length > 0)
                {
                    return sprites[0];
                }

                var texture = Resources.Load<Texture2D>(path);
                if (texture != null)
                {
                    return Sprite.Create(
                        texture,
                        new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.12f),
                        100f,
                        0,
                        SpriteMeshType.FullRect);
                }
            }

            return null;
        }

        static void CreateCircle(Transform parent, Vector2 localPosition, float scale, Color color, int sortingOrder)
        {
            var go = new GameObject("CloudPuff");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = Vector3.one * scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.CreateCircle(color, 32, 0.94f);
            renderer.sortingOrder = sortingOrder;
        }
    }

    public sealed class RunnerRaOrbProjectile : MonoBehaviour
    {
        RunnerPlayerController _player;
        Vector3 _velocity;
        AudioClip _impactClip;
        float _groundY;
        float _life = 5f;
        float _baseScale;
        Transform _glow;

        public void Configure(
            RunnerPlayerController player,
            Sprite customSprite,
            Vector3 startPosition,
            Vector3 velocity,
            AudioClip impactClip,
            float groundY)
        {
            _player = player;
            _velocity = velocity;
            _impactClip = impactClip;
            _groundY = groundY;
            transform.position = startPosition;

            var renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 136;
            renderer.sprite = customSprite != null
                ? customSprite
                : RuntimeSpriteFactory.CreateCircle(new Color(1f, 0.83f, 0.14f), 32, 0.96f);

            if (customSprite != null)
            {
                _baseScale = 0.50f / Mathf.Max(0.05f, customSprite.bounds.size.x);
            }
            else
            {
                _baseScale = 0.28f;
            }

            transform.localScale = Vector3.one * _baseScale;

            BuildGlow();
            BuildTrail();
        }

        void Update()
        {
            if (_player == null)
            {
                Destroy(gameObject);
                return;
            }

            _life -= Time.deltaTime;
            if (_life <= 0f)
            {
                Impact(transform.position);
                return;
            }

            var previous = transform.position;
            _velocity += Vector3.down * 3.4f * Time.deltaTime;
            var next = previous + _velocity * Time.deltaTime;
            var movement = next - previous;

            if (movement.sqrMagnitude > 0.0001f)
            {
                var hit = Physics2D.Raycast(previous, movement.normalized, movement.magnitude, GameLayers.EnvironmentMask);
                if (hit.collider != null)
                {
                    Impact(hit.point);
                    return;
                }
            }

            transform.position = next;

            var pulse = 1f + Mathf.Sin(Time.time * 24f) * 0.09f;
            transform.localScale = Vector3.one * (_baseScale * pulse);
            if (_glow != null)
            {
                var glowPulse = 1.42f + Mathf.Abs(Mathf.Sin(Time.time * 17f)) * 0.22f;
                _glow.localScale = Vector3.one * glowPulse;
            }

            var playerCenter = _player.transform.position + Vector3.up * 0.35f;
            if (!_player.IsDead && Vector2.Distance(transform.position, playerCenter) < 0.58f)
            {
                _player.TakeHit();
                Impact(transform.position);
                return;
            }

            if (transform.position.y <= _groundY)
            {
                Impact(new Vector3(transform.position.x, _groundY, 0f));
            }
        }

        void BuildGlow()
        {
            _glow = new GameObject("OrbGlow").transform;
            _glow.SetParent(transform, false);
            var glowRenderer = _glow.gameObject.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = RuntimeSpriteFactory.CreateCircle(new Color(0.30f, 0.94f, 1f), 32, 0.94f);
            glowRenderer.sortingOrder = 135;
            glowRenderer.color = new Color(0.30f, 0.94f, 1f, 0.40f);
        }

        void BuildTrail()
        {
            var trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = 0.23f;
            trail.startWidth = 0.22f;
            trail.endWidth = 0.02f;
            trail.minVertexDistance = 0.04f;
            trail.sortingOrder = 134;
            trail.textureMode = LineTextureMode.Stretch;

            var shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                trail.material = new Material(shader);
            }

            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(1f, 0.94f, 0.42f), 0f),
                    new GradientColorKey(new Color(0.24f, 0.91f, 1f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0.88f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });
            trail.colorGradient = gradient;
        }

        void Impact(Vector3 point)
        {
            RunnerRaImpactFx.Spawn(point, false, _impactClip);
            Destroy(gameObject);
        }
    }

    public sealed class RunnerRaStrike : MonoBehaviour
    {
        RunnerPlayerController _player;
        SpriteRenderer _beam;
        SpriteRenderer _warningDisc;
        AudioClip _strikeClip;
        AudioClip _impactClip;
        float _groundY;
        float _timer = 0.86f;
        bool _struck;

        public void Configure(
            RunnerPlayerController player,
            AudioClip strikeClip,
            AudioClip impactClip,
            float groundY)
        {
            _player = player;
            _strikeClip = strikeClip;
            _impactClip = impactClip;
            _groundY = groundY;

            _beam = gameObject.AddComponent<SpriteRenderer>();
            _beam.sprite = RuntimeSpriteFactory.CreateRect(Color.white, 64, 64);
            _beam.sortingOrder = GameSorting.Vfx + 5;
            _beam.color = new Color(1f, 0.82f, 0.20f, 0.18f);
            transform.localScale = new Vector3(0.12f, 8.5f, 1f);

            var warning = new GameObject("SolarWarning");
            warning.transform.SetParent(transform, false);
            warning.transform.localPosition = new Vector3(0f, -0.42f, 0f);
            warning.transform.localScale = new Vector3(5.6f, 0.11f, 1f);
            _warningDisc = warning.AddComponent<SpriteRenderer>();
            _warningDisc.sprite = RuntimeSpriteFactory.CreateCircle(new Color(1f, 0.44f, 0.10f), 48, 0.95f);
            _warningDisc.sortingOrder = GameSorting.Vfx + 4;
            _warningDisc.color = new Color(1f, 0.44f, 0.10f, 0.34f);
        }

        void Update()
        {
            _timer -= Time.deltaTime;

            if (!_struck)
            {
                var pulse = 0.14f + Mathf.Abs(Mathf.Sin(Time.time * 19f)) * 0.23f;
                _beam.color = new Color(1f, 0.77f, 0.16f, pulse);
                if (_warningDisc != null)
                {
                    var discPulse = 0.72f + Mathf.Abs(Mathf.Sin(Time.time * 14f)) * 0.30f;
                    _warningDisc.transform.localScale = new Vector3(5.6f * discPulse, 0.11f * discPulse, 1f);
                }

                if (_timer <= 0f)
                {
                    Strike();
                }

                return;
            }

            var alpha = Mathf.Clamp01((_timer + 0.26f) / 0.26f);
            _beam.color = new Color(1f, 0.96f, 0.64f, alpha);
            if (_warningDisc != null)
            {
                _warningDisc.color = new Color(1f, 0.58f, 0.18f, alpha * 0.55f);
            }

            if (_timer <= -0.26f)
            {
                Destroy(gameObject);
            }
        }

        void Strike()
        {
            _struck = true;
            _beam.color = new Color(1f, 0.98f, 0.72f, 1f);
            transform.localScale = new Vector3(0.86f, 8.5f, 1f);

            RunnerRaImpactFx.Spawn(new Vector3(transform.position.x, _groundY, 0f), true, _impactClip);
            RunnerRaImpactFx.PlayClip(_strikeClip, 0.62f);

            if (_player != null && Mathf.Abs(_player.transform.position.x - transform.position.x) < 0.92f)
            {
                _player.TakeHit();
            }
        }
    }

    public sealed class RunnerRaImpactFx : MonoBehaviour
    {
        SpriteRenderer[] _rings;
        float _life;
        float _duration;
        bool _large;

        public static void Spawn(Vector3 position, bool large, AudioClip clip)
        {
            var go = new GameObject(large ? "SolarImpactFX" : "OrbImpactFX");
            go.transform.position = position;
            var fx = go.AddComponent<RunnerRaImpactFx>();
            fx.Build(large, clip);
        }

        public static void PlayClip(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            var audioObject = new GameObject("RaOneShotAudio");
            var source = audioObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = volume;
            source.clip = clip;
            source.Play();
            Object.Destroy(audioObject, clip.length + 0.08f);
        }

        void Build(bool large, AudioClip clip)
        {
            _large = large;
            _duration = large ? 0.42f : 0.25f;
            _life = _duration;
            _rings = new SpriteRenderer[large ? 4 : 3];

            for (var i = 0; i < _rings.Length; i++)
            {
                var ringObject = new GameObject($"ImpactRing_{i}");
                ringObject.transform.SetParent(transform, false);
                ringObject.transform.localScale = Vector3.one * (0.25f + i * 0.16f);
                var renderer = ringObject.AddComponent<SpriteRenderer>();
                renderer.sprite = RuntimeSpriteFactory.CreateCircle(
                    i % 2 == 0 ? new Color(1f, 0.86f, 0.23f) : new Color(0.27f, 0.91f, 1f),
                    32,
                    0.92f);
                renderer.sortingOrder = GameSorting.Vfx + 8 + i;
                _rings[i] = renderer;
            }

            PlayClip(clip, large ? 0.72f : 0.42f);
        }

        void Update()
        {
            _life -= Time.deltaTime;
            var t = 1f - Mathf.Clamp01(_life / _duration);
            var baseGrowth = _large ? 3.8f : 1.8f;

            for (var i = 0; i < _rings.Length; i++)
            {
                var renderer = _rings[i];
                if (renderer == null)
                {
                    continue;
                }

                var delayedT = Mathf.Clamp01(t * 1.25f - i * 0.08f);
                renderer.transform.localScale = Vector3.one * Mathf.Lerp(0.18f + i * 0.10f, baseGrowth + i * 0.38f, delayedT);
                var color = renderer.color;
                color.a = 1f - delayedT;
                renderer.color = color;
            }

            if (_life <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public static class RunnerRaAudioFactory
    {
        const int SampleRate = 22050;

        public static AudioClip CreateSweep(string name, float duration, float fromHz, float toHz, float amplitude)
        {
            var sampleCount = Mathf.Max(1, Mathf.CeilToInt(duration * SampleRate));
            var data = new float[sampleCount];
            var phase = 0f;

            for (var i = 0; i < sampleCount; i++)
            {
                var t = i / (float)(sampleCount - 1);
                var frequency = Mathf.Lerp(fromHz, toHz, t * t);
                phase += 2f * Mathf.PI * frequency / SampleRate;
                var envelope = Mathf.Sin(Mathf.PI * t) * (1f - 0.25f * t);
                var sparkle = Mathf.Sin(phase * 2.03f) * 0.22f;
                data[i] = (Mathf.Sin(phase) + sparkle) * envelope * amplitude;
            }

            var clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public static AudioClip CreateImpact(string name, float duration, float baseHz, float amplitude)
        {
            var sampleCount = Mathf.Max(1, Mathf.CeilToInt(duration * SampleRate));
            var data = new float[sampleCount];
            var phase = 0f;

            for (var i = 0; i < sampleCount; i++)
            {
                var t = i / (float)(sampleCount - 1);
                var frequency = Mathf.Lerp(baseHz * 2.2f, baseHz, t);
                phase += 2f * Mathf.PI * frequency / SampleRate;
                var envelope = Mathf.Exp(-6.5f * t);
                var noise = (Random.value * 2f - 1f) * 0.32f;
                data[i] = (Mathf.Sin(phase) * 0.82f + noise) * envelope * amplitude;
            }

            var clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
