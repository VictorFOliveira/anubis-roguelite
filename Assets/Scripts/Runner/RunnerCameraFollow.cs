using UnityEngine;

namespace Anubis.Runner
{
    public sealed class RunnerCameraFollow : MonoBehaviour
    {
        Transform _target;
        Vector3 _velocity;
        float _frontierX;

        public void Configure(Transform target)
        {
            _target = target;
            _frontierX = target != null ? target.position.x : 0f;
        }

        void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            // A câmera nunca recua: o jogador pode voltar um pouco dentro da tela
            // para esquivar dos projéteis sem fazer o cenário inteiro andar para trás.
            _frontierX = Mathf.Max(_frontierX, _target.position.x);
            var desired = new Vector3(_frontierX + 4.2f, 0.2f, -10f);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, 0.14f);
        }
    }
}
