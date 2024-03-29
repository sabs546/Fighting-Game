﻿using UnityEngine;

public class AISpriteManager : MonoBehaviour
{
    AIController controller;
    AIAttackController atkController;
    Animator animator;
    SpriteRenderer sprite;
    HealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<AIController>();
        atkController = GetComponent<AIAttackController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        healthManager = GetComponent<HealthManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetInteger("XDir", 0);
        animator.SetInteger("YDir", 0);
        animator.SetBool("Sprint", false);
        animator.SetBool("Crouch", false);
        if (atkController.state != AIAttackController.AttackState.Empty)
        {
            if (atkController.currentAttack.attackType == BaseAttack.AttackType.Punch)
            {
                animator.SetBool("Punch", true);
                animator.SetBool("Kick", false);
                animator.SetBool("Throw", false);
            }
            else if (atkController.currentAttack.attackType == BaseAttack.AttackType.Kick)
            {
                animator.SetBool("Punch", false);
                animator.SetBool("Kick", true);
                animator.SetBool("Throw", false);
            }
            else if (atkController.currentAttack.attackType == BaseAttack.AttackType.Throw)
            {
                animator.SetBool("Punch", false);
                animator.SetBool("Kick", false);
                animator.SetBool("Throw", true);
            }
        }
        else
        {
            animator.SetBool("Punch", false);
            animator.SetBool("Kick", false);
            animator.SetBool("Throw", false);
        }

        if (controller.pState == AIController.PlayerStates.Grounded || controller.pState == AIController.PlayerStates.Crouching)
        {
            if (controller.gState == AIController.GroundStates.Dash)
            {
                animator.SetInteger("XDir", 1);
                if (controller.pState == AIController.PlayerStates.Crouching)
                {
                    animator.SetBool("Crouch", true);
                }
                animator.SetBool("Guard", false);
            }
            else if (controller.gState == AIController.GroundStates.Backdash)
            {
                animator.SetInteger("XDir", -1);
            }
            else if (controller.gState == AIController.GroundStates.Sprint)
            {
                animator.SetBool("Sprint", true);
                sprite.flipX = controller.currentSide == AIController.Side.Right ? true : false;
                animator.SetBool("Guard", false);
            }
            else if (controller.gState == AIController.GroundStates.Neutral)
            {
                animator.SetBool("Guard", false);
            }
        }
        else if (controller.pState == AIController.PlayerStates.Airborne)
        {
            if (controller.aState == AIController.AirStates.Rising)
            {
                animator.SetInteger("YDir", 1);
            }
            else if (controller.aState == AIController.AirStates.Falling)
            {
                animator.SetInteger("YDir", -1);
            }
        }

        if (controller.gState == AIController.GroundStates.Stun)
        {
            animator.SetBool("Stun", true);
        }
        else
        {
            animator.SetBool("Stun", false);
        }

        // Can't turn around mid attack
        if (atkController.state != AIAttackController.AttackState.Recovery)
        {
            sprite.flipX = controller.currentSide == AIController.Side.Right ? true : false;
        }

        if (healthManager.currentHealth <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("DeathStun"))
        {
            animator.SetTrigger("Die");
        }
        else if (healthManager.currentHealth > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("DeathStun"))
        {
            animator.SetTrigger("Revive");
        }
    }

    public void EnableFollowup(bool enable)
    {
        animator.SetBool("Followup", enable);
    }

    public void EnableBlock()
    {
        animator.SetBool("Guard", true);
    }

    public void HeavyStun()
    {
        animator.SetBool("HeavyStun", true);
    }

    public void UndoHeavy()
    {
        animator.SetBool("HeavyStun", false);
    }
}
