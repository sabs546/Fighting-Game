using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    public enum AttackType { Punch, Kick, Throw };
    private BaseAttack currentAttack;
    private BaseAttack nextAttack;

    public enum AttackState { Empty, Starting, Active, Stopping };
    public AttackState state;

    PlayerController controller;
    SetControls controls;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        controls = GetComponent<SetControls>();
        state = AttackState.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(controls.Punch) && state == AttackState.Empty)
        {
            currentAttack = FindAttack(AttackType.Punch);
            if (currentAttack != null)
            {
                state = AttackState.Starting;
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
                        if (attackType == AttackType.Punch) return new DashPunch();
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
