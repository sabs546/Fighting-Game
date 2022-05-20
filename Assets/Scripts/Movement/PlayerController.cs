﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    public enum Side { Left, Right };
    public Side currentSide;

    private SetControls controls;
    private PlayerPhysics physics;
    private PlayerAttackController attackController;

    // Start is called before the first frame update
    void OnEnable()
    {
        controls = GetComponent<SetControls>();
        physics = GetComponent<PlayerPhysics>();
        attackController = GetComponent<PlayerAttackController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
}
