using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class InputSwap : MonoBehaviour
{
    [SerializeField]
    private string originalString;
    [SerializeField]
    private SetControls controls;
    private TextMeshProUGUI currentText;
    private Button button;
    private bool keyboardSelected;
    private bool gamepadSelected;

    // Start is called before the first frame update
    void Start()
    {
        currentText = GetComponent<TextMeshProUGUI>();
        button = GetComponent<Button>();
        keyboardSelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboardSelected)
        {
            if (Input.inputString != string.Empty && Input.inputString != Input.inputString.ToUpper())
            {
                string input = Input.inputString.ToUpper();
                currentText.text = originalString + input;
                KeyCode thisKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), input);
                switch (originalString)
                {
                    case "Punch - ":
                        controls.keyboardControls.Punch = thisKeyCode;
                        break;
                    case "Kick - ":
                        controls.keyboardControls.Kick = thisKeyCode;
                        break;
                    case "Throw - ":
                        controls.keyboardControls.Throw = thisKeyCode;
                        break;
                }
                keyboardSelected = false;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        else if (gamepadSelected)
        {
            if (Input.anyKey == true && Input.inputString == string.Empty)
            {
                //foreach (KeyCode kCode in System.Enum.GetValues(typeof(KeyCode)))
                for (int key = (int)KeyCode.Joystick1Button0; key < (int)KeyCode.Joystick2Button19; key++)
                {
                    KeyCode kCode = (KeyCode)key;
                    if (!Input.GetKey(kCode)) continue;

                    string code = ConvertToController(kCode);
                    if (code != string.Empty)
                    {
                        currentText.text = originalString + ConvertToController(kCode);
                        KeyCode thisKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), kCode.ToString());
                        switch (originalString)
                        {
                            case "Punch - ":
                                controls.gamepadControls.Punch = thisKeyCode;
                                break;
                            case "Kick - ":
                                controls.gamepadControls.Kick = thisKeyCode;
                                break;
                            case "Throw - ":
                                controls.gamepadControls.Throw = thisKeyCode;
                                break;
                        }
                        gamepadSelected = false;
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
            }
        }
    }

    public void Mode(int type)
    {
        if (type == (int)SetControls.ControllerType.Keyboard)
        {
            keyboardSelected = true;
        }
        if (type == (int)SetControls.ControllerType.Controller)
        {
            gamepadSelected = true;
        }
    }

    private string ConvertToController(KeyCode kCode)
    {
        switch (kCode)
        {
            case KeyCode.Joystick1Button0:
                return "A";
            case KeyCode.Joystick1Button1:
                return "B";
            case KeyCode.Joystick1Button2:
                return "X";
            case KeyCode.Joystick1Button3:
                return "Y";
            case KeyCode.Joystick1Button4:
                return "LB";
            case KeyCode.Joystick1Button5:
                return "RB";

            case KeyCode.Joystick2Button0:
                return "A";
            case KeyCode.Joystick2Button1:
                return "B";
            case KeyCode.Joystick2Button2:
                return "X";
            case KeyCode.Joystick2Button3:
                return "Y";
            case KeyCode.Joystick2Button4:
                return "LB";
            case KeyCode.Joystick2Button5:
                return "RB";
        }
        return string.Empty;
    }
}
