using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float sprint;
    public float dashDistance;
    public float jumpPower;

    [Header("Inputs")]
    private bool up;
    private bool down;
    private bool left;
    private bool right;
    private bool rUp;
    private bool rDown;
    private bool rLeft;
    private bool rRight;
    private DPadButtons dpadInputs;

    public enum PlayerStates { Crouching, Grounded, Airborne };
    public enum GroundStates { Neutral, Dash, Backdash, Sprint, Stun };
    public enum AirStates    { Rising, Falling };
    [Header("States")]
    public PlayerStates pState;
    public GroundStates gState;
    public AirStates    aState;

    public enum Side { Left, Right };
    public Side currentSide;

    [Header("Components")]
    private SetControls controls;
    private PlayerPhysics physics;
    private PlayerAttackController attackController;

    // Start is called before the first frame update
    void OnEnable()
    {
        controls = GetComponent<SetControls>();
        physics = GetComponent<PlayerPhysics>();
        attackController = GetComponent<PlayerAttackController>();
        dpadInputs = GetComponent<DPadButtons>();
    }

    // Update is called once per frame
    void Update()
    {
        up = down = left = right = false;
        rUp = rDown = rLeft = rRight = false;

        if (controls.type == SetControls.ControllerType.Keyboard)
        {
            up = Input.GetKeyDown(controls.keyboardControls.Up) != up;
            down = Input.GetKeyDown(controls.keyboardControls.Down) != down;
            left = Input.GetKeyDown(controls.keyboardControls.Left) != left;
            right = Input.GetKeyDown(controls.keyboardControls.Right) != right;

            rUp = Input.GetKeyUp(controls.keyboardControls.Up) != rUp;
            rDown = Input.GetKeyUp(controls.keyboardControls.Down) != rDown;
            rLeft = Input.GetKeyUp(controls.keyboardControls.Left) != rLeft;
            rRight = Input.GetKeyUp(controls.keyboardControls.Right) != rRight;
        }
        else if (controls.type == SetControls.ControllerType.Controller)
        {
            switch (dpadInputs.DPadDown())
            {
                case DPadButtons.Inputs.Up:
                    up = true;
                    break;
                case DPadButtons.Inputs.Down:
                    down = true;
                    break;
                case DPadButtons.Inputs.Left:
                    left = true;
                    break;
                case DPadButtons.Inputs.Right:
                    right = true;
                    break;
                case DPadButtons.Inputs.None:
                    switch (dpadInputs.DPadUp())
                    {
                        case DPadButtons.Inputs.Up:
                            rUp = true;
                            break;
                        case DPadButtons.Inputs.Down:
                            rDown = true;
                            break;
                        case DPadButtons.Inputs.Left:
                            rLeft = true;
                            break;
                        case DPadButtons.Inputs.Right:
                            rRight = true;
                            break;
                    }
                    break;
            }

            dpadInputs.ClearInputs();
        }

        // --------------------------------------------------------
        // - Movement Inputs -
        // -------
        // You shouldn't be able to move while attacking
        if (attackController.state != PlayerAttackController.AttackState.Empty || gState == GroundStates.Stun)
        {
            if (attackController.state == PlayerAttackController.AttackState.Startup)
            {
                
            }
            else if (attackController.state == PlayerAttackController.AttackState.Active)
            {
                
            }
            else if (attackController.state == PlayerAttackController.AttackState.Recovery)
            {

            }

            // Sprint attacks need to still cancel
            if ((left || right) && gState != GroundStates.Neutral)
            {
                physics.startSprint = false;
                physics.travel = 0.0f;
            }
        }
        else if (pState == PlayerStates.Grounded && gState != GroundStates.Stun)
        {
            if (up)
            {
                physics.launch += jumpPower;
            }

            if (down)
            {
                if (gState == GroundStates.Dash)
                {
                    physics.enableCrouch = true;
                }
            }

            if (left && gState == GroundStates.Neutral)
            {
                physics.travel -= dashDistance;
            }
            else if (left && gState != GroundStates.Neutral)
            {
                physics.startSprint = true;
                physics.travel = -sprint;
            }
            else if (rLeft && gState != GroundStates.Neutral)
            {
                physics.startSprint = false;
                physics.travel = 0.0f;
            }

            if (right && gState == GroundStates.Neutral)
            {
                physics.travel += dashDistance;
            }
            else if (right && gState != GroundStates.Neutral)
            {
                physics.startSprint = true;
                physics.travel = sprint;
            }
            else if (rRight && gState != GroundStates.Neutral)
            {
                physics.startSprint = false;
                physics.travel = 0.0f;
            }
        }
        else if (pState == PlayerStates.Airborne)
        {
            if (gState == GroundStates.Sprint)
            {
                if (left && physics.airLock == -1)
                {
                    physics.startSprint = true;
                    physics.travel = -sprint;
                }
                else if (right && physics.airLock == 1)
                {
                    physics.startSprint = true;
                    physics.travel = sprint;
                }

                if (rLeft || rRight)
                {
                    physics.startSprint = false;
                    physics.travel = 0.0f;
                }
            }
        }

        if (controls.type == SetControls.ControllerType.Keyboard)
        {
            if (Input.GetKeyUp(controls.keyboardControls.Punch))
            {
                attackController.sendPunch = true;
            }
            else if (Input.GetKeyUp(controls.keyboardControls.Kick))
            {
                attackController.sendKick = true;
            }
            else if (Input.GetKeyUp(controls.keyboardControls.Throw))
            {
                attackController.sendThrow = true;
            }
        }
        else
        {
            if (Input.GetKeyUp(controls.gamepadControls.Punch))
            {
                attackController.sendPunch = true;
            }
            else if (Input.GetKeyUp(controls.gamepadControls.Kick))
            {
                attackController.sendKick = true;
            }
            else if (Input.GetKeyUp(controls.gamepadControls.Throw))
            {
                attackController.sendThrow = true;
            }
        }
    }
}
