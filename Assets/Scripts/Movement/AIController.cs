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

    public GameObject opponent;

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

    private void DecisionMaker()
    {
        PlayerController opponentController = opponent.GetComponent<PlayerController>();
        PlayerAttackController opponentAtkController = opponent.GetComponent<PlayerAttackController>();
        bool attackRange = false; // This will tell you if you're in attack range

        if (playStyle == Playstyle.Rushdown)
        {
            // attackRange = CheckIfInAttackRange();
            // Regardless of attack range, we're going to need some sort of movement
            // Being rushdown, movement will be approach, so we're going to assume we dash here
            if (!attackRange)
            {
                // The important part though is that we're not within attack range, so below is what we do from that dash
                // Being aggressive, we've decided already that they're going to approach
                // The question is how?
                // To determine this we're gonna need to know the current status of the opponent, for example grounded
                if (opponentController.pState == PlayerController.PlayerStates.Grounded && pState == PlayerStates.Grounded)
                {
                    // The following are dummy variables for the if statement below
                    int distance = 0;  // So first we find their range from us
                    int threshold = 0; // This will determine if they want to swap to sprint or not

                    // We've aready started our approach, lets pretend they are too far away and we need to sprint instead
                    if (distance > threshold)
                    {
                        // This is where we'd send the signal to start the sprint
                    }
                    // Otherwise continue with the dash decision Until we're within attack range
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
