using System;
using Anubis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Anubis.Runner
{
    public enum RunnerUpgradeType
    {
        Shield,
        HighJump,
        HorusLeap
    }

    public sealed class RunnerPlayerController : MonoBehaviour
    {
        static readonly Vector2 StandingColliderSize = new(0.72f, 1.72f);
        static readonly Vector2 StandingColliderOffset = new(0f, 0.02f);
        static readonly Vector2 SlidingColliderSize = new(1.24f, 0.78f);
        static readonly Vector2 SlidingColliderOffset = new(0.18f, -0.45f);

        const float CoyoteDuration = 0.12f;
        const float JumpBufferDuration = 0.12f;
        const float SlideMinDuration = 0.22f;
        const float SlideCooldownDuration = 0.12f;

        Rigidbody2D _body;
        CapsuleCollider2D _collider;
        RunnerAnubisView _view;
        float _startX;
        float _invulnerability;
        float _boostTimer;
        float _coyoteTimer;
        float _jumpBufferTimer;
        float _slideTimer;
        float _slideCooldown;
        bool _dead;
        bool _wasGrounded;
        bool _slideHeld;

        public event Action Died;
        public event Action<string> UpgradeCollected;

        public float BaseRunSpeed { get; private set; } = 7f;
        public float DistanceMeters => Mathf.Max(0f, (transform.position.x - _startX) * 10f);
        public int ShieldCharges { get; private set; }
        public float JumpMultiplier { get; private set; } = 1f;
        public bool IsDead => _dead;
        public bool IsGrounded { get; private set; }
        public bool IsSliding { get; private set; }
        public float CurrentRunSpeed { get; private set; }

        public void Configure()
        {
            gameObject.layer = GameLayers.Player;
            _startX = transform.position.x;

            _body = gameObject.AddComponent<Rigidbody2D>();
            _body.gravityScale = 1f;
            _body.freezeRotation = true;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;

            _collider = gameObject.AddComponent<CapsuleCollider2D>();
            _collider.direction = CapsuleDirection2D.Vertical;
            _collider.size = StandingColliderSize;
            _collider.offset = StandingColliderOffset;

            _view = gameObject.AddComponent<RunnerAnubisView>();
            _view.Build();
        }

        void Update()
        {
            if (_dead)
            {
                return;
            }

            ReadGroundState();
            TickTimers();
            ReadInput();
            ResolveMovementActions();

            _view.Tick(
                _body.linearVelocity.y,
                IsGrounded,
                _invulnerability > 0f,
                _boostTimer > 0f,
                IsSliding,
                CurrentRunSpeed);

            if (transform.position.y < -8.5f)
            {
                Die();
            }
        }

        void ReadGroundState()
        {
            _wasGrounded = IsGrounded;
            IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.02f, GameLayers.EnvironmentMask);

            if (!_wasGrounded && IsGrounded)
            {
                _view.TriggerLanding();
            }
        }

        void TickTimers()
        {
            if (_invulnerability > 0f) _invulnerability -= Time.deltaTime;
            if (_boostTimer > 0f) _boostTimer -= Time.deltaTime;
            if (_jumpBufferTimer > 0f) _jumpBufferTimer -= Time.deltaTime;
            if (_slideCooldown > 0f) _slideCooldown -= Time.deltaTime;

            if (IsGrounded)
            {
                _coyoteTimer = CoyoteDuration;
            }
            else if (_coyoteTimer > 0f)
            {
                _coyoteTimer -= Time.deltaTime;
            }

            if (IsSliding)
            {
                _slideTimer += Time.deltaTime;
            }
        }

        void ReadInput()
        {
            var keyboard = Keyboard.current;
            var gamepad = Gamepad.current;

            var jumpPressed = keyboard != null &&
                (keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame);
            jumpPressed |= gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;

            var jumpReleased = keyboard != null &&
                (keyboard.spaceKey.wasReleasedThisFrame || keyboard.wKey.wasReleasedThisFrame || keyboard.upArrowKey.wasReleasedThisFrame);
            jumpReleased |= gamepad != null && gamepad.buttonSouth.wasReleasedThisFrame;

            _slideHeld = keyboard != null &&
                (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed || keyboard.leftCtrlKey.isPressed);
            _slideHeld |= gamepad != null &&
                (gamepad.dpad.down.isPressed || gamepad.leftStick.down.isPressed || gamepad.rightShoulder.isPressed);

            if (jumpPressed)
            {
                _jumpBufferTimer = JumpBufferDuration;
            }

            // Soltar o botão no meio da subida corta o pulo: toque curto = pulo baixo, segurar = pulo alto.
            if (jumpReleased && _body.linearVelocity.y > 2f)
            {
                var velocity = _body.linearVelocity;
                velocity.y *= 0.48f;
                _body.linearVelocity = velocity;
            }
        }

        void ResolveMovementActions()
        {
            if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
            {
                if (IsSliding)
                {
                    StopSlide(true);
                }

                var velocity = _body.linearVelocity;
                velocity.y = 11.4f * JumpMultiplier;
                _body.linearVelocity = velocity;
                _jumpBufferTimer = 0f;
                _coyoteTimer = 0f;
                IsGrounded = false;
                _view.TriggerJump();
                return;
            }

            if (IsGrounded)
            {
                if (!IsSliding && _slideHeld && _slideCooldown <= 0f)
                {
                    StartSlide();
                }

                if (IsSliding && _slideTimer >= SlideMinDuration && !_slideHeld && CanStandUp())
                {
                    StopSlide(false);
                }

                return;
            }

            if (IsSliding)
            {
                StopSlide(true);
            }

            // ↓ no ar funciona como mergulho para aterrissar rapidamente e preparar outro salto/slide.
            if (_slideHeld && _body.linearVelocity.y < 2.5f)
            {
                var velocity = _body.linearVelocity;
                velocity.y = Mathf.Min(velocity.y - 26f * Time.deltaTime, -13.5f);
                _body.linearVelocity = velocity;
            }
        }

        bool CanStandUp()
        {
            var center = (Vector2)transform.position + StandingColliderOffset;
            var overlap = Physics2D.OverlapCapsule(
                center,
                StandingColliderSize * 0.96f,
                CapsuleDirection2D.Vertical,
                0f,
                GameLayers.EnvironmentMask);
            return overlap == null;
        }

        void StartSlide()
        {
            IsSliding = true;
            _slideTimer = 0f;
            _collider.direction = CapsuleDirection2D.Horizontal;
            _collider.size = SlidingColliderSize;
            _collider.offset = SlidingColliderOffset;
            _view.TriggerSlide();
        }

        void StopSlide(bool force)
        {
            if (!IsSliding)
            {
                return;
            }

            if (!force && !CanStandUp())
            {
                return;
            }

            IsSliding = false;
            _slideCooldown = SlideCooldownDuration;
            _collider.direction = CapsuleDirection2D.Vertical;
            _collider.size = StandingColliderSize;
            _collider.offset = StandingColliderOffset;
        }

        void FixedUpdate()
        {
            if (_dead)
            {
                return;
            }

            var difficulty = Mathf.Min(4.5f, DistanceMeters / 1800f);
            var speed = BaseRunSpeed + difficulty;

            if (_boostTimer > 0f)
            {
                speed *= 1.7f;
            }

            if (IsSliding)
            {
                speed += 0.85f;
            }

            CurrentRunSpeed = speed;
            var velocity = _body.linearVelocity;
            velocity.x = speed;
            _body.linearVelocity = velocity;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (_dead)
            {
                return;
            }

            var enemy = collision.collider.GetComponent<RunnerEnemy>();
            if (enemy == null)
            {
                enemy = collision.collider.GetComponentInParent<RunnerEnemy>();
            }

            if (enemy == null)
            {
                return;
            }

            var descending = _body.linearVelocity.y <= 0.5f;
            var aboveEnemy = transform.position.y > enemy.transform.position.y + 0.45f;
            if (!IsSliding && descending && aboveEnemy)
            {
                enemy.Stomp();
                var velocity = _body.linearVelocity;
                velocity.y = 9.2f;
                _body.linearVelocity = velocity;
                _view.TriggerStompBounce();
                return;
            }

            TakeHit();
        }

        public void TakeHit()
        {
            if (_dead || _invulnerability > 0f)
            {
                return;
            }

            if (ShieldCharges > 0)
            {
                ShieldCharges--;
                _invulnerability = 1.15f;
                UpgradeCollected?.Invoke("Escudo de Ma'at absorveu o golpe");
                return;
            }

            Die();
        }

        public void ApplyUpgrade(RunnerUpgradeType upgrade)
        {
            if (_dead)
            {
                return;
            }

            switch (upgrade)
            {
                case RunnerUpgradeType.Shield:
                    ShieldCharges = Mathf.Min(3, ShieldCharges + 1);
                    UpgradeCollected?.Invoke("Escudo de Ma'at +1");
                    break;
                case RunnerUpgradeType.HighJump:
                    JumpMultiplier = Mathf.Min(1.75f, JumpMultiplier + 0.18f);
                    UpgradeCollected?.Invoke("Passo de Hórus: salto ampliado");
                    break;
                case RunnerUpgradeType.HorusLeap:
                    if (IsSliding) StopSlide(true);
                    transform.position += new Vector3(30f, 4.5f, 0f);
                    var velocity = _body.linearVelocity;
                    velocity.y = 13.5f;
                    _body.linearVelocity = velocity;
                    _boostTimer = 2.1f;
                    _view.TriggerJump();
                    UpgradeCollected?.Invoke("Voo de Hórus: +300 m!");
                    break;
            }
        }

        void Die()
        {
            if (_dead)
            {
                return;
            }

            _dead = true;
            if (IsSliding) StopSlide(true);
            _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
            _view.SetDead();
            Died?.Invoke();
        }
    }
}
