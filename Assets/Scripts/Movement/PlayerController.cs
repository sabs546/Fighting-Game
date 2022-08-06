using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    // Movement Values ==================================
    [Header("Movement")]
    public float sprint;                  // Sprint speed
    public float dashDistance;            // Dash speed
    public float jumpPower;               // Jump height

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
    private PhotonView view;

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
        physics.startSprint = false;
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
            if (GameStateControl.gameState == GameStateControl.GameState.Fighting &&
                controls.type == SetControls.ControllerType.Keyboard && Input.GetKeyDown(controls.keyboardControls.Pause) ||
                controls.type == SetControls.ControllerType.Controller && Input.GetKeyDown(controls.gamepadControls.Pause))
            {
                Camera.main.GetComponent<GameStateControl>().SetGameState(GameStateControl.GameState.Pause);
            }
        }

        up = down = left = right = false;
        rUp = rDown = rLeft = rRight = false;

        if ((WorldRules.offline || (view != null && view.IsMine)) && attackController.state == PlayerAttackController.AttackState.Empty && gState != GroundStates.Stun)
        {
            if (controls.type == SetControls.ControllerType.Keyboard)
            {
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
                if (view != null) view.RPC("RPC_CancelSprint", PhotonNetwork.PlayerListOthers[0]);
            }
        }
        else if (pState == PlayerStates.Grounded && gState != GroundStates.Stun)
        {
            if (up)
            {
                physics.launch += jumpPower;
                if (view != null) view.RPC("RPC_Jump", PhotonNetwork.PlayerListOthers[0]);
            }

            if (down)
            {
                if (gState == GroundStates.Dash)
                {
                    physics.enableCrouch = true;
                    if (view != null) view.RPC("RPC_Crouch", PhotonNetwork.PlayerListOthers[0]);
                }
            }

            if (left && gState == GroundStates.Neutral)
            {
                physics.travel -= dashDistance;
                blocking = true;
                if (view != null)
                {
                    view.RPC("RPC_DashLeft", PhotonNetwork.PlayerListOthers[0]);
                    view.RPC("RPC_SendBlock", PhotonNetwork.PlayerListOthers[0]);
                }
            }
            else if (left && gState != GroundStates.Neutral)
            {
                physics.startSprint = true;
                physics.travel = -sprint;
                if (view != null) view.RPC("RPC_SprintLeft", PhotonNetwork.PlayerListOthers[0]);
            }
            else if (rLeft && gState != GroundStates.Neutral)
            {
                physics.startSprint = false;
                physics.travel = 0.0f;
                if (view != null) view.RPC("RPC_CancelSprint", PhotonNetwork.PlayerListOthers[0]);
            }

            if (right && gState == GroundStates.Neutral)
            {
                physics.travel += dashDistance;
                blocking = true;
                if (view != null)
                {
                    view.RPC("RPC_DashRight", PhotonNetwork.PlayerListOthers[0]);
                    view.RPC("RPC_SendBlock", PhotonNetwork.PlayerListOthers[0]);
                }
            }
            else if (right && gState != GroundStates.Neutral)
            {
                physics.startSprint = true;
                physics.travel = sprint;
                if (view != null) view.RPC("RPC_SprintRight", PhotonNetwork.PlayerListOthers[0]);
            }
            else if (rRight && gState != GroundStates.Neutral)
            {
                physics.startSprint = false;
                physics.travel = 0.0f;
                if (view != null) view.RPC("RPC_CancelSprint", PhotonNetwork.PlayerListOthers[0]);
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
                    if (view != null) view.RPC("RPC_SprintLeft", PhotonNetwork.PlayerListOthers[0]);
                }
                else if (right && physics.airLock == 1)
                {
                    physics.startSprint = true;
                    physics.travel = sprint;
                    if (view != null) view.RPC("RPC_SprintRight", PhotonNetwork.PlayerListOthers[0]);
                }

                if (rLeft || rRight)
                {
                    physics.startSprint = false;
                    physics.travel = 0.0f;
                    if (view != null) view.RPC("RPC_CancelSprint", PhotonNetwork.PlayerListOthers[0]);
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
                    if (view != null) view.RPC("RPC_SendPunch", PhotonNetwork.PlayerListOthers[0]);
                }
                else if (Input.GetKeyUp(controls.keyboardControls.Kick))
                {
                    attackController.sendKick = true;
                    if (view != null) view.RPC("RPC_SendKick", PhotonNetwork.PlayerListOthers[0]);
                }
                else if (Input.GetKeyUp(controls.keyboardControls.Throw))
                {
                    attackController.sendThrow = true;
                    if (view != null) view.RPC("RPC_SendThrow", PhotonNetwork.PlayerListOthers[0]);
                }
            }
            else
            {
                if (Input.GetKeyUp(controls.gamepadControls.Punch))
                {
                    attackController.sendPunch = true;
                    if (view != null) view.RPC("RPC_SendPunch", PhotonNetwork.PlayerListOthers[0]);
                }
                else if (Input.GetKeyUp(controls.gamepadControls.Kick))
                {
                    attackController.sendKick = true;
                    if (view != null) view.RPC("RPC_SendKick", PhotonNetwork.PlayerListOthers[0]);
                }
                else if (Input.GetKeyUp(controls.gamepadControls.Throw))
                {
                    attackController.sendThrow = true;
                    if (view != null) view.RPC("RPC_SendThrow", PhotonNetwork.PlayerListOthers[0]);
                }
            }
        }
    }

    public void DisableBlock()
    {
        blocking = false;
    }

    public void SwapOfflineInputs()
    {
        GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1].ActorNumber);
    }

    public void SetView()
    {
        view = GetComponent<PhotonView>();
    }

    public void NullifyView()
    {
        view = null;
    }

    // ====================================================================================
    // RPCFunctions
    // =======
    [PunRPC]
    private void RPC_Jump()
    {
        physics.launch += jumpPower;
    }
    [PunRPC]
    private void RPC_DashLeft()
    {
        physics.travel -= dashDistance;
    }
    [PunRPC]
    private void RPC_DashRight()
    {
        physics.travel += dashDistance;
    }
    [PunRPC]
    private void RPC_Crouch()
    {
        physics.enableCrouch = true;
    }
    [PunRPC]
    private void RPC_SprintLeft()
    {
        physics.startSprint = true;
        physics.travel = -sprint;
    }
    [PunRPC]
    private void RPC_SprintRight()
    {
        physics.startSprint = true;
        physics.travel = sprint;
    }
    [PunRPC]
    private void RPC_CancelSprint()
    {
        physics.startSprint = false;
        physics.travel = 0.0f;
    }
    [PunRPC]
    private void RPC_SendPunch()
    {
        attackController.sendPunch = true;
    }
    [PunRPC]
    private void RPC_SendKick()
    {
        attackController.sendKick = true;
    }
    [PunRPC]
    private void RPC_SendThrow()
    {
        attackController.sendThrow = true;
    }
    // todo maybe not necessary? might want cancel block, not sure
    [PunRPC]
    private void RPC_SendBlock()
    {
        blocking = true;
    }

    public void StartGame()
    {
        view.RPC("RPC_StartGame", PhotonNetwork.MasterClient);
        view.RPC("RPC_StartGame", PhotonNetwork.PlayerListOthers[0]);
    }
    [PunRPC]
    private void RPC_StartGame()
    {
        Camera.main.GetComponent<CameraControl>().StartGame();
    }
}
