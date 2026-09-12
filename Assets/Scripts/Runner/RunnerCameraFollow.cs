using UnityEngine;

namespace Anubis.Runner
{
    public sealed class RunnerCameraFollow : MonoBehaviour
    {
        Transform _target;
        Vector3 _velocity;

        public void Configure(Transform target)
        {
            _target = target;
        }

        void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            var desired = new Vector3(_target.position.x + 4.2f, 0.2f, -10f);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, 0.12f);
        }
    }
}
