using Anubis.Core;
using UnityEngine;

namespace Anubis.Runner
{
    public sealed class RunnerUpgradePickup : MonoBehaviour
    {
        RunnerUpgradeType _type;
        SpriteRenderer _renderer;
        Vector3 _basePosition;

        public void Configure(RunnerUpgradeType type)
        {
            _type = type;
            _basePosition = transform.position;

            _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = RuntimeSpriteFactory.CreateDiamond(ColorFor(type), 48);
            _renderer.sortingOrder = GameSorting.Vfx;
            transform.localScale = Vector3.one * 0.8f;

            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }

        void Update()
        {
            var position = _basePosition;
            position.y += Mathf.Sin(Time.time * 3.4f + transform.position.x) * 0.18f;
            transform.position = position;
            transform.Rotate(0f, 0f, 55f * Time.deltaTime);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<RunnerPlayerController>() ?? other.GetComponentInParent<RunnerPlayerController>();
            if (player == null)
            {
                return;
            }

            player.ApplyUpgrade(_type);
            Destroy(gameObject);
        }

        static Color ColorFor(RunnerUpgradeType type)
        {
            return type switch
            {
                RunnerUpgradeType.Shield => new Color(0.25f, 0.72f, 1f),
                RunnerUpgradeType.HighJump => new Color(0.45f, 1f, 0.58f),
                RunnerUpgradeType.HorusLeap => new Color(1f, 0.78f, 0.18f),
                _ => Color.white
            };
        }
    }
}
