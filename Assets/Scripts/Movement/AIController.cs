using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // Movement Values =========================
    [Header("Movement")]
    public float sprint;         // Sprint speed
    public float dashDistance;   // Dash speed
    public float jumpPower;      // Jump height

    // States ===============================================================
    public enum PlayerStates { Crouching, Grounded, Airborne };
    public enum GroundStates { Neutral, Dash, Backdash, Sprint, Stun, Jump };
    public enum AirStates    { Rising, Falling };
    [Header("States")]
    public PlayerStates pState;
    public GroundStates gState;
    public AirStates    aState;

    // The AI needs a decision making process, but they need something to decide why they would make a decision
    // Rather than just always being optimal, the playstyle should decide what they want to do at any given time
    // todo Currently only rushdown is in use, the rest will be implemented with time
    public enum Playstyle { Rushdown, Adaptive, Turtle };
    public Playstyle playStyle;

    public enum Side { Left, Right };
    public Side currentSide;

    // Tracking ====================================================
    [Header("Tracking")]
    public GameObject opponent;  // Used for AI tracking of opponent
    public float      reach;     // Check for punching range
    public float      dashReach; // Check for approach range
    [SerializeField]
    [Tooltip("Range at which the AI wants to backdash")]
    private float     shyRange;

    // Delay =======================================
    [Header("Delay")]
    [SerializeField]
    private int aFatigue;        // Attack fatigue
    [SerializeField]
    private int mFatigue;        // Movement fatigue

    // Other Components ========================
    private AIPhysics physics;
    private AIAttackController attackController;

    // Calculated Values ==================================================================
    private int fatigue;         // Inherits one of the above fatigues to slow down actions
    private int ticker;          // Countdown for aggression

    // Prevents knockback auto-block
    public bool blocking { get; private set; }

    // Start is called before the first frame update
    void OnEnable()
    {
        physics = GetComponent<AIPhysics>();
        attackController = GetComponent<AIAttackController>();
        fatigue = mFatigue;
        ticker = 0;
    }

    private void OnDisable()
    {
        gState = GroundStates.Neutral;
        pState = PlayerStates.Crouching;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameStateControl.gameState == GameStateControl.GameState.Pause)
        {
            return;
        }

        if (attackController.stunLimit > 0)
        {
            ticker = attackController.stunLimit;
        }
        if (ticker <= 0)
        {
            DecisionMaker();
            ticker = fatigue;
        }
        ticker--;
    }

    // When you're almost inside each other and can no longer land anything
    private bool TooCloseForComfort()
    {
        if (Math.Abs(transform.position.x - opponent.transform.position.x) < shyRange)
        {
            return true;
        }

        return false;
    }

    // Check the range between the fighters
    private bool CheckInRange(bool armsReach, bool vertical = false)
    {
        float reachDistance = armsReach ? reach : dashReach;

        if (!vertical)
        {
            if (Math.Abs(transform.position.x - opponent.transform.position.x) < reachDistance) return true;
        }
        else
        {
            if (Math.Abs(transform.position.y - opponent.transform.position.y) == 0.0f) return true;
        }
        return false;
    }

    private void SendMovementSignal(GroundStates state)
    {
        switch (state)
        {
            case GroundStates.Dash:
                if (currentSide == Side.Left) physics.travel += dashDistance;
                if (currentSide == Side.Right) physics.travel -= dashDistance;
                break;
            case GroundStates.Backdash:
                if (currentSide == Side.Left) physics.travel -= dashDistance;
                if (currentSide == Side.Right) physics.travel += dashDistance;
                blocking = true;
                break;
            case GroundStates.Sprint:
                physics.startSprint = true;
                if (currentSide == Side.Left) physics.travel = sprint;
                if (currentSide == Side.Right) physics.travel = -sprint;
                break;
            case GroundStates.Jump:
                physics.launch += jumpPower;
                break;
        }
    }

    private void SendAttackSignal()
    {
        if (attackController.allowFollowup)
        {
            attackController.currentAttack = attackController.currentAttack.Followup;
            return;
        }

        switch (pState)
        {
            case PlayerStates.Crouching:
                attackController.currentAttack = attackController.FindAttack((BaseAttack.AttackType)UnityEngine.Random.Range(1, 3));
                break;
            case PlayerStates.Grounded:
                switch (gState)
                {
                    case GroundStates.Dash:
                        attackController.currentAttack = attackController.FindAttack((BaseAttack.AttackType)UnityEngine.Random.Range(1, 3));
                        break;
                    case GroundStates.Sprint:
                        attackController.currentAttack = attackController.FindAttack((BaseAttack.AttackType)UnityEngine.Random.Range(1, 3));
                        break;
                }
                break;
            case PlayerStates.Airborne:
                switch (aState)
                {
                    case AirStates.Rising:
                        if (opponent.transform.position.y > transform.position.y) attackController.currentAttack = attackController.FindAttack(BaseAttack.AttackType.Punch);
                        break;
                    case AirStates.Falling:
                        if (opponent.transform.position.y < transform.position.y) attackController.currentAttack = attackController.FindAttack(BaseAttack.AttackType.Kick);
                        break;
                }
                break;
        }

        if (attackController.currentAttack == null) return;

        fatigue = aFatigue + attackController.currentAttack.Speed.z;

        if (attackController.currentAttack.Followup == null)
        {
            ticker += attackController.currentAttack.Speed.z;
        }
        else
        {
            ticker += UnityEngine.Random.Range(attackController.currentAttack.Speed.y, attackController.currentAttack.Speed.z);
            attackController.allowFollowup = true;
        }
    }

    private void SendThrowSignal()
    {
        switch (pState)
        {
            case PlayerStates.Grounded:
                switch (gState)
                {
                    case GroundStates.Dash:
                        attackController.currentAttack = attackController.FindAttack(BaseAttack.AttackType.Throw);
                        break;
                }
                break;
        }
    }

    private void DecisionMaker()
    {
        PlayerController opponentController = opponent.GetComponent<PlayerController>();
        PlayerAttackController opponentAtkController = opponent.GetComponent<PlayerAttackController>();

        if (GetComponent<HealthManager>().currentHealth <= 0)
        {
            return;
        }

        if (attackController.currentAttack == null)
        {
            attackController.allowFollowup = false;
        }

        // todo This seems inefficient once rushdown, always rushdown right? Might need to come back to this one
        if (playStyle == Playstyle.Rushdown)
        {
            // First we check if we're within hitting range or not
            if (!CheckInRange(true))
            {
                attackController.allowFollowup = false;
                // So we're not within attack range, that means we need to advance
                if (pState == PlayerStates.Grounded)
                {
                    // That means we need to dash
                    if (gState != GroundStates.Dash && gState != GroundStates.Sprint)
                    {
                        SendMovementSignal(GroundStates.Dash);
                        fatigue = mFatigue;
                        return;
                    }

                    // If we are already dashing, it's a good time to check on the opponent
                    if (opponentController.pState == PlayerController.PlayerStates.Grounded)
                    {
                        if (!CheckInRange(false))
                        {
                            // We're approaching, but they're far enough that one dash isn't enough, we can kick up a sprint here
                            SendMovementSignal(GroundStates.Sprint);
                        }
                        // Otherwise let the dash ride, we can still make it
                    }
                }
            }
            else
            {
                // We're within range, we should probably throw out some kinda attack
                // Lets deal with that in another script at another time, we're just focusing on movement right now

                // That being said, first we need to stop moving towards the player, and if that movement is a sprint, it needs to be stopped manually
                physics.startSprint = false;

                if (gState == GroundStates.Dash && pState != PlayerStates.Crouching)
                {
                    // I just made it 1 in 3 since it means it has an equal chance as standard punch or kick
                    if (UnityEngine.Random.Range(0, 3) == 0)
                    {
                        physics.enableCrouch = true;
                        ticker = mFatigue;
                        return;
                    }
                    if (opponentAtkController.state == PlayerAttackController.AttackState.Startup)
                    {
                        SendThrowSignal();
                    }
                }

                if (pState == PlayerStates.Grounded)
                {
                    if (!CheckInRange(false, true))
                    {
                        // We're within range, but they're too high up
                        SendMovementSignal(GroundStates.Jump);
                        fatigue = mFatigue;
                        return;
                    }

                    // Otherwise continue as normal
                    if (gState == GroundStates.Neutral)
                    {
                        // Getting within comfortable physical violence range
                        SendMovementSignal(TooCloseForComfort() ? GroundStates.Backdash : GroundStates.Dash);
                        fatigue = mFatigue;
                    }
                    else
                    {
                        SendAttackSignal();
                    }
                }
                else if (pState == PlayerStates.Airborne)
                {
                    // So we're within attack range, and we're in the air, we can throw something out
                    SendAttackSignal();
                }
            }
        }
    }

    public void DisableBlock()
    {
        blocking = false;
    }
}