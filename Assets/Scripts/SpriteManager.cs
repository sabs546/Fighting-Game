using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    PlayerController controller;
    PlayerAttackController atkController;
    Animator animator;
    SpriteRenderer sprite;
    HealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        atkController = GetComponent<PlayerAttackController>();
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
        if (atkController.state != PlayerAttackController.AttackState.Empty)
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

        if (controller.pState == PlayerController.PlayerStates.Grounded || controller.pState == PlayerController.PlayerStates.Crouching)
        {
            if (controller.gState == PlayerController.GroundStates.Dash)
            {
                animator.SetInteger("XDir", 1);
                if (controller.pState == PlayerController.PlayerStates.Crouching)
                {
                    animator.SetBool("Crouch", true);
                }
                animator.SetBool("Guard", false);
            }
            else if (controller.gState == PlayerController.GroundStates.Backdash)
            {
                animator.SetInteger("XDir", -1);
            }
            else if (controller.gState == PlayerController.GroundStates.Sprint)
            {
                animator.SetBool("Sprint", true);
                sprite.flipX = controller.currentSide == PlayerController.Side.Right ? true : false;
                animator.SetBool("Guard", false);
            }
            else if (controller.gState == PlayerController.GroundStates.Neutral)
            {
                animator.SetBool("Guard", false);
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

        if (controller.gState == PlayerController.GroundStates.Stun)
        {
            animator.SetBool("Stun", true);
        }
        else
        {
            animator.SetBool("Stun", false);
            animator.SetBool("HeavyStun", false);
        }

        // Can't turn around mid attack
        if (atkController.state != PlayerAttackController.AttackState.Recovery)
        {
            sprite.flipX = controller.currentSide == PlayerController.Side.Right ? true : false;
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
