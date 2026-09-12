using Anubis.Characters;
using Anubis.Combat;
using Anubis.Core;
using UnityEngine;

namespace Anubis.AI
{
    public sealed class EnemyController : MonoBehaviour
    {
        public EnemyDefinition Definition { get; private set; }
        public TopDownMotor Motor { get; private set; }
        public Health Health { get; private set; }
        public StateMachine Machine { get; } = new();
        public EnemyIdleState Idle { get; private set; }
        public EnemyChaseState Chase { get; private set; }
        public EnemyKeepDistanceState KeepDistance { get; private set; }
        public EnemyAttackState Attack { get; private set; }
        public EnemyDeadState Dead { get; private set; }
        public ObjectPool Pool { get; private set; }
        public Health TargetHealth { get; private set; }

        public bool TargetAlive => TargetHealth != null && TargetHealth.IsAlive;
        public Vector2 ToTarget => TargetAlive ? (Vector2)TargetHealth.transform.position - (Vector2)transform.position : Vector2.zero;
        public bool HasTargetInDetectRange => TargetAlive && ToTarget.magnitude <= Definition.DetectRange;
        public bool CanAttack => _attackCooldown <= 0f;

        GameSignals _signals;
        float _attackCooldown;
        SpriteRenderer _renderer;

        public void Bind(EnemyDefinition definition, Health target, GameSignals signals, ObjectPool pool)
        {
            Definition = definition;
            TargetHealth = target;
            _signals = signals;
            Pool = pool;

            var body = gameObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = definition.ColliderRadius;

            var knockback = gameObject.AddComponent<KnockbackBody>();
            knockback.Initialize(body);

            Health = gameObject.AddComponent<Health>();
            Health.Configure(TeamId.Enemy, definition.MaxHealth, 0.08f, signals);
            Health.Died += OnDied;
            Health.Damaged += OnDamaged;

            Motor = gameObject.AddComponent<TopDownMotor>();
            Motor.Configure(body, definition.MoveSpeed, definition.Acceleration, 32f);

            Idle = new EnemyIdleState(this);
            Chase = new EnemyChaseState(this);
            KeepDistance = new EnemyKeepDistanceState(this);
            Attack = new EnemyAttackState(this);
            Dead = new EnemyDeadState(this);

            CreateVisual(definition);
            gameObject.layer = GameLayers.Enemy;
            gameObject.tag = "Enemy";
            Machine.ChangeState(Idle);
        }

        public IState ResolveCombatState()
        {
            return Definition.Archetype == EnemyArchetype.Ranged ? KeepDistance : Chase;
        }

        public void NotifyAttackUsed()
        {
            _attackCooldown = Definition.AttackCooldown;
        }

        void Update()
        {
            if (_attackCooldown > 0f)
            {
                _attackCooldown -= Time.deltaTime;
            }

            Machine.Tick(Time.deltaTime);
        }

        void OnDamaged(DamageInfo damage)
        {
            if (TryGetComponent<KnockbackBody>(out var knockback))
            {
                knockback.Apply(damage.Direction, damage.Knockback);
            }

            if (_renderer != null)
            {
                _renderer.color = Color.white;
            }
        }

        void OnDied()
        {
            Machine.ChangeState(Dead);
            _signals.EnemyKilled.Raise(new EnemyKillInfo(Definition.Id, transform.position));
            Destroy(gameObject, 0.35f);
        }

        void CreateVisual(EnemyDefinition definition)
        {
            var view = new GameObject("View");
            view.transform.SetParent(transform, false);
            view.transform.localScale = Vector3.one * definition.VisualScale;
            _renderer = view.AddComponent<SpriteRenderer>();
            _renderer.sprite = RuntimeSpriteFactory.CreateCircle(definition.BodyColor);
            _renderer.sortingOrder = GameSorting.Entities;
        }

        void LateUpdate()
        {
            if (_renderer == null || !Health.IsAlive)
            {
                return;
            }

            _renderer.color = Color.Lerp(_renderer.color, Definition.BodyColor, Time.deltaTime * 8f);
        }
    }
}
