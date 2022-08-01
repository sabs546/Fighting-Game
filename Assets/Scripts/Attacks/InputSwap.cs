using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class InputSwap : MonoBehaviour, IDeselectHandler
{
    [SerializeField]
    private string originalString;
    [SerializeField]
    private SetControls controls;
    [SerializeField]
    private SetControls onlineControls;
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
            if (Input.inputString != string.Empty)
            {
                KeyCode code = CheckValid(Input.inputString);
                if (code != KeyCode.None)
                {
                    switch (originalString)
                    {
                        case "Punch - ":
                            controls.keyboardControls.Punch = code;
                            if (onlineControls != null) controls.keyboardControls.Punch = code;
                            break;
                        case "Kick - ":
                            controls.keyboardControls.Kick = code;
                            if (onlineControls != null) controls.keyboardControls.Kick = code;
                            break;
                        case "Throw - ":
                            controls.keyboardControls.Throw = code;
                            if (onlineControls != null) controls.keyboardControls.Throw = code;
                            break;
                    }
                    keyboardSelected = false;
                    EventSystem.current.SetSelectedGameObject(null);
                }
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
                                if (onlineControls != null) controls.gamepadControls.Punch = thisKeyCode;
                                break;
                            case "Kick - ":
                                controls.gamepadControls.Kick = thisKeyCode;
                                if (onlineControls != null) controls.gamepadControls.Kick = thisKeyCode;
                                break;
                            case "Throw - ":
                                controls.gamepadControls.Throw = thisKeyCode;
                                if (onlineControls != null) controls.gamepadControls.Throw = thisKeyCode;
                                break;
                        }
                        gamepadSelected = false;
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
            }
        }
    }

    public void OnDeselect(BaseEventData data)
    {
        keyboardSelected = gamepadSelected = false;
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

    private KeyCode CheckValid(string inputString)
    {
        // Letters
        if (inputString != string.Empty && inputString != inputString.ToUpper())
        {
            string input = inputString.ToUpper();
            currentText.text = originalString + input;
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), input);
        }

        // Numpad
        byte[] ascii = System.Text.Encoding.ASCII.GetBytes(inputString);
        KeyCode code = GetNumpadDown();
        if (code != KeyCode.None)
        {
            currentText.text = originalString + "Numpad " + inputString;
            return code;
        }

        // Alpha
        if (ascii.First() >= 48 && ascii.First() <= 57)
        {
            currentText.text = originalString + inputString;
            return (KeyCode)ascii.First();
        }
        return KeyCode.None;
    }

    private KeyCode GetNumpadDown()
    {
        if (Input.GetKey(KeyCode.Keypad1)) return KeyCode.Keypad1;
        if (Input.GetKey(KeyCode.Keypad2)) return KeyCode.Keypad2;
        if (Input.GetKey(KeyCode.Keypad3)) return KeyCode.Keypad3;
        if (Input.GetKey(KeyCode.Keypad4)) return KeyCode.Keypad4;
        if (Input.GetKey(KeyCode.Keypad5)) return KeyCode.Keypad5;
        if (Input.GetKey(KeyCode.Keypad6)) return KeyCode.Keypad6;
        if (Input.GetKey(KeyCode.Keypad7)) return KeyCode.Keypad7;
        if (Input.GetKey(KeyCode.Keypad8)) return KeyCode.Keypad8;
        if (Input.GetKey(KeyCode.Keypad9)) return KeyCode.Keypad9;
        return KeyCode.None;
    }
}
