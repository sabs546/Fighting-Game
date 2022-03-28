using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpriteManager : MonoBehaviour
{
    AIController controller;
    AIAttackController atkController;
    Animator animator;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<AIController>();
        atkController = GetComponent<AIAttackController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
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
            }
            else if (atkController.currentAttack.attackType == BaseAttack.AttackType.Kick)
            {
                animator.SetBool("Kick", true);
                animator.SetBool("Punch", false);
            }
        }
        else
        {
            animator.SetBool("Punch", false);
            animator.SetBool("Kick", false);
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
            }
            else if (controller.gState == AIController.GroundStates.Backdash)
            {
                animator.SetInteger("XDir", -1);
            }
            else if (controller.gState == AIController.GroundStates.Sprint)
            {
                animator.SetBool("Sprint", true);
                sprite.flipX = controller.currentSide == AIController.Side.Right ? true : false;
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

        // Can't turn around mid attack
        if (atkController.state != AIAttackController.AttackState.Recovery)
        {
            sprite.flipX = controller.currentSide == AIController.Side.Right ? true : false;
        }
    }

    public void EnableFollowup(bool enable)
    {
        animator.SetBool("Followup", enable);
    }
}
