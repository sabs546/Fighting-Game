using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    public BaseAttack currentAttack;
    private BaseAttack nextAttack;

    public enum AttackState { Empty, Startup, Active, Recovery };
    public AttackState state;

    PlayerController controller;
    SpriteRenderer sprite;
    SetControls controls;
    private int timer;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        controls = GetComponent<SetControls>();
        currentAttack = new BaseAttack();
        nextAttack = new BaseAttack();
        sprite = GetComponent<SpriteRenderer>();
        state = AttackState.Empty;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // First attack
        if (state == AttackState.Empty)
        {
            if (Input.GetKeyDown(controls.Punch))
            {
                currentAttack = FindAttack(controls.Punch);
                if (currentAttack != null)
                {
                    state = AttackState.Startup;
                    nextAttack = currentAttack.followup;
                }
            }
            else if (Input.GetKeyDown(controls.Kick))
            {
                currentAttack = FindAttack(controls.Kick);
                if (currentAttack != null)
                {
                    state = AttackState.Startup;
                    nextAttack = currentAttack.followup;
                }
            }
        }
        // The next part of the string
        else if (state == AttackState.Recovery)
        {
            if ((Input.GetKeyDown(controls.Punch) && nextAttack.attackType == BaseAttack.AttackType.Punch) ||
                (Input.GetKeyDown(controls.Kick)  && nextAttack.attackType == BaseAttack.AttackType.Kick))
            {
                currentAttack = nextAttack;
                if (currentAttack != null)
                {
                    state = AttackState.Startup;
                    timer = 0;
                    Debug.Log(currentAttack.ToString());
                    if (currentAttack.followup != null)
                    {
                        nextAttack = currentAttack.followup;
                    }
                }
            }
        }

        // What happens in the attack state
        if (state != AttackState.Empty)
        {
            timer++;
            if (timer < currentAttack.startup)
            {
                state = AttackState.Startup;
                sprite.color = Color.cyan;
            }
            else if (timer < currentAttack.active)
            {
                state = AttackState.Active;
                sprite.color = Color.red;
            }
            else if (timer < currentAttack.recovery)
            {
                state = AttackState.Recovery;
                sprite.color = Color.yellow;
            }
            else
            {
                state = AttackState.Empty;
                sprite.color = Color.white;
                timer = 0;
            }
        }
    }

    BaseAttack FindAttack(KeyCode attackType)
    {
        switch (controller.pState)
        {
            case PlayerController.PlayerStates.Grounded:
                switch (controller.gState)
                {
                    case PlayerController.GroundStates.Neutral:
                        break;

                    case PlayerController.GroundStates.Dash:
                        if (attackType == controls.Punch) { return new DashPunch(); }
                        break;

                    case PlayerController.GroundStates.Sprint:
                        break;
                }
                break;

            case PlayerController.PlayerStates.Airborne:
                break;

            case PlayerController.PlayerStates.Crouching:
                break;
        }
        return null;
    }
}
