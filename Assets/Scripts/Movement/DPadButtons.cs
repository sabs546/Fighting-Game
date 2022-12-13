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

    public int delayFrames;
    public bool forceDelay;

    private bool useController2;
    private SetControls controls;

    // Start is called before the first frame update
    void Start()
    {
        lastX = lastY = 0.0f;
        controls = GetComponent<SetControls>();
        forceDelay = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (WorldRules.online)
        {
            if (delayFrames > 0)
            {
                // Delay moment
                delayFrames--;
                return;
            }
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

        if (WorldRules.online && (
            Input.GetKey(controls.keyboardControls.Punch) ||
            Input.GetKey(controls.keyboardControls.Kick)  ||
            Input.GetKey(controls.keyboardControls.Throw) ||
            currentX != 0 || currentY != 0 || forceDelay))
        {
            //delayFrames = Mathf.RoundToInt((PhotonNetwork.GetPing() / (1000.0f / WorldRules.physicsRate)) + 0.5f);
            delayFrames = Mathf.CeilToInt((PhotonNetwork.GetPing() + 0) / (2 * (1000.0f / WorldRules.physicsRate)));
            forceDelay = false;
        }
    }

    public void SwapControllerNumber(bool setting)
    {
        useController2 = setting;
        SetControls controls = GetComponent<SetControls>();

        int offset = useController2 ? 20 : -20;
        controls.gamepadControls.Punch += offset;
        controls.gamepadControls.Kick  += offset;
        controls.gamepadControls.Throw += offset;
        controls.gamepadControls.Pause += offset;
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
