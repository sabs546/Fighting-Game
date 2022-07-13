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
    [SerializeField]
    private SetControls controlsP2;
    private KeyCode triggerP1;
    private KeyCode triggerP2;
    private int checkAxisY1;
    private int checkAxisY2;

    // Start is called before the first frame update
    void OnEnable()
    {
        checkAxisY1 = 0;
        checkAxisY2 = 0;
        switch (inputKey)
        {
            case InputKey.Pause:
                triggerP1 = controlsP1.type == SetControls.ControllerType.Keyboard ? controlsP1.keyboardControls.Pause : triggerP1 = controlsP1.gamepadControls.Pause;
                triggerP2 = controlsP2.type == SetControls.ControllerType.Keyboard ? controlsP2.keyboardControls.Pause : triggerP2 = controlsP2.gamepadControls.Pause;
                break;
            case InputKey.Down:
                if (controlsP1.type == SetControls.ControllerType.Keyboard) triggerP1 = controlsP1.keyboardControls.Down;
                if (controlsP2.type == SetControls.ControllerType.Keyboard) triggerP2 = controlsP2.keyboardControls.Down;
                checkAxisY1 = -1;
                checkAxisY2 = -1;
                break;
            case InputKey.Up:
                if (controlsP1.type == SetControls.ControllerType.Keyboard) triggerP1 = controlsP1.keyboardControls.Up;
                if (controlsP2.type == SetControls.ControllerType.Keyboard) triggerP2 = controlsP2.keyboardControls.Up;
                checkAxisY1 = 1;
                checkAxisY2 = 1;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldRules.PvP)
        {
            if (Input.GetKey(triggerP1) || Input.GetKey(triggerP2))
            {
                button.onClick.Invoke();
            }
            if ((checkAxisY1 != 0 && Input.GetAxisRaw("DPadY") == checkAxisY1) || (checkAxisY2 != 0 && Input.GetAxisRaw("DPadY2") == checkAxisY2))
            {
                button.onClick.Invoke();
            }
        }
        else
        {
            if (Input.GetKey(triggerP1) || (checkAxisY1 != 0 && Input.GetAxisRaw("DPadY") == checkAxisY1))
            {
                button.onClick.Invoke();
            }
        }
    }
}
