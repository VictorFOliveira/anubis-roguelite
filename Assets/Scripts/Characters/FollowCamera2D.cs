using UnityEngine;

namespace Anubis.Characters
{
    public sealed class FollowCamera2D : MonoBehaviour
    {
        Transform _target;
        Vector2 _min;
        Vector2 _max;
        float _damp = 8f;
        Vector3 _velocity;

        public void Configure(Transform target, Vector2 arenaSize, float damp = 8f)
        {
            _target = target;
            _damp = damp;
            var half = arenaSize * 0.5f;
            var camera = GetComponent<Camera>();
            var vertical = camera.orthographicSize;
            var horizontal = vertical * camera.aspect;
            _min = new Vector2(-half.x + horizontal, -half.y + vertical);
            _max = new Vector2(half.x - horizontal, half.y - vertical);
            if (_min.x > _max.x)
            {
                _min.x = _max.x = 0f;
            }

            if (_min.y > _max.y)
            {
                _min.y = _max.y = 0f;
            }
        }

        void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            var target = _target.position;
            target.x = Mathf.Clamp(target.x, _min.x, _max.x);
            target.y = Mathf.Clamp(target.y, _min.y, _max.y);
            target.z = -10f;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, 1f / _damp);
        }
    }
}
