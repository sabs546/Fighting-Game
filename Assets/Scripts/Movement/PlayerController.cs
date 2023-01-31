using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    // Movement Values ==================================
    [Header("Movement")]
    public float sprint;                  // Sprint speed
    public float dashDistance;            // Dash speed
    public float jumpPower;               // Jump height
    private string delayedInput;
    public UnityEngine.UI.Text Delay;
    public UnityEngine.UI.Text Correction;
    private bool forceCorrection;

    // Input Values =======================================================
    private enum Inputs { Up, Down, Left, Right, None };
    private bool up;
    private bool down;
    private bool left;
    private bool right;
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
    public PhotonView view { get; private set; }
    private PlayerController opponentController; // Mainly just for online use

    // Prevents knockback auto-block
    public bool blocking { get; private set; }
    private int ticker;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!WorldRules.online)
        {
            view = null;
            controls = GetComponent<SetControls>();
            dpadInputs = GetComponent<DPadButtons>();
            keyboardInputs = GetComponent<KeyboardInput>();
        }
        else if (controls == null)
        { // todo controls seems very awkward here, it's separate from the controls used in keyboard and pad controls
            controls = GetComponent<SetControls>();
            dpadInputs = GetComponent<DPadButtons>();
            keyboardInputs = GetComponent<KeyboardInput>();
            delayedInput = string.Empty;
            ticker = 0;
            if (view != null) view.RPC("RPC_SetOpponentController", PhotonNetwork.PlayerListOthers[0]);
            forceCorrection = false;
        }
        physics = GetComponent<PlayerPhysics>();
        attackController = GetComponent<PlayerAttackController>();
    }

    private void OnDisable()
    {
        gState = GroundStates.Neutral;
        pState = PlayerStates.Crouching;
        physics.startSprint = false;
    }

    private void FixedUpdate()
    {
        //int delayFrames = controls.type == SetControls.ControllerType.Keyboard ? keyboardInputs.delayFrames : dpadInputs.delayFrames;
        //if (WorldRules.online && delayedInput != string.Empty && delayFrames >= 0)
        //{
        //    if (delayFrames == 0)
        //    {
        //        switch (delayedInput)
        //        {
        //            case "RPC_Jump":
        //                physics.launch += jumpPower;
        //                delayedInput = string.Empty;
        //                break;
        //            case "RPC_Crouch":
        //                physics.enableCrouch = true;
        //                delayedInput = string.Empty;
        //                break;
        //            case "RPC_DashLeft":
        //                physics.travel -= dashDistance;
        //                blocking = true;
        //                delayedInput = string.Empty;
        //                break;
        //            case "RPC_DashRight":
        //                physics.travel += dashDistance;
        //                blocking = true;
        //                delayedInput = string.Empty;
        //                break;
        //            case "RPC_SprintLeft":
        //                physics.startSprint = true;
        //                physics.travel = -sprint;
        //                delayedInput = string.Empty;
        //                break;
        //            case "RPC_SprintRight":
        //                physics.startSprint = true;
        //                physics.travel = sprint;
        //                delayedInput = string.Empty;
        //                break;
        //            case "RPC_CancelSprint":
        //                physics.startSprint = false;
        //                physics.travel = 0.0f;
        //                delayedInput = string.Empty;
        //                break;

        //            case "RPC_SendPunch":
        //                attackController.sendPunch = true;
        //                delayedInput = string.Empty;
        //                break;
        //            case "RPC_SendKick":
        //                attackController.sendKick = true;
        //                delayedInput = string.Empty;
        //                break;
        //            case "RPC_SendThrow":
        //                attackController.sendThrow = true;
        //                delayedInput = string.Empty;
        //                break;
        //        }
        //    }
        //}
        if (WorldRules.online && view != null && forceCorrection)
        {
            view.RPC("RPC_SmoothSyncPos", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldRules.online && PhotonNetwork.PlayerListOthers.Length == 0)
        {
            Camera.main.GetComponent<GameStateControl>().IncorrectEndGame();
            return;
        }

        if (!WorldRules.online || (view != null && view.IsMine))
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
        }

        if (WorldRules.online && delayedInput != string.Empty && (keyboardInputs.delayFrames > 0 || dpadInputs.delayFrames > 0))
        {
            return;
        }

        up = down = left = right = false;
        rLeft = rRight = false;

        if ((!WorldRules.online || (view != null && view.IsMine)) && attackController.state == PlayerAttackController.AttackState.Empty && gState != GroundStates.Stun)
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
                if (view != null)
                {
                    view.RPC("RPC_CancelSprint", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                }

                physics.startSprint = false;
                physics.travel = 0.0f;
            }
        }
        else if (pState == PlayerStates.Grounded && gState != GroundStates.Stun)
        {
            if (up)
            {
                if (view != null)
                    view.RPC("RPC_Jump", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                physics.launch += jumpPower;
            }

            if (down)
            {
                if (gState == GroundStates.Dash)
                {
                    if (view != null)
                        view.RPC("RPC_Crouch", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                    physics.enableCrouch = true;
                }
            }

            if (left && gState == GroundStates.Neutral)
            {
                if (view != null)
                    view.RPC("RPC_DashLeft", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                physics.travel -= dashDistance;
                blocking = true;
            }
            else if (left && gState != GroundStates.Neutral)
            {
                if (view != null)
                {
                    view.RPC("RPC_SprintLeft", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                }

                physics.startSprint = true;
                physics.travel = -sprint;
            }
            else if (rLeft && gState == GroundStates.Sprint)
            {
                if (view != null)
                {
                    view.RPC("RPC_CancelSprint", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                }

                physics.startSprint = false;
                physics.travel = 0.0f;
            }

            if (right && gState == GroundStates.Neutral)
            {
                if (view != null)
                    view.RPC("RPC_DashRight", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                physics.travel += dashDistance;
                blocking = true;
            }
            else if (right && gState != GroundStates.Neutral)
            {
                if (view != null)
                {
                    view.RPC("RPC_SprintRight", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                }

                physics.startSprint = true;
                physics.travel = sprint;
            }
            else if (rRight && gState == GroundStates.Sprint)
            {
                if (view != null)
                {
                    view.RPC("RPC_CancelSprint", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                }

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
                    if (view != null && view.IsMine)
                    {
                        view.RPC("RPC_SprintLeft", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                    }

                    physics.startSprint = true;
                    physics.travel = -sprint;
                }
                else if (right && physics.airLock == 1)
                {
                    if (view != null)
                    {
                        view.RPC("RPC_SprintRight", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                    }

                    physics.startSprint = true;
                    physics.travel = sprint;
                }

                if (rLeft || rRight)
                {
                    if (view != null)
                    {
                        view.RPC("RPC_CancelSprint", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position);
                    }

                    physics.startSprint = false;
                    physics.travel = 0.0f;
                }
            }
        }

        if ((attackController.state == PlayerAttackController.AttackState.Empty || attackController.state == PlayerAttackController.AttackState.Recovery) && gState != GroundStates.Stun)
        {
            if (controls.type == SetControls.ControllerType.Keyboard)
            {
                if (Input.GetKeyDown(controls.keyboardControls.Punch))
                {
                    if (view != null && view.IsMine)
                        view.RPC("RPC_SendPunch", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position, physics.GetMovementState(true), physics.GetMovementState(false));
                    attackController.sendPunch = true;
                }
                else if (Input.GetKeyDown(controls.keyboardControls.Kick))
                {
                    if (view != null && view.IsMine)
                        view.RPC("RPC_SendKick", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position, physics.GetMovementState(true), physics.GetMovementState(false));
                    attackController.sendKick = true;
                }
                else if (Input.GetKeyDown(controls.keyboardControls.Throw))
                {
                    if (view != null && view.IsMine)
                        view.RPC("RPC_SendThrow", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position, physics.GetMovementState(true), physics.GetMovementState(false));
                    attackController.sendThrow = true;
                }
            }
            else
            {
                if (Input.GetKeyDown(controls.gamepadControls.Punch))
                {
                    if (view != null && view.IsMine)
                        view.RPC("RPC_SendPunch", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position, physics.GetMovementState(true), physics.GetMovementState(false));
                    attackController.sendPunch = true;
                }
                else if (Input.GetKeyDown(controls.gamepadControls.Kick))
                {
                    if (view != null && view.IsMine)
                        view.RPC("RPC_SendKick", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position, physics.GetMovementState(true), physics.GetMovementState(false));
                    attackController.sendKick = true;
                }
                else if (Input.GetKeyDown(controls.gamepadControls.Throw))
                {
                    if (view != null && view.IsMine)
                        view.RPC("RPC_SendThrow", PhotonNetwork.PlayerListOthers[0], PhotonNetwork.ServerTimestamp, transform.position, physics.GetMovementState(true), physics.GetMovementState(false));
                    attackController.sendThrow = true;
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

        physics = GetComponent<PlayerPhysics>();
        controls = physics.opponent.GetComponent<SetControls>();
        dpadInputs = physics.opponent.GetComponent<DPadButtons>();
        keyboardInputs = physics.opponent.GetComponent<KeyboardInput>();
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
    private void RPC_SetOpponentController()
    {
        opponentController = physics.opponent.GetComponent<PlayerController>();
    }
    [PunRPC]
    private void RPC_Jump(int sendTime, Vector3 syncPos)
    {
        physics.launch += jumpPower;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        physics.fastSimulate = trueDelayFrames;
        transform.position = syncPos;
    }
    [PunRPC]
    private void RPC_DashLeft(int sendTime, Vector3 syncPos)
    {
        physics.travel -= dashDistance;
        blocking = true;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        physics.fastSimulate = trueDelayFrames;
        Correction.text = physics.fastSimulate + " (" + trueDelayFrames + ")" + trueDelayMs;
        transform.position = syncPos;
    }
    [PunRPC]
    private void RPC_DashRight(int sendTime, Vector3 syncPos)
    {
        physics.travel += dashDistance;
        blocking = true;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        physics.fastSimulate = trueDelayFrames;
        Correction.text = physics.fastSimulate + " (" + trueDelayFrames + ")" + trueDelayMs;
        transform.position = syncPos;
    }
    [PunRPC]
    private void RPC_Crouch(int sendTime, Vector3 syncPos)
    {
        physics.enableCrouch = true;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        physics.fastSimulate = trueDelayFrames;
        transform.position = syncPos;
    }
    [PunRPC]
    private void RPC_SprintLeft(int sendTime, Vector3 syncPos)
    {
        physics.startSprint = true;
        physics.travel = -sprint;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        physics.fastSimulate = trueDelayFrames;
        transform.position = syncPos;
    }
    [PunRPC]
    private void RPC_SprintRight(int sendTime, Vector3 syncPos)
    {
        physics.startSprint = true;
        physics.travel = sprint;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        physics.fastSimulate = trueDelayFrames;
        transform.position = syncPos;
    }
    [PunRPC]
    private void RPC_CancelSprint(int sendTime, Vector3 syncPos)
    {
        physics.startSprint = false;
        physics.travel = 0.0f;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        physics.fastSimulate = trueDelayFrames;
        transform.position = syncPos;
    }
    [PunRPC]
    private void RPC_SendPunch(int sendTime, Vector3 syncPos, float gravity, float movement)
    {
        attackController.sendPunch = true;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        attackController.fastSimulate = trueDelayFrames - 1;
        if (gState == GroundStates.Sprint)
        {
            physics.fastSimulate = trueDelayFrames;
            transform.position = syncPos;
        }

        physics.ForceMovementState(gravity, movement);
    }
    [PunRPC]
    private void RPC_SendKick(int sendTime, Vector3 syncPos, float gravity, float movement)
    {
        attackController.sendKick = true;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        attackController.fastSimulate = trueDelayFrames - 1;
        if (gState == GroundStates.Sprint)
        {
            physics.fastSimulate = trueDelayFrames;
            transform.position = syncPos;
        }

        physics.ForceMovementState(gravity, movement);
    }
    [PunRPC]
    private void RPC_SendThrow(int sendTime, Vector3 syncPos, float gravity, float movement)
    {
        attackController.sendThrow = true;
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        attackController.fastSimulate = trueDelayFrames - 1;
        if (gState == GroundStates.Sprint)
        {
            physics.fastSimulate = trueDelayFrames;
            transform.position = syncPos;
        }

        physics.ForceMovementState(gravity, movement);
    }
    [PunRPC]
    private void RPC_SyncPos(Vector3 syncPos)
    {
        transform.position = syncPos;
    }
    [PunRPC]
    private void RPC_SmoothSyncPos(int sendTime, Vector3 syncPos)
    {
        int trueDelayMs = PhotonNetwork.ServerTimestamp - sendTime;
        int trueDelayFrames = Mathf.CeilToInt(trueDelayMs / (2 * (1000.0f / WorldRules.physicsRate)));
        physics.fastSimulate = trueDelayFrames;

        float lagDistance = (transform.position - syncPos).magnitude;
        if (lagDistance < 0.1f)
        {
            transform.position = syncPos;
            return;
        }

        float xPos = transform.position.x;
        float yPos = transform.position.y;
        if (transform.position.x != syncPos.x)
        {
            xPos -= (transform.position.x - syncPos.x) / 10;
            if (xPos < 1.0f && xPos > -1.0f)
            {
                xPos = transform.position.x;
            }
        }
        if (transform.position.y != syncPos.y)
        {
            yPos -= (transform.position.y - syncPos.y) / 10;
            if (yPos < 1.0f && yPos > -1.0f)
            {
                yPos = transform.position.y;
            }
        }
        transform.position = new Vector3(xPos, yPos, transform.position.z);
        //if (opponentController != null) opponentController.view.RPC("RPC_SyncPos", PhotonNetwork.PlayerListOthers[0], opponentController.transform.position);
    }

    public void StartGame()
    {
        int delayFrames = Mathf.CeilToInt((PhotonNetwork.GetPing() + 0) / (2 * (1000.0f / WorldRules.physicsRate)));
        //Camera.main.GetComponent<CameraControl>().StartGame(Mathf.RoundToInt((PhotonNetwork.GetPing() / (1000.0f / WorldRules.physicsRate)) + 0.5f));
        Camera.main.GetComponent<CameraControl>().StartGame(Mathf.CeilToInt((PhotonNetwork.GetPing()) / (2 * (1000.0f / WorldRules.physicsRate))));
        view.RPC("RPC_StartGame", PhotonNetwork.PlayerListOthers[0]);
    }
    [PunRPC]
    private void RPC_StartGame()
    {
        Camera.main.GetComponent<CameraControl>().StartGame();
    }
    [PunRPC]
    private void RPC_EndGame()
    {
        Camera.main.GetComponent<GameStateControl>().EndGame();
    }
}
