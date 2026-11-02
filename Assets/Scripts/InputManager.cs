using UnityEngine.InputSystem;
using UnityEngine;
public class InputManager
{
    public static float sensitivity = 0.1f;
    public static float Vertical()
    {
        float value = 0f;
        if (Keyboard.current.wKey.isPressed)
        {
            value += 1f;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            value -= 1f;
        }
        return value;
    }
    public static float Horizontal()
    {
        float value = 0f;
        if (Keyboard.current.aKey.isPressed)
        {
            value -= 1f;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            value += 1f;
        }
        return value;
    }
    public static bool Jump()
    {
        return Keyboard.current.spaceKey.wasPressedThisFrame;
    }
    public static bool Interact()
    {
        return Keyboard.current.eKey.wasPressedThisFrame;
    }
    private static float xRotation = 0f;
    public static void Turn(GameObject player, GameObject camera)
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * sensitivity;
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.transform.Rotate(Vector3.up * mouseDelta.x);
    }
    public static bool LeftClick()
    {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }
    public static bool RightClick()
    {
        return Mouse.current.rightButton.wasPressedThisFrame;
    }
    public static bool LeftClickHeld()
    {
        return Mouse.current.leftButton.isPressed;
    }
    public static bool RightClickHeld()
    {
        return Mouse.current.rightButton.isPressed;
    }
    public static bool Run()
    {
        return Keyboard.current.leftShiftKey.isPressed;
    }
    public static bool Crouch()
    {
        return Keyboard.current.leftCtrlKey.isPressed;
    }
}
