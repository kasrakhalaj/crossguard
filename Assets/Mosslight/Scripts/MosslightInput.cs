using UnityEngine;
using UnityEngine.InputSystem;

namespace Mosslight
{
    // Keep device-specific code here. Player movement only receives simple values.
    public struct HeroInput
    {
        public float move;
        public bool jumpPressed, jumpHeld, attack, dash, interact;
    }

    public static class MosslightInput
    {
        public static HeroInput Read()
        {
            var k = Keyboard.current;
            var p = Gamepad.current;
            float move = 0;
            if (k != null)
            {
                if (k.aKey.isPressed || k.leftArrowKey.isPressed) move -= 1;
                if (k.dKey.isPressed || k.rightArrowKey.isPressed) move += 1;
            }
            if (p != null && Mathf.Abs(p.leftStick.x.ReadValue()) > 0.2f)
                move = p.leftStick.x.ReadValue();
            if (p != null && Mathf.Abs(p.dpad.x.ReadValue()) > 0.2f)
                move = p.dpad.x.ReadValue();
            return new HeroInput
            {
                move = move,
                jumpPressed = (k?.spaceKey.wasPressedThisFrame ?? false) || (p?.buttonSouth.wasPressedThisFrame ?? false),
                jumpHeld = (k?.spaceKey.isPressed ?? false) || (p?.buttonSouth.isPressed ?? false),
                attack = (k?.jKey.wasPressedThisFrame ?? false) || (Mouse.current?.leftButton.wasPressedThisFrame ?? false) || (p?.buttonWest.wasPressedThisFrame ?? false),
                dash = (k?.leftShiftKey.wasPressedThisFrame ?? false) || (k?.kKey.wasPressedThisFrame ?? false) || (p?.rightShoulder.wasPressedThisFrame ?? false),
                interact = (k?.eKey.wasPressedThisFrame ?? false) || (p?.buttonNorth.wasPressedThisFrame ?? false)
            };
        }
        public static bool Confirm => (Keyboard.current?.enterKey.wasPressedThisFrame ?? false) || (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false);
        public static bool Pause => (Keyboard.current?.escapeKey.wasPressedThisFrame ?? false) || (Gamepad.current?.startButton.wasPressedThisFrame ?? false);
        public static bool Restart => Keyboard.current?.rKey.wasPressedThisFrame ?? false;
    }
}
