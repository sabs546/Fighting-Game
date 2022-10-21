using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KeyboardInput : MonoBehaviour
{
    public enum Inputs { None, Up, Down, Left, Right };
    [HideInInspector]
    public Inputs currentInput;

    private int lastX;
    private int lastY;
    private int currentX;
    private int currentY;

    private float msSendTime;
    public int delayFrames;
    private int currentDelay; // The delay on the key you just hit

    private SetControls controls;

    // Start is called before the first frame update
    void Start()
    {
        lastX = lastY = 0;
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
            msSendTime = PhotonNetwork.GetPing() / 2;
            msSendTime /= 1000.0f;
            delayFrames = 0;
        }

        currentX = 0;
        if (Input.GetKey(controls.keyboardControls.Left))
        {
            currentX = -1;
            delayFrames = Mathf.RoundToInt((Time.deltaTime / msSendTime) + 0.5f);
        }
        else if (Input.GetKey(controls.keyboardControls.Right))
        {
            currentX = 1;
            delayFrames = Mathf.RoundToInt((Time.deltaTime / msSendTime) + 0.5f);
        }
        currentY = 0;
        if (Input.GetKey(controls.keyboardControls.Up))
        {
            currentY = 1;
            delayFrames = Mathf.RoundToInt((Time.deltaTime / msSendTime) + 0.5f);
        }
        else if (Input.GetKey(controls.keyboardControls.Down))
        {
            currentY = -1;
            delayFrames = Mathf.RoundToInt((Time.deltaTime / msSendTime) + 0.5f);
        }
    }

    // This seems convoluted, I mean why not just go with Input.GetKey?
    // Well it turns out this keeps it consistent with pad controls that use axis inputs rather than the normal ones
    // So the inputs are read the way Unity would, but processed manually so it definitely goes through the same way
    public Inputs KeyboardDown()
    {
        if (currentY == 1 && lastY != 1)
        {
            return Inputs.Up;
        }
        else if (currentY == -1 && lastY != -1)
        {
            return Inputs.Down;
        }
        if (currentX == -1 && lastX != -1)
        {
            return Inputs.Left;
        }
        else if (currentX == 1 && lastX != 1)
        {
            return Inputs.Right;
        }
        return Inputs.None;
    }

    public Inputs KeyboardUp()
    {
        if (currentY != 1 && lastY == 1)
        {
            return Inputs.Up;
        }
        else if (currentY != -1 && lastY == -1)
        {
            return Inputs.Down;
        }
        if (currentX != -1 && lastX == -1)
        {
            return Inputs.Left;
        }
        else if (currentX != 1 && lastX == 1)
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
