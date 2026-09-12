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
        Rigidbody2D _body;
        CapsuleCollider2D _collider;
        RunnerAnubisView _view;
        float _startX;
        float _invulnerability;
        float _boostTimer;
        bool _dead;

        public event Action Died;
        public event Action<string> UpgradeCollected;

        public float BaseRunSpeed { get; private set; } = 7f;
        public float DistanceMeters => Mathf.Max(0f, (transform.position.x - _startX) * 10f);
        public int ShieldCharges { get; private set; }
        public float JumpMultiplier { get; private set; } = 1f;
        public bool IsDead => _dead;
        public bool IsGrounded { get; private set; }

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
            _collider.size = new Vector2(0.72f, 1.72f);
            _collider.offset = new Vector2(0f, 0.02f);

            _view = gameObject.AddComponent<RunnerAnubisView>();
            _view.Build();
        }

        void Update()
        {
            if (_dead)
            {
                return;
            }

            if (_invulnerability > 0f)
            {
                _invulnerability -= Time.deltaTime;
            }

            if (_boostTimer > 0f)
            {
                _boostTimer -= Time.deltaTime;
            }

            IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.02f, GameLayers.EnvironmentMask);

            var jumpPressed = Keyboard.current != null &&
                (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame);
            jumpPressed |= Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;

            if (jumpPressed && IsGrounded)
            {
                var velocity = _body.linearVelocity;
                velocity.y = 11.4f * JumpMultiplier;
                _body.linearVelocity = velocity;
            }

            _view.Tick(_body.linearVelocity.y, IsGrounded, _invulnerability > 0f, _boostTimer > 0f);

            if (transform.position.y < -8.5f)
            {
                Die();
            }
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
            if (descending && aboveEnemy)
            {
                enemy.Stomp();
                var velocity = _body.linearVelocity;
                velocity.y = 9.2f;
                _body.linearVelocity = velocity;
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
                    transform.position += Vector3.right * 30f;
                    var velocity = _body.linearVelocity;
                    velocity.y = 13.5f;
                    _body.linearVelocity = velocity;
                    _boostTimer = 2.1f;
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
            _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
            _view.SetDead();
            Died?.Invoke();
        }
    }
}
