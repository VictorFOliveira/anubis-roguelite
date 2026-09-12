using UnityEngine;

namespace Anubis.Rooms
{
    public enum DoorDirection
    {
        North,
        South,
        East,
        West
    }

    public sealed class DoorController : MonoBehaviour
    {
        public DoorDirection Direction { get; private set; }
        public bool IsOpen { get; private set; }

        SpriteRenderer _renderer;
        BoxCollider2D _blocker;
        Color _closed = new(0.45f, 0.18f, 0.12f);
        Color _open = new(0.72f, 0.55f, 0.22f);

        public void Build(DoorDirection direction, Vector2 position, Vector2 size)
        {
            Direction = direction;
            transform.position = position;
            gameObject.tag = "Door";

            _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = Core.RuntimeSpriteFactory.CreateRect(_closed, 64, 24);
            _renderer.sortingOrder = Core.GameSorting.Environment;
            transform.localScale = new Vector3(size.x, size.y, 1f);

            _blocker = gameObject.AddComponent<BoxCollider2D>();
            _blocker.size = Vector2.one;
            SetOpen(false);
        }

        public void SetOpen(bool open)
        {
            IsOpen = open;
            if (_renderer != null)
            {
                _renderer.color = open ? _open : _closed;
            }

            if (_blocker != null)
            {
                _blocker.enabled = !open;
            }
        }
    }
}
