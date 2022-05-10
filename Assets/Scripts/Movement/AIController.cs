using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public float sprint;
    public float dashDistance;
    public float jumpPower;

    public enum PlayerStates { Crouching, Grounded, Airborne };
    public enum GroundStates { Neutral, Dash, Backdash, Sprint, Stun, Jump };
    public enum AirStates    { Rising, Falling };
    public PlayerStates pState;
    public GroundStates gState;
    public AirStates    aState;
    public enum Playstyle { Rushdown, Adaptive, Turtle }; // The AI needs a decision making process, but they need something to decide why they would make a decision
    public Playstyle playStyle;                           // Rather than just always being optimal, the playstyle should decide what they want to do at any given time

    public enum Side { Left, Right };
    public Side currentSide;

    public GameObject opponent;  // Used for AI tracking of opponent
    public float      reach;     // Check for punching range
    public float      dashReach; // Check for approach range

    private SetControls controls;
    private AIPhysics physics;
    private AIAttackController attackController;
    private int fatigue; // Decision fatigue is how high to set the timer before making another decision
    private int ticker;  // Countdown for aggression

    // Start is called before the first frame update
    void Start()
    {
        controls = GetComponent<SetControls>();
        physics = GetComponent<AIPhysics>();
        attackController = GetComponent<AIAttackController>();
        fatigue = 20;
        ticker = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        // todo repeat declarations
        Vector3 pos = transform.position;
        Vector3 oppPos = opponent.transform.position;

        // todo Might need this to be a variable or something
        if (Math.Abs(pos.x - oppPos.x) < 2)
        {
            return true;
        }

        return false;
    }

    // Check the range between the fighters
    private bool CheckInRange(bool armsReach, bool vertical = false)
    {
        Vector3 pos = transform.position;
        Vector3 oppPos = opponent.transform.position;

        float reachDistance = armsReach ? reach : dashReach;

        if (!vertical)
        {
            if (Math.Abs(pos.x - oppPos.x) < reachDistance) return true;
        }
        else
        {
            if (Math.Abs(pos.y - oppPos.y) == 0.0f) return true;
        }
        return false;
    }

    private void SendMovementSignal(GroundStates state)
    {
        Vector3 pos = transform.position;
        Vector3 oppPos = opponent.transform.position;
        switch (state)
        {
            case GroundStates.Dash:
                if (currentSide == Side.Left) physics.travel += dashDistance;
                if (currentSide == Side.Right) physics.travel -= dashDistance;
                break;
            case GroundStates.Backdash:
                if (currentSide == Side.Left) physics.travel -= dashDistance;
                if (currentSide == Side.Right) physics.travel += dashDistance;
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
        Vector3 pos = transform.position;
        Vector3 oppPos = opponent.transform.position;

        if (attackController.allowFollowup)
        {
            attackController.currentAttack = attackController.currentAttack.Followup;
            return;
        }

        switch (pState)
        {
            case PlayerStates.Grounded:
                switch (gState)
                {
                    case GroundStates.Dash:
                        attackController.currentAttack = attackController.FindAttack((BaseAttack.AttackType)UnityEngine.Random.Range(1, 3));
                        break;
                    case GroundStates.Sprint:
                        attackController.currentAttack = attackController.FindAttack(BaseAttack.AttackType.Punch);
                        break;
                }
                break;
            case PlayerStates.Airborne:
                switch (aState)
                {
                    case AirStates.Rising:
                        if (oppPos.y > pos.y) attackController.currentAttack = attackController.FindAttack(BaseAttack.AttackType.Punch);
                        break;
                    case AirStates.Falling:
                        if (oppPos.y < pos.y) attackController.currentAttack = attackController.FindAttack(BaseAttack.AttackType.Kick);
                        break;
                }
                break;
        }

        if (attackController.currentAttack == null) return;

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

    private void DecisionMaker()
    {
        PlayerController opponentController = opponent.GetComponent<PlayerController>();
        PlayerAttackController opponentAtkController = opponent.GetComponent<PlayerAttackController>();

        // todo This seems inefficient once rushdown, always rushdown right? Might need to come back to this one
        if (playStyle == Playstyle.Rushdown)
        {
            // First we check if we're within hitting range or not
            if (!CheckInRange(true))
            {
                // So we're not within attack range, that means we need to advance
                if (pState == PlayerStates.Grounded)
                {
                    // That means we need to dash
                    if (gState != GroundStates.Dash && gState != GroundStates.Sprint)
                    {
                        SendMovementSignal(GroundStates.Dash);
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
                    // On the other hand if they're in the air this is gonna get a little complicated
                    // todo approaching airborne?
                    else if (opponentController.pState == PlayerController.PlayerStates.Airborne)
                    {
                        int distance = 0;
                        int threshold = 0;

                        // Depending on if the opponent is going to hit the ground or not, we can have different responses
                        if (opponentController.aState == PlayerController.AirStates.Rising)
                        {
                            // My logic behind this is that if they're going up, they probably recently jumped and have some airborne time left
                            // That means to reach them you probably might need a jump

                            // You can't just jump though, they have to be within a range where jumping should actually catch them, for the most part
                            if (distance > threshold)
                            {
                                // Also at this point we may trigger a sprint since that will increase the jump height
                                // We would also jump because we're somewhere within the range of catching them
                            }
                        }
                        else if (opponentController.aState == PlayerController.AirStates.Falling)
                        {
                            if (distance > threshold)
                            {
                                // We would trigger a sprint here to get closer for the attack
                            }
                        }
                    }
                }
                // If you do jump for an attack, or jump with an attack and there's a whiff, you may still have a decision to make
                else if (pState == PlayerStates.Airborne)
                {
                    if (aState == AirStates.Rising)
                    {
                        //if (AI.pos.y < Opp.pos.y && distance < threshold) // About close enough to aim for an uppercut
                    }
                    else if (aState == AirStates.Falling)
                    {
                        //if (AI.pos.y > Opp.pos.y && distance < threshold) // About close enough to recover with a rough dive kick
                    }
                }
            }
            else
            {
                // We're within range, we should probably throw out some kinda attack
                // Lets deal with that in another script at another time, we're just focusing on movement right now

                // That being said, first we need to stop moving towards the player, and if that movement is a sprint, it needs to be stopped manually
                physics.startSprint = false;

                if (pState == PlayerStates.Grounded)
                {
                    if (!CheckInRange(false, true))
                    {
                        // We're within range, but they're too high up
                        SendMovementSignal(GroundStates.Jump);
                        return;
                    }

                    // Otherwise continue as normal
                    if (gState == GroundStates.Neutral)
                    {
                        // Getting within comfortable physical violence range
                        SendMovementSignal(TooCloseForComfort() ? GroundStates.Backdash : GroundStates.Dash);
                    }
                    else
                    {
                        //Debug.Log(attackController.currentAttack);
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
}
