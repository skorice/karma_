using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader
{
    public Vector2 MovementInput { get; private set; }

    public void Read()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector2 input = Vector2.zero;

        if (keyboard.wKey.isPressed) input.y += 1;
        if (keyboard.sKey.isPressed) input.y -= 1;
        if (keyboard.aKey.isPressed) input.x -= 1;
        if (keyboard.dKey.isPressed) input.x += 1;

        MovementInput = input.magnitude > 1f ? input.normalized : input;
    }
}