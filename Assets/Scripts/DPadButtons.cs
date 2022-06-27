using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPadButtons : MonoBehaviour
{
    public enum Inputs { None, Up, Down, Left, Right };
    public Inputs currentInput;

    private float lastX;
    private float lastY;
    private float currentX;
    private float currentY;

    // Start is called before the first frame update
    void Start()
    {
        lastX = lastY = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Inputs DPadDown()
    {
        currentX = Input.GetAxisRaw("DPadX");
        currentY = Input.GetAxisRaw("DPadY");

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
        currentX = Input.GetAxisRaw("DPadX");
        currentY = Input.GetAxisRaw("DPadY");

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
