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
    private PlayerPhysics physics;
    private AIAttackController attackController;

    // Start is called before the first frame update
    void Start()
    {
        controls = GetComponent<SetControls>();
        physics = GetComponent<PlayerPhysics>();
        attackController = GetComponent<AIAttackController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // --------------------------------------------------------
        // - Movement Inputs -
        // -------
        // You shouldn't be able to move while attacking
        if (attackController.state != AIAttackController.AttackState.Empty || gState == GroundStates.Stun)
        {
            if (attackController.state == AIAttackController.AttackState.Startup)
            {
                
            }
            else if (attackController.state == AIAttackController.AttackState.Active)
            {
                
            }
            else if (attackController.state == AIAttackController.AttackState.Recovery)
            {

            }

            // Sprint attacks need to still cancel
            if ((Input.GetKeyUp(controls.Left) || Input.GetKeyUp(controls.Right)) && gState != GroundStates.Neutral)
            {
                physics.startSprint = false;
                physics.travel = 0.0f;
            }
        }
        else if (pState == PlayerStates.Grounded && gState != GroundStates.Stun)
        {
            if (Input.GetKeyDown(controls.Up))
            {
                physics.launch += jumpPower;
            }

            if (Input.GetKeyDown(controls.Down))
            {
                if (gState == GroundStates.Dash)
                {
                    physics.enableCrouch = true;
                }
            }

            if (Input.GetKeyDown(controls.Left) && gState == GroundStates.Neutral)
            {
                physics.travel -= dashDistance;
            }
            else if (Input.GetKeyDown(controls.Left) && gState != GroundStates.Neutral)
            {
                physics.startSprint = true;
                physics.travel = -sprint;
            }
            else if (Input.GetKeyUp(controls.Left) && gState != GroundStates.Neutral)
            {
                physics.startSprint = false;
                physics.travel = 0.0f;
            }

            if (Input.GetKeyDown(controls.Right) && gState == GroundStates.Neutral)
            {
                physics.travel += dashDistance;
            }
            else if (Input.GetKeyDown(controls.Right) && gState != GroundStates.Neutral)
            {
                physics.startSprint = true;
                physics.travel = sprint;
            }
            else if (Input.GetKeyUp(controls.Right) && gState != GroundStates.Neutral)
            {
                physics.startSprint = false;
                physics.travel = 0.0f;
            }

            // For missed input ups
            if (gState == GroundStates.Sprint && !Input.GetKey(controls.Left) && !Input.GetKey(controls.Right))
            {
                physics.startSprint = false;
            }
        }
        else if (pState == PlayerStates.Airborne)
        {
            if (gState == GroundStates.Sprint)
            {
                if (Input.GetKeyDown(controls.Left) && physics.airLock == -1)
                {
                    physics.startSprint = true;
                    physics.travel = -sprint;
                }
                else if (Input.GetKeyDown(controls.Right) && physics.airLock == 1)
                {
                    physics.startSprint = true;
                    physics.travel = sprint;
                }

                if (Input.GetKeyUp(controls.Left) || Input.GetKeyUp(controls.Right))
                {
                    physics.startSprint = false;
                    physics.travel = 0.0f;
                }
            }
        }
    }

    // Check the range between the fighters
    private bool CheckInRange(bool armsReach, bool vertical = false)
    {
        Vector3 pos = transform.position;
        Vector3 oppPos = opponent.transform.position;
        float reachDistance = armsReach ? reach : dashReach;

        if (!vertical)
        {
            float posX = Math.Abs(transform.position.x);
            float oppX = Math.Abs(opponent.transform.position.x);

            if (Math.Abs(posX - oppX) < reach)
            {
                return true;
            }
        }
        return false;
    }

    private void DecisionMaker()
    {
        PlayerController opponentController = opponent.GetComponent<PlayerController>();
        PlayerAttackController opponentAtkController = opponent.GetComponent<PlayerAttackController>();
        bool attackRange = false; // This will tell you if you're in attack range

        if (playStyle == Playstyle.Rushdown)
        {
            /// attackRange = CheckInRange(true);
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
                    /// if (gState != GroundedStates.Dash)
                    /// {
                            // Just a note that the SendMovementSignal function might need overloads for different enum types
                    ///     SendMovementSignal(GroundStates.Dash);
                    ///     return;
                    /// }
                    // If they're grounded and you're grounded and have begun approaching, we can just gauge distance
                    if (opponentController.pState == PlayerController.PlayerStates.Grounded)
                    {
                        // We've aready started our approach, lets pretend they are too far away and we need to sprint instead
                        /// if (CheckInRange(false))
                        /// {
                                // Considering we're out of dash range, we can kick up a sprint here
                        ///     SendMovementSignal(GroundStates.Sprint);
                        /// }
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
            }
        }
    }
}
