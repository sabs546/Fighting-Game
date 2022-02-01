using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetControls : MonoBehaviour
{
    public enum ControllerType { Keyboard, Controller };
    public ControllerType type; // Controller might need a new script, I'll leave it for later

    public KeyCode Up;
    public KeyCode Down;
    public KeyCode Left;
    public KeyCode Right;

    public KeyCode Punch;
    public KeyCode Kick;
    public KeyCode Throw;

    public void DisableKeyboard()
    {
        Up = KeyCode.None;
        Down = KeyCode.None;
        Left = KeyCode.None;
        Right = KeyCode.None;

        Punch = KeyCode.None;
        Kick = KeyCode.None;
        Throw = KeyCode.None;
    }
}