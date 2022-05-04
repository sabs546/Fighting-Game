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
    public enum GroundStates { Neutral, Dash, Backdash, Sprint, Stun };
    public enum AirStates    { Rising, Falling };
    public PlayerStates pState;
    public GroundStates gState;
    public AirStates    aState;
    public enum Playstyle { Rushdown, Adaptive, Turtle }; // The AI needs a decision making process, but they need something to decide why they would make a decision
    public Playstyle playStyle;                           // Rather than just always being optimal, the playstyle should decide what they want to do at any given time

    public enum Side { Left, Right };
    public Side currentSide;

    public GameObject opponent; // Used for AI tracking of opponent
    public float   reach;       // Check for punching range
    public float   dashReach;   // Check for approach range

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
        if (ticker <= 0)
        {
            DecisionMaker();
            ticker = fatigue;
        }
        ticker--;
    }

    // Check the range between the fighters
    private bool CheckInRange(bool armsReach, bool vertical = false)
    {
        Vector3 pos = transform.position;
        Vector3 oppPos = opponent.transform.position;
        float reachDistance = armsReach ? reach : dashReach;

        if (!vertical)
        {
            if (Math.Abs(pos.x - oppPos.x) < reachDistance)
            {
                return true;
            }
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
            case GroundStates.Sprint:
                physics.startSprint = true;
                if (currentSide == Side.Left) physics.travel = sprint;
                if (currentSide == Side.Right) physics.travel = -sprint;
                break;
        }
    }

    private void SendAttackSignal()
    {

        switch (pState)
        {
            case PlayerStates.Grounded:
                switch (gState)
                {
                    case GroundStates.Dash:
                        attackController.currentAttack = attackController.FindAttack(BaseAttack.AttackType.Punch);
                        fatigue += attackController.currentAttack.Speed.z;
                        break;
                    case GroundStates.Sprint:
                        break;
                }
                break;
            case PlayerStates.Airborne:
                break;
        }
    }

    private void DecisionMaker()
    {
        PlayerController opponentController = opponent.GetComponent<PlayerController>();
        PlayerAttackController opponentAtkController = opponent.GetComponent<PlayerAttackController>();
        bool attackRange = false; // This will tell you if you're in attack range

        if (playStyle == Playstyle.Rushdown)
        {
            attackRange = CheckInRange(true);
            // Regardless of attack range, we're going to need some sort of movement
            // Being rushdown, movement will be approach, but we need to be grounded first, we'll deal with that later
            if (!attackRange)
            {
                // The important part though is that we're not within attack range, so below is what we do from that dash
                // Being aggressive, we've decided already that they're going to approach
                // The question is how?
                // To determine this we're gonna need information on the state of either fighter
                if (pState == PlayerStates.Grounded)
                {
                    // Some projected code, we're gonna need to dash if we're not already dashing and then we can make another decision
                    if (gState != GroundStates.Dash && gState != GroundStates.Sprint)
                    {
                        // Just a note that the SendMovementSignal function might need overloads for different enum types
                        SendMovementSignal(GroundStates.Dash);
                        return;
                    }
                    // If they're grounded and you're grounded and have begun approaching, we can just gauge distance
                    if (opponentController.pState == PlayerController.PlayerStates.Grounded)
                    {
                        // We've aready started our approach, lets pretend they are too far away and we need to sprint instead
                        if (!CheckInRange(false))
                        {
                            // Considering we're out of dash range, we can kick up a sprint here
                            SendMovementSignal(GroundStates.Sprint);
                        }
                        // Otherwise let the dash ride, we can still make it
                    }
                    // On the other hand if they're in the air this is gonna get a little complicated
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
                    if (gState == GroundStates.Neutral)
                    {
                        SendMovementSignal(GroundStates.Dash);
                    }
                    else
                    {
                        SendAttackSignal();
                    }
                }
            }
        }
    }
}
