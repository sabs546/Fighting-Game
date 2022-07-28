using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerButton : MonoBehaviour
{
    private enum InputKey { Up, Down, Left, Right, Punch, Kick, Throw, Pause };
    [SerializeField]
    private InputKey inputKey;
    [SerializeField]
    private Button button;
    [SerializeField]
    private SetControls controlsP1;
    private SetControls.ControllerType currentType1;
    [SerializeField]
    private SetControls controlsP2;
    private SetControls.ControllerType currentType2;
    private KeyCode triggerP1;
    private KeyCode triggerP2;
    private int checkAxisY1;
    private int checkAxisY2;

    // Start is called before the first frame update
    void OnEnable()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        if (!button.interactable)
        {
            return;
        }

        if (controlsP1.type != currentType1 || controlsP2.type != currentType2)
        {
            Setup();
        }

        if (Input.GetKeyDown(triggerP1))
        {
            button.onClick.Invoke();
        }
        else if ((checkAxisY1 != 0 && Input.GetAxisRaw("DPadY") == checkAxisY1) || Input.GetKeyDown(triggerP1))
        {
            button.onClick.Invoke();
        }

        if (WorldRules.PvP)
        {
            if (Input.GetKeyDown(triggerP2))
            {
                button.onClick.Invoke();
            }
            else if ((checkAxisY2 != 0 && Input.GetAxisRaw("DPadY2") == checkAxisY2) || Input.GetKeyDown(triggerP2))
            {
                button.onClick.Invoke();
            }
        }
        currentType1 = controlsP1.type;
        currentType2 = controlsP2.type;
    }

    private void Setup()
    {
        checkAxisY1 = 0;
        checkAxisY2 = 0;
        switch (inputKey)
        {
            case InputKey.Pause:
                triggerP1 = controlsP1.type == SetControls.ControllerType.Keyboard ? controlsP1.keyboardControls.Pause : controlsP1.gamepadControls.Pause;
                triggerP2 = controlsP2.type == SetControls.ControllerType.Keyboard ? controlsP2.keyboardControls.Pause : controlsP2.gamepadControls.Pause;
                break;
            case InputKey.Down:
                triggerP1 = controlsP1.keyboardControls.Down;
                triggerP2 = controlsP2.keyboardControls.Down;
                checkAxisY1 = -1;
                checkAxisY2 = -1;
                break;
            case InputKey.Up:
                triggerP1 = controlsP1.keyboardControls.Up;
                triggerP2 = controlsP2.keyboardControls.Up;
                checkAxisY1 = 1;
                checkAxisY2 = 1;
                break;
            case InputKey.Left:
                triggerP1 = controlsP1.type == SetControls.ControllerType.Keyboard ? controlsP1.keyboardControls.Left : KeyCode.JoystickButton4;
                triggerP2 = controlsP2.type == SetControls.ControllerType.Keyboard ? controlsP2.keyboardControls.Left : KeyCode.JoystickButton4;
                break;
            case InputKey.Right:
                triggerP1 = controlsP1.type == SetControls.ControllerType.Keyboard ? controlsP1.keyboardControls.Right : KeyCode.JoystickButton5;
                triggerP2 = controlsP2.type == SetControls.ControllerType.Keyboard ? controlsP2.keyboardControls.Right : KeyCode.JoystickButton5;
                break;
        }
    }
}
