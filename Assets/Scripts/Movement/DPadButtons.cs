using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DPadButtons : MonoBehaviour
{
    public enum Inputs { None, Up, Down, Left, Right };
    [HideInInspector]
    public Inputs currentInput;

    private float lastX;
    private float lastY;
    private float currentX;
    private float currentY;

    private float msSendTime;
    public int delayFrames;
    private int currentDelay; // The delay on the key you just hit

    private bool useController2;
    private SetControls controls;

    // Start is called before the first frame update
    void Start()
    {
        lastX = lastY = 0.0f;
        controls = GetComponent<SetControls>();
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldRules.online)
        {
            if (delayFrames > 0)
            {
                // Delay moment
                delayFrames--;
                return;
            }
            msSendTime = PhotonNetwork.GetPing();
            msSendTime /= 1000.0f;
            delayFrames = 0;
        }

        currentX = 0;
        currentY = 0;
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

        if (Input.GetKey(controls.keyboardControls.Punch) ||
            Input.GetKey(controls.keyboardControls.Kick) ||
            Input.GetKey(controls.keyboardControls.Throw) ||
            currentX != 0 && currentY != 0)
        {
            delayFrames = Mathf.RoundToInt((Time.deltaTime / msSendTime) + 0.5f);
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
