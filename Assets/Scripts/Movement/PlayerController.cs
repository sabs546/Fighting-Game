using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement Values =======================
    [Header("Movement")]
    public float sprint;       // Sprint speed
    public float dashDistance; // Dash speed
    public float jumpPower;    // Jump height

    // Input Values =======================================================
    private enum Inputs { Up, Down, Left, Right, None };
    private Inputs currentInput;          // The down input of a button
    private Inputs currentRInput;         // The up input of a button
    private bool up;
    private bool down;
    private bool left;
    private bool right;
    private bool rUp;
    private bool rDown;
    private bool rLeft;
    private bool rRight;
    private DPadButtons dpadInputs;       // Handles the directional inputs
    private KeyboardInput keyboardInputs; // Also did it to keyboard

    // States =========================================================
    public enum PlayerStates { Crouching, Grounded, Airborne };
    public enum GroundStates { Neutral, Dash, Backdash, Sprint, Stun };
    public enum AirStates    { Rising, Falling };
    [Header("States")]
    public PlayerStates pState;
    public GroundStates gState;
    public AirStates    aState;

    public enum Side { Left, Right };
    public Side currentSide;

    // Components ==================================
    [Header("Components")]
    private SetControls controls;
    private PlayerPhysics physics;
    private PlayerAttackController attackController;

    // Prevents knockback auto-block
    public bool blocking { get; private set; }

    // Start is called before the first frame update
    void OnEnable()
    {
        controls = GetComponent<SetControls>();
        physics = GetComponent<PlayerPhysics>();
        attackController = GetComponent<PlayerAttackController>();
        dpadInputs = GetComponent<DPadButtons>();
        keyboardInputs = GetComponent<KeyboardInput>();
    }

    private void OnDisable()
    {
        gState = GroundStates.Neutral;
        pState = PlayerStates.Crouching;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateControl.gameState == GameStateControl.GameState.Pause)
        {
            if (controls.type == SetControls.ControllerType.Keyboard && Input.GetKeyDown(controls.keyboardControls.Pause) ||
                controls.type == SetControls.ControllerType.Controller && Input.GetKeyDown(controls.gamepadControls.Pause))
            {
                Camera.main.GetComponent<GameStateControl>().SetGameState(GameStateControl.GameState.Fighting);
            }
            return;
        }
        else
        {
            if (controls.type == SetControls.ControllerType.Keyboard && Input.GetKeyDown(controls.keyboardControls.Pause) ||
                controls.type == SetControls.ControllerType.Controller && Input.GetKeyDown(controls.gamepadControls.Pause))
            {
                Camera.main.GetComponent<GameStateControl>().SetGameState(GameStateControl.GameState.Pause);
            }
        }

        up = down = left = right = false;
        rUp = rDown = rLeft = rRight = false;

        if (attackController.state == PlayerAttackController.AttackState.Empty && gState != GroundStates.Stun)
        {
            if (controls.type == SetControls.ControllerType.Keyboard)
            { // todo something is wrong with sprint attacks and keyboard inputs
                switch (keyboardInputs.KeyboardDown())
                {
                    case KeyboardInput.Inputs.Up:
                        up = true;
                        break;
                    case KeyboardInput.Inputs.Down:
                        down = true;
                        break;
                    case KeyboardInput.Inputs.Left:
                        left = true;
                        break;
                    case KeyboardInput.Inputs.Right:
                        right = true;
                        break;
                    case KeyboardInput.Inputs.None:
                        switch (keyboardInputs.KeyboardUp())
                        {
                            case KeyboardInput.Inputs.Up:
                                rUp = true;
                                break;
                            case KeyboardInput.Inputs.Down:
                                rDown = true;
                                break;
                            case KeyboardInput.Inputs.Left:
                                rLeft = true;
                                break;
                            case KeyboardInput.Inputs.Right:
                                rRight = true;
                                break;
                        }
                        break;
                }

                keyboardInputs.ClearInputs();
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
                blocking = true;
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
                blocking = true;
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

        if ((attackController.state == PlayerAttackController.AttackState.Empty || attackController.state == PlayerAttackController.AttackState.Recovery) && gState != GroundStates.Stun)
        {
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

    public void EnableBlock()
    {
        blocking = true;
    }
}
