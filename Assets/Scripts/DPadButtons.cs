using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPadButtons : MonoBehaviour
{
    public enum Inputs { None, Up, Down, Left, Right };
    [HideInInspector]
    public Inputs currentInput;

    private float lastX;
    private float lastY;
    private float currentX;
    private float currentY;

    private bool useController2;

    // Start is called before the first frame update
    void Start()
    {
        lastX = lastY = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (useController2)
        {
            currentX = Input.GetAxisRaw("DPadX2");
            currentY = Input.GetAxisRaw("DPadY2");
        }
        else
        {
            currentX = Input.GetAxisRaw("DPadX");
            currentY = Input.GetAxisRaw("DPadY");
        }
    }

    public void SwapControllerNumber(bool setting)
    {
        useController2 = setting;
        if (useController2)
        {
            SetControls controls = GetComponent<SetControls>();
            controls.gamepadControls.Punch = controls.gamepadControls.Punch + 20;
            controls.gamepadControls.Kick = controls.gamepadControls.Kick + 20;
            controls.gamepadControls.Throw = controls.gamepadControls.Throw + 20;
            controls.gamepadControls.Pause = controls.gamepadControls.Pause + 20;
        }
        else
        {
            SetControls controls = GetComponent<SetControls>();
            controls.gamepadControls.Punch = controls.gamepadControls.Punch - 20;
            controls.gamepadControls.Kick = controls.gamepadControls.Kick - 20;
            controls.gamepadControls.Throw = controls.gamepadControls.Throw - 20;
            controls.gamepadControls.Pause = controls.gamepadControls.Pause - 20;
        }
    }

    public Inputs DPadDown()
    {
        if (currentY == 1.0f && lastY != 1.0f)
        {
            return Inputs.Up;
        }
        else if (currentY == -1.0f && lastY != -1.0f)
        {
            return Inputs.Down;
        }
        if (currentX == -1.0f && lastX != -1.0f)
        {
            return Inputs.Left;
        }
        else if (currentX == 1.0f && lastX != 1.0f)
        {
            return Inputs.Right;
        }
        return Inputs.None;
    }

    public Inputs DPadUp()
    {
        if (currentY != 1.0f && lastY == 1.0f)
        {
            return Inputs.Up;
        }
        else if (currentY != -1.0f && lastY == -1.0f)
        {
            return Inputs.Down;
        }
        if (currentX != -1.0f && lastX == -1.0f)
        {
            return Inputs.Left;
        }
        else if (currentX != 1.0f && lastX == 1.0f)
        {
            return Inputs.Right;
        }
        return Inputs.None;
    }

    public void ClearInputs()
    {
        lastY = currentY;
        lastX = currentX;
    }
}
