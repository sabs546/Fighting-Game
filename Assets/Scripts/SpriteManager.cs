using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    PlayerController controller;
    PlayerAttackController atkController;
    Animator animator;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        atkController = GetComponent<PlayerAttackController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetInteger("XDir", 0);
        animator.SetInteger("YDir", 0);
        animator.SetBool("Sprint", false);
        if (atkController.state != PlayerAttackController.AttackState.Empty)
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

        if (controller.pState == PlayerController.PlayerStates.Grounded)
        {
            if (controller.gState == PlayerController.GroundStates.Dash)
            {
                animator.SetInteger("XDir", 1);
            }
            else if (controller.gState == PlayerController.GroundStates.Backdash)
            {
                animator.SetInteger("XDir", -1);
            }
            else if (controller.gState == PlayerController.GroundStates.Sprint)
            {
                animator.SetBool("Sprint", true);
                sprite.flipX = controller.currentSide == PlayerController.Side.Right ? true : false;
            }
        }
        else if (controller.pState == PlayerController.PlayerStates.Airborne)
        {
            if (controller.aState == PlayerController.AirStates.Rising)
            {
                animator.SetInteger("YDir", 1);
            }
            else if (controller.aState == PlayerController.AirStates.Falling)
            {
                animator.SetInteger("YDir", -1);
            }
        }

        sprite.flipX = controller.currentSide == PlayerController.Side.Right ? true : false;
    }
}
