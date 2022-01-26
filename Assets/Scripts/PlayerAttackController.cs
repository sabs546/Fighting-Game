using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    public enum AttackType { Punch, Kick, Throw };
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
        sprite = GetComponent<SpriteRenderer>();
        state = AttackState.Empty;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(controls.Punch) && state == AttackState.Empty)
        {
            currentAttack = FindAttack(AttackType.Punch);
            if (currentAttack != null)
            {
                state = AttackState.Startup;
            }
        }
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

    BaseAttack FindAttack(AttackType attackType)
    {
        switch (controller.pState)
        {
            case PlayerController.PlayerStates.Grounded:
                switch (controller.gState)
                {
                    case PlayerController.GroundStates.Neutral:
                        break;

                    case PlayerController.GroundStates.Dash:
                        if (attackType == AttackType.Punch)
                        {
                            return new DashPunch();
                        }
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
