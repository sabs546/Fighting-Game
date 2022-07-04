using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetControls : MonoBehaviour
{
    public enum ControllerType { Keyboard, Controller };
    public ControllerType type; // Controller might need a new script, I'll leave it for later

    public KeyboardControls keyboardControls;
    public GamepadControls gamepadControls;

    public void SetControlType(bool padEnabled)
    {
        type = padEnabled ? ControllerType.Controller : ControllerType.Keyboard;
    }
}

[System.Serializable]
public struct KeyboardControls
{
    public KeyCode Up;
    public KeyCode Down;
    public KeyCode Left;
    public KeyCode Right;

    public KeyCode Punch;
    public KeyCode Kick;
    public KeyCode Throw;

    public KeyCode Pause;
}

[System.Serializable]
public struct GamepadControls
{
    public KeyCode Punch;
    public KeyCode Kick;
    public KeyCode Throw;

    public KeyCode Pause;
}