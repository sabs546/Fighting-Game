using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    // Attack Values =======================================================================================
    public BaseAttack currentAttack;                              // Attack currently playing
    private BaseAttack nextAttack;                                // The next available attack in the string
    
    public enum AttackState { Empty, Startup, Active, Recovery }; // Attack phases
    public AttackState state;                                     // And what will hold them

    // Player Values ========================================
    private PlayerController controller;   // Player control
    private SpriteRenderer sprite;         // Sprite control
    private SetControls controls;          // Attack controls

    // Attack Traits ====================================================
    private PlayerPhysics physics;         // For your own knockback
    private AIPhysics opponentPhysics; // For the opponents knockback
    private BoxCollider2D hitbox;          // The hitbox of the attack
    private int timer;                     // Frame counter
    public  HitSparkManager hitSpark;      // The hit sparks
    public int  stunLimit;                 // How long the stun lasts
    public bool enableLowAttacks;          // Unlock crouch attacks

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
        stunLimit = 0;

        physics = GetComponent<PlayerPhysics>();
        opponentPhysics = physics.opponent.GetComponent<AIPhysics>();
        hitbox = gameObject.AddComponent<BoxCollider2D>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // First attack
        if (state == AttackState.Empty && controller.gState != PlayerController.GroundStates.Stun)
        {
            if (Input.GetKeyDown(controls.Punch))
            {
                currentAttack = FindAttack(controls.Punch);
                if (currentAttack != null)
                {
                    if (sprite.flipX) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    nextAttack = currentAttack.Followup;
                }
            }
            else if (Input.GetKeyDown(controls.Kick))
            {
                currentAttack = FindAttack(controls.Kick);
                if (currentAttack != null)
                {
                    if (sprite.flipX) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    nextAttack = currentAttack.Followup;
                }
            }
        }
        // The next part of the string
        else if (state == AttackState.Recovery)
        {
            if (nextAttack != null && ((Input.GetKeyDown(controls.Punch) && nextAttack.attackType == BaseAttack.AttackType.Punch) ||
                                       (Input.GetKeyDown(controls.Kick)  && nextAttack.attackType == BaseAttack.AttackType.Kick)))
            {
                currentAttack = nextAttack;
                GetComponent<SpriteManager>().EnableFollowup(true);
                if (currentAttack != null)
                {
                    if (sprite.flipX) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    timer = 0;
                    if (currentAttack.Followup != null)
                    {
                        nextAttack = currentAttack.Followup;
                    }
                }
            }
        }
        // This one is an odd case, where an attack transitions into another attack automatically
        else if (state == AttackState.Active)
        {
            if (currentAttack is FallingKick && controller.pState == PlayerController.PlayerStates.Grounded)
            {
                currentAttack = new SlideKick();
                state = AttackState.Startup;
                timer = 0;
                currentAttack.RemoveRecoil();
            }
        }

        // If there is some stun, start the clock
        if (stunLimit > 0)
        {
            if (timer == 0)
            {
                controller.gState = PlayerController.GroundStates.Stun;
                GetComponent<Animator>().SetBool("Stun", true);
            }

            if (timer < stunLimit)
            {
                timer++;
            }
            else
            {
                controller.gState = PlayerController.GroundStates.Neutral;
                GetComponent<Animator>().SetBool("Stun", false);
                stunLimit = 0;
                timer = 0;
            }
        }

        // What happens in the attack state
        if (state != AttackState.Empty)
        {
            timer++;
            if (timer < currentAttack.Speed.x) // During attack startup
            {
                state = AttackState.Startup;
                if (currentAttack.AlwaysRecoil && !currentAttack.DelayRecoil)
                {
                    physics.travel += sprite.flipX ? currentAttack.Recoil.x / WorldRules.physicsRate : -currentAttack.Recoil.x / WorldRules.physicsRate;
                    physics.launch += currentAttack.Recoil.y / WorldRules.physicsRate;
                    currentAttack.RemoveRecoil();
                }
                if (timer == 1)
                {
                    if (currentAttack.SparkType == HitSparkManager.SparkType.Launch)
                    {
                        GetComponent<AttackAudioManager>().PlaySound("Whiff_Heavy_01");
                    }
                    else if (currentAttack.SparkType == HitSparkManager.SparkType.Mid || currentAttack.SparkType == HitSparkManager.SparkType.Low)
                    {
                        GetComponent<AttackAudioManager>().PlaySound("Whiff_Light_01");
                    }
                }
            }
            else if (timer < currentAttack.Speed.y) // During attack active
            {
                state = AttackState.Active;
                hitbox.enabled = true;
                hitbox.offset = currentAttack.Range;
                hitbox.size = currentAttack.Size;
                if (currentAttack.DelayRecoil)
                {
                    physics.travel += sprite.flipX ? currentAttack.Recoil.x / WorldRules.physicsRate : -currentAttack.Recoil.x / WorldRules.physicsRate;
                    physics.launch += currentAttack.Recoil.y / WorldRules.physicsRate;
                    currentAttack.RemoveRecoil();
                }
            }
            else if (timer < currentAttack.Speed.z) // During attack recovery
            {
                state = AttackState.Recovery;
                hitbox.enabled = false;
                if (controller.gState == PlayerController.GroundStates.Sprint) physics.startSprint = false;
            }
            else // After attack completes
            {
                state = AttackState.Empty;
                timer = 0;
                stunLimit = 0;
                GetComponent<SpriteManager>().EnableFollowup(false);
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
                        if (attackType == controls.Kick) { return new DashKick(); }
                        break;

                    case PlayerController.GroundStates.Sprint:
                        if (attackType == controls.Punch) { return new SprintPunch(); }
                        break;
                }
                break;

            case PlayerController.PlayerStates.Airborne:
                switch (controller.aState)
                {
                    case PlayerController.AirStates.Rising:
                        if (attackType == controls.Punch) { return new RisingPunch(); }
                        break;
                    case PlayerController.AirStates.Falling:
                        if (attackType == controls.Kick) { return new FallingKick(); }
                        break;
                }
                break;

            case PlayerController.PlayerStates.Crouching:
                if (attackType == controls.Kick) { return new SlideKick(); }
                break;
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // todo This should never trigger, but triggers are always active I guess, which is odd because it doesn't trigger anyway
        if (currentAttack == null)
        {
            //Debug.Log(hitbox.name + " " + hitbox.enabled);
            return;
        }
        // Weight stuff
        physics.launch -= currentAttack.Recoil.y / WorldRules.physicsRate;
        opponentPhysics.travel += currentAttack.Knockback.x / WorldRules.physicsRate;
        opponentPhysics.launch += currentAttack.Knockback.y / WorldRules.physicsRate;
        if (!currentAttack.AlwaysRecoil)
        {
            if (currentAttack.Recoil != Vector2.zero)
            {
                physics.travel -= currentAttack.Recoil.x / WorldRules.physicsRate;
            }
            else
            {
                physics.Brake();
                opponentPhysics.Brake();
            }
        }
        
        // Hitspark stuff
        if (opponentPhysics.GetComponent<AIController>().pState == AIController.PlayerStates.Grounded)
        {
            opponentPhysics.GetComponent<AIAttackController>().stunLimit = currentAttack.Stun;
        }

        Vector2 sparkPos = new Vector2(transform.position.x + (!sprite.flipX ? transform.lossyScale.x : -transform.lossyScale.x), transform.position.y);
        if (currentAttack.SparkType == HitSparkManager.SparkType.Launch)
        {
            sparkPos.y += transform.lossyScale.y * 0.5f;
        }
        hitSpark.CreateHitSpark(currentAttack.SparkType, sparkPos.x, sparkPos.y, !sprite.flipX, physics.travel, controller.pState);

        // Other stuff
        opponentPhysics.GetComponent<HealthManager>().SendDamage(currentAttack.Damage);
        GetComponent<AttackAudioManager>().PlaySound(currentAttack.SoundName);
        timer = currentAttack.Speed.y;
    }
}
