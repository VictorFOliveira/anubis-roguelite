using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Anubis.Core
{
    public sealed class GameInputReader : IDisposable
    {
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public Vector2 PointerScreen { get; private set; }
        public bool UsingGamepadAim { get; private set; }

        public event Action AttackPressed;
        public event Action DashPressed;
        public event Action InteractPressed;
        public event Action CancelPressed;

        readonly InputActionAsset _actions;
        readonly InputAction _move;
        readonly InputAction _look;
        readonly InputAction _pointer;
        readonly InputAction _attack;
        readonly InputAction _dash;
        readonly InputAction _interact;
        readonly InputAction _cancel;

        public GameInputReader(InputActionAsset actions)
        {
            _actions = actions;
            var player = _actions.FindActionMap("Player", true);
            var ui = _actions.FindActionMap("UI", true);

            _move = player.FindAction("Move", true);
            _look = player.FindAction("Look", true);
            _pointer = player.FindAction("Point", true);
            _attack = player.FindAction("Attack", true);
            _dash = player.FindAction("Dash", true);
            _interact = player.FindAction("Interact", true);
            _cancel = ui.FindAction("Cancel", true);

            _move.performed += OnMove;
            _move.canceled += OnMove;
            _look.performed += OnLook;
            _look.canceled += OnLook;
            _pointer.performed += OnPointer;
            _attack.performed += OnAttack;
            _dash.performed += OnDash;
            _interact.performed += OnInteract;
            _cancel.performed += OnCancel;

            _actions.Enable();
        }

        public void SetGameplayEnabled(bool enabled)
        {
            var player = _actions.FindActionMap("Player");
            if (enabled)
            {
                player.Enable();
            }
            else
            {
                player.Disable();
                Move = Vector2.zero;
            }
        }

        public void Dispose()
        {
            _move.performed -= OnMove;
            _move.canceled -= OnMove;
            _look.performed -= OnLook;
            _look.canceled -= OnLook;
            _pointer.performed -= OnPointer;
            _attack.performed -= OnAttack;
            _dash.performed -= OnDash;
            _interact.performed -= OnInteract;
            _cancel.performed -= OnCancel;
            _actions.Disable();
        }

        void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
            UsingGamepadAim = context.control?.device is Gamepad && Look.sqrMagnitude > 0.04f;
        }

        void OnPointer(InputAction.CallbackContext context)
        {
            PointerScreen = context.ReadValue<Vector2>();
            if (context.control?.device is Mouse)
            {
                UsingGamepadAim = false;
            }
        }

        void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackPressed?.Invoke();
            }
        }

        void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                DashPressed?.Invoke();
            }
        }

        void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InteractPressed?.Invoke();
            }
        }

        void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CancelPressed?.Invoke();
            }
        }
    }
}
