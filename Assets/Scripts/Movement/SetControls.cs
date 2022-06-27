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
}