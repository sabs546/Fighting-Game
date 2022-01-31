using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    // Attack Values =======================================================
    public BaseAttack currentAttack;
    private BaseAttack nextAttack;
    
    public enum AttackState { Empty, Startup, Active, Recovery };
    public AttackState state;

    // Player Values =======================================================
    private PlayerController controller;   // Player control
    private SpriteRenderer sprite;         // Sprite control
    private SetControls controls;          // Attack controls

    // Attack Traits =======================================================
    private PlayerPhysics physics;         // For your own knockback
    private PlayerPhysics opponentPhysics; // For the opponents knockback
    private BoxCollider2D hitbox;          // The hitbox of the attack
    private int timer;                     // Frame counter

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

        physics = GetComponent<PlayerPhysics>();
        opponentPhysics = physics.opponent.GetComponent<PlayerPhysics>();
        hitbox = gameObject.AddComponent<BoxCollider2D>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;
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
                    if (transform.position.x > opponentPhysics.transform.position.x) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    nextAttack = currentAttack.Followup;
                }
            }
            else if (Input.GetKeyDown(controls.Kick))
            {
                currentAttack = FindAttack(controls.Kick);
                if (currentAttack != null)
                {
                    if (transform.position.x > opponentPhysics.transform.position.x) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    nextAttack = currentAttack.Followup;
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
                    if (transform.position.x > opponentPhysics.transform.position.x) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    timer = 0;
                    Debug.Log(currentAttack.ToString());
                    if (currentAttack.Followup != null)
                    {
                        nextAttack = currentAttack.Followup;
                    }
                }
            }
        }

        // What happens in the attack state
        if (state != AttackState.Empty)
        {
            timer++;
            if (timer < currentAttack.Speed.x) // During attack startup
            {
                state = AttackState.Startup;
                sprite.color = Color.cyan;
            }
            else if (timer < currentAttack.Speed.y) // During attack active
            {
                state = AttackState.Active;
                sprite.color = Color.red;
                hitbox.enabled = true;
                hitbox.offset = currentAttack.Range;
                hitbox.size = currentAttack.Size;
                if (currentAttack.AlwaysRecoil)
                {
                    physics.travel += sprite.flipX ? currentAttack.Recoil.x / WorldRules.physicsRate : -currentAttack.Recoil.x / WorldRules.physicsRate;
                }
            }
            else if (timer < currentAttack.Speed.z) // During attack recovery
            {
                state = AttackState.Recovery;
                sprite.color = Color.yellow;
                hitbox.enabled = false;
                if (controller.gState == PlayerController.GroundStates.Sprint) physics.startSprint = false;
            }
            else // After attack completes
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
                        if (attackType == controls.Punch) { return new SprintPunch(); }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        physics.launch -= currentAttack.Recoil.y / WorldRules.physicsRate;
        if (!currentAttack.AlwaysRecoil) physics.travel -= currentAttack.Recoil.x / WorldRules.physicsRate;
        opponentPhysics.launch += currentAttack.Knockback.y / WorldRules.physicsRate;
        opponentPhysics.travel += currentAttack.Knockback.x / WorldRules.physicsRate;
    }
}
