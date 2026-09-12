using Anubis.Characters;
using Anubis.Core;
using UnityEngine;

namespace Anubis.Rooms
{
    public sealed class RewardShrine : MonoBehaviour
    {
        GameSignals _signals;
        GameInputReader _input;
        Transform _player;
        bool _ready;
        bool _claimed;
        SpriteRenderer _renderer;

        public void Bind(GameSignals signals, GameInputReader input, Transform player)
        {
            _signals = signals;
            _input = input;
            _player = player;
            _input.InteractPressed += OnInteract;

            _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = RuntimeSpriteFactory.CreateDiamond(new Color(0.93f, 0.78f, 0.28f), 64);
            _renderer.sortingOrder = GameSorting.Environment + 1;
            transform.localScale = Vector3.one * 1.15f;

            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.55f;
            gameObject.tag = "Reward";
            gameObject.SetActive(false);
        }

        public void Reveal(Vector2 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
            _ready = true;
            _claimed = false;
            _signals.RewardReady.Raise();
        }

        void Update()
        {
            if (!_ready || _claimed || _renderer == null)
            {
                return;
            }

            var pulse = 0.9f + Mathf.Sin(Time.time * 4f) * 0.08f;
            transform.localScale = Vector3.one * pulse;
        }

        void OnInteract()
        {
            if (!_ready || _claimed || _player == null)
            {
                return;
            }

            if (Vector2.Distance(_player.position, transform.position) > 1.4f)
            {
                return;
            }

            Claim();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!_ready || _claimed)
            {
                return;
            }

            if (other.GetComponent<PlayerController>() != null)
            {
                Claim();
            }
        }

        void Claim()
        {
            _claimed = true;
            _ready = false;
            gameObject.SetActive(false);
            _signals.BlessingOfferOpened.Raise();
        }

        void OnDestroy()
        {
            if (_input != null)
            {
                _input.InteractPressed -= OnInteract;
            }
        }
    }
}
