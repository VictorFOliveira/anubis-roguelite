using UnityEngine;
using UnityEngine.InputSystem;

namespace Anubis.Core
{
    public static class GameInputFactory
    {
        public static InputActionAsset Create()
        {
            var text = Resources.Load<TextAsset>("Input/GameInput");
            if (text != null && !string.IsNullOrWhiteSpace(text.text))
            {
                return InputActionAsset.FromJson(text.text);
            }

            return CreateProgrammatic();
        }

        static InputActionAsset CreateProgrammatic()
        {
            var asset = ScriptableObject.CreateInstance<InputActionAsset>();
            var player = asset.AddActionMap("Player");
            var ui = asset.AddActionMap("UI");

            var move = player.AddAction("Move", InputActionType.Value, expectedControlLayout: "Vector2");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            move.AddBinding("<Gamepad>/leftStick");

            var look = player.AddAction("Look", InputActionType.Value, expectedControlLayout: "Vector2");
            look.AddBinding("<Gamepad>/rightStick");

            var point = player.AddAction("Point", InputActionType.Value, expectedControlLayout: "Vector2");
            point.AddBinding("<Mouse>/position");

            var attack = player.AddAction("Attack", InputActionType.Button);
            attack.AddBinding("<Mouse>/leftButton");
            attack.AddBinding("<Keyboard>/j");
            attack.AddBinding("<Gamepad>/buttonWest");
            attack.AddBinding("<Gamepad>/rightTrigger");

            var dash = player.AddAction("Dash", InputActionType.Button);
            dash.AddBinding("<Keyboard>/space");
            dash.AddBinding("<Keyboard>/leftShift");
            dash.AddBinding("<Gamepad>/buttonEast");
            dash.AddBinding("<Gamepad>/leftShoulder");

            var interact = player.AddAction("Interact", InputActionType.Button);
            interact.AddBinding("<Keyboard>/e");
            interact.AddBinding("<Gamepad>/buttonNorth");

            var navigate = ui.AddAction("Navigate", InputActionType.Value, expectedControlLayout: "Vector2");
            navigate.AddBinding("<Gamepad>/leftStick");
            navigate.AddBinding("<Gamepad>/dpad");

            var submit = ui.AddAction("Submit", InputActionType.Button);
            submit.AddBinding("<Keyboard>/enter");
            submit.AddBinding("<Gamepad>/buttonSouth");

            var cancel = ui.AddAction("Cancel", InputActionType.Button);
            cancel.AddBinding("<Keyboard>/escape");
            cancel.AddBinding("<Gamepad>/buttonEast");

            return asset;
        }
    }
}
