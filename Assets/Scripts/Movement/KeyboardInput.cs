using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    public enum Inputs { None, Up, Down, Left, Right };
    [HideInInspector]
    public Inputs currentInput;

    private int lastX;
    private int lastY;
    private int currentX;
    private int currentY;
    
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
        currentX = 0;
        if (Input.GetKey(controls.keyboardControls.Left))
        {
            currentX = -1;
        }
        else if (Input.GetKey(controls.keyboardControls.Right))
        {
            currentX = 1;
        }
        currentY = 0;
        if (Input.GetKey(controls.keyboardControls.Up))
        {
            currentY = 1;
        }
        else if (Input.GetKey(controls.keyboardControls.Down))
        {
            currentY = -1;
        }
    }

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
