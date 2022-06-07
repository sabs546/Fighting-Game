﻿using System;
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
    private int aFatigue; // Attack fatigue
    private int mFatigue; // Movement fatigue
    private int fatigue;  // Decision fatigue is how high to set the timer before making another decision
    private int ticker;   // Countdown for aggression

    public Queue<GenericAction> commandHistory; // This should help when the AI gets stuck in a loop
    private GenericAction containerAction;      // Action will be reused to add to the command history
    private bool forceAttack;

    // Start is called before the first frame update
    void OnEnable()
    {
        controls = GetComponent<SetControls>();
        physics = GetComponent<AIPhysics>();
        attackController = GetComponent<AIAttackController>();
        aFatigue = 20;
        mFatigue = 10;
        fatigue = mFatigue;
        ticker = 0;

        // Start off a list of actions, we're gonna keep the last 3
        commandHistory = new Queue<GenericAction>();
        containerAction = new GenericAction();
        containerAction.SetActionType(0);
        commandHistory.Enqueue(containerAction);
        commandHistory.Enqueue(containerAction);
        commandHistory.Enqueue(containerAction);
        forceAttack = false;
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
            if (attackController.currentAttack == null || attackController.currentAttack.Followup == null)
            {
                Debug.Log("Something wrong with the current attack?");
                return;
            }
            attackController.currentAttack = attackController.currentAttack.Followup;
            return;
        }

        switch (pState)
        {
            case PlayerStates.Crouching:
                attackController.currentAttack = attackController.FindAttack(BaseAttack.AttackType.Kick);
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

    private bool CheckRepeatActions()
    {
        int[] typeList = { 0, 0, 0 };
        int index = 0;
        foreach (GenericAction command in commandHistory)
        {
            typeList[index] = command.GetActionType();
            index++;
        }
        if (typeList[0] == typeList[1] && typeList[1] == typeList[2])
        {
            // Then we just repeated the same type of action 3 times
            return true;
        }
        return false;
    }

    private void DecisionMaker()
    {
        PlayerController opponentController = opponent.GetComponent<PlayerController>();
        PlayerAttackController opponentAtkController = opponent.GetComponent<PlayerAttackController>();

        if (GetComponent<HealthManager>().currentHealth <= 0)
        {
            return;
        }

        if (forceAttack)
        {
            forceAttack = false;
            attackController.allowFollowup = false; // todo there's an issue with allowfollowup being stuck on true, I'll fix it later
            SendAttackSignal();
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
                        commandHistory.Dequeue();
                        commandHistory.Enqueue(containerAction.SetActionType(1));
                        return;
                    }

                    // If we are already dashing, it's a good time to check on the opponent
                    if (opponentController.pState == PlayerController.PlayerStates.Grounded)
                    {
                        if (!CheckInRange(false))
                        {
                            // We're approaching, but they're far enough that one dash isn't enough, we can kick up a sprint here
                            SendMovementSignal(GroundStates.Sprint);
                            commandHistory.Dequeue();
                            commandHistory.Enqueue(containerAction.SetActionType(1));
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

                if (pState == PlayerStates.Grounded || pState == PlayerStates.Crouching)
                {
                    if (!CheckInRange(false, true))
                    {
                        // We're within range, but they're too high up
                        SendMovementSignal(GroundStates.Jump);
                        fatigue = mFatigue;
                        commandHistory.Dequeue();
                        commandHistory.Enqueue(containerAction.SetActionType(1));
                        return;
                    }

                    // Otherwise continue as normal
                    if (gState == GroundStates.Neutral)
                    {
                        // Getting within comfortable physical violence range
                        if (CheckRepeatActions())
                        {
                            SendMovementSignal(GroundStates.Dash);
                            forceAttack = true;
                        }
                        else
                        {
                            SendMovementSignal(TooCloseForComfort() ? GroundStates.Backdash : GroundStates.Dash);
                        }
                        fatigue = mFatigue;
                        commandHistory.Dequeue();
                        commandHistory.Enqueue(containerAction.SetActionType(1));
                    }
                    else
                    {
                        SendAttackSignal();
                        fatigue = aFatigue;
                        commandHistory.Dequeue();
                        commandHistory.Enqueue(containerAction.SetActionType(2));
                    }
                }
                else if (pState == PlayerStates.Airborne)
                {
                    // So we're within attack range, and we're in the air, we can throw something out
                    SendAttackSignal();
                    fatigue = aFatigue;
                    commandHistory.Dequeue();
                    commandHistory.Enqueue(containerAction.SetActionType(2));
                }
            }
        }
    }
}

public class GenericAction
{
    private enum ActionType { Empty, Movement, Attack };
    private ActionType action;

    public GenericAction SetActionType(int type)
    {
        action = (ActionType)type;
        return this;
    }

    public int GetActionType()
    {
        return (int)action;
    }
}