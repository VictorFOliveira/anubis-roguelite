using Anubis.Combat;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Characters
{
    public sealed class PlayerController : MonoBehaviour
    {
        public CharacterDefinition Definition { get; private set; }
        public Health Health { get; private set; }
        public TopDownMotor Motor { get; private set; }
        public DashController Dash { get; private set; }
        public IPrimaryAttack PrimaryAttack { get; private set; }
        public ICharacterView View { get; private set; }
        public SpriteRenderer Body => View?.Body;

        GameInputReader _input;
        Camera _camera;
        GameSignals _signals;
        bool _inputBound;

        public void Bind(CharacterDefinition definition, GameInputReader input, Camera worldCamera, GameSignals signals, ObjectPool pool)
        {
            Definition = definition;
            _input = input;
            _camera = worldCamera;
            _signals = signals;

            var body = gameObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.linearDamping = 0f;

            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.36f;

            var knockback = gameObject.AddComponent<KnockbackBody>();
            knockback.Initialize(body);

            Health = gameObject.AddComponent<Health>();
            Health.Configure(TeamId.Player, definition.MaxHealth, definition.HitInvulnerability, signals);
            Health.Died += OnDied;
            Health.Damaged += OnDamaged;

            Motor = gameObject.AddComponent<TopDownMotor>();
            Motor.Configure(body, definition.MoveSpeed, definition.Acceleration, definition.Deceleration);

            Dash = gameObject.AddComponent<DashController>();
            Dash.Configure(definition, Motor, Health, body);

            PrimaryAttack = CharacterRuntimeFactory.AddPrimaryAttack(gameObject, definition, Motor, pool);
            View = CharacterRuntimeFactory.AddView(gameObject, definition);
            gameObject.layer = GameLayers.Player;
            gameObject.tag = "Player";

            _input.AttackPressed += OnAttack;
            _input.DashPressed += OnDash;
            _inputBound = true;
        }

        public void SetControlEnabled(bool enabled)
        {
            enabled = enabled && Health.IsAlive;
            _input.SetGameplayEnabled(enabled);
            Motor.SetLocked(!enabled);
        }

        public Vector2 AimDirection()
        {
            if (_input.UsingGamepadAim)
            {
                return _input.Look.sqrMagnitude > 0.05f ? _input.Look : Motor.Facing;
            }

            var world = _camera.ScreenToWorldPoint(_input.PointerScreen);
            var delta = (Vector2)world - (Vector2)transform.position;
            return delta.sqrMagnitude > 0.01f ? delta : Motor.Facing;
        }

        void Update()
        {
            if (Health == null || !Health.IsAlive)
            {
                return;
            }

            View?.SetDashing(Dash.IsDashing);
            View?.SetFacing(Motor.Facing);
            if (Dash.IsDashing)
            {
                return;
            }

            Motor.SetDesiredVelocity(_input.Move);
            var aim = AimDirection();
            if (_input.UsingGamepadAim || _input.Move.sqrMagnitude < 0.01f)
            {
                Motor.Face(aim);
            }
        }

        void OnAttack()
        {
            if (!Health.IsAlive || Dash.IsDashing)
            {
                return;
            }

            if (PrimaryAttack != null && PrimaryAttack.TryAttack(AimDirection()))
            {
                View?.PlayPrimaryAttack();
            }
        }

        void OnDash()
        {
            if (!Health.IsAlive)
            {
                return;
            }

            var direction = _input.Move.sqrMagnitude > 0.05f ? _input.Move : AimDirection();
            Dash.TryDash(direction);
        }

        void OnDamaged(DamageInfo damage)
        {
            if (TryGetComponent<KnockbackBody>(out var knockback))
            {
                knockback.Apply(damage.Direction, damage.Knockback);
            }
        }

        void OnDied()
        {
            Motor.SetLocked(true);
            _input.SetGameplayEnabled(false);
            _signals.PlayerDied.Raise();
        }

        void OnDestroy()
        {
            if (_inputBound && _input != null)
            {
                _input.AttackPressed -= OnAttack;
                _input.DashPressed -= OnDash;
            }

            if (Health != null)
            {
                Health.Died -= OnDied;
                Health.Damaged -= OnDamaged;
            }
        }
    }
}
