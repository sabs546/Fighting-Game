using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    // Attack Values ===================================================================
    public  BaseAttack currentAttack;         // Attack currently playing
    private BaseAttack nextAttack;            // The next available attack in the string
    
    public enum AttackState { Empty, Startup, Active, Recovery };
    public AttackState state;                 // Attack phases and what will hold them

    // Player Values ===========================================
    private PlayerController controller;      // Player control
    private SpriteRenderer   sprite;          // Sprite control
    private SetControls      controls;        // Attack controls

    // Attack Traits =============================================================
    public  PlayerPhysics   physics;          // For your own knockback
    public  PlayerPhysics   p2Physics;        // For the second player's knockback
    public  AIPhysics       opponentPhysics;  // For the opponents knockback
    public  HitSparkManager hitSpark;         // The hit sparks
    public  int             stunLimit;        // How long the stun lasts
    public  bool            enableLowAttacks; // Unlock crouch attacks
    // Private ------------------------------------------------------------------
    private BoxCollider2D   hitbox;           // The hitbox of the attack
    private int             timer;            // Frame counter
    private int             blockStun;        // Stacks on top of attack cooldown

    // Input registering
    [HideInInspector]
    public  bool sendPunch;
    [HideInInspector]
    public  bool sendKick;
    [HideInInspector]
    public  bool sendThrow;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        controls = GetComponent<SetControls>();
        currentAttack = null;
        nextAttack = null;
        sprite = GetComponent<SpriteRenderer>();
        state = AttackState.Empty;
        timer = 0;
        stunLimit = 0;
        blockStun = 0;

        physics = GetComponent<PlayerPhysics>();
        opponentPhysics = physics.opponent.GetComponent<AIPhysics>();
        p2Physics = physics.opponent.GetComponent<PlayerPhysics>();
        hitbox = gameObject.AddComponent<BoxCollider2D>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;

        sendPunch = false;
        sendKick = false;
        sendThrow = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // First attack
        if (state == AttackState.Empty && controller.gState != PlayerController.GroundStates.Stun)
        {
            if (sendPunch)
            {
                sendPunch = false;
                currentAttack = FindAttack(controls.keyboardControls.Punch);
                if (currentAttack != null)
                {
                    if (sprite.flipX) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    nextAttack = currentAttack.Followup;
                }
            }
            else if (sendKick)
            {
                sendKick = false;
                currentAttack = FindAttack(controls.keyboardControls.Kick);
                if (currentAttack != null)
                {
                    if (sprite.flipX) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    nextAttack = currentAttack.Followup;
                }
            }
            else if (sendThrow)
            {
                sendThrow = false;
                currentAttack = FindAttack(controls.keyboardControls.Throw);
                if (currentAttack != null)
                {
                    if (sprite.flipX) { currentAttack.SideSwap(); }
                    state = AttackState.Startup;
                    nextAttack = currentAttack.Followup;
                }
            }
        }
        // The next part of the string
        else if (state == AttackState.Recovery && currentAttack.Followup != null)
        {
            if (nextAttack != null && ((sendPunch && nextAttack.attackType == BaseAttack.AttackType.Punch) ||
                                       (sendKick  && nextAttack.attackType == BaseAttack.AttackType.Kick)))
            {
                sendPunch = false;
                sendKick = false;
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
                GetComponent<AttackAudioManager>().PlaySound("BlockHeavy", 1);
                Camera.main.GetComponent<CameraControl>().StartShake(4, 4u, 0.5f);
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
            }

            if (timer < stunLimit)
            {
                timer++;
            }
            else
            {
                controller.gState = PlayerController.GroundStates.Neutral;
                stunLimit = 0;
                timer = 0;
            }
        }
        else if (controller.gState == PlayerController.GroundStates.Stun && stunLimit == 0)
        {
            controller.gState = PlayerController.GroundStates.Neutral;
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
                    physics.travel -= currentAttack.Recoil.x / WorldRules.physicsRate;
                    physics.launch -= currentAttack.Recoil.y / WorldRules.physicsRate;
                    currentAttack.RemoveRecoil();
                }

                // Special case for air hang time, this is the only attack that does it
                if (currentAttack is FallingKick) physics.Hang();

                if (timer == 1)
                { // Changing the sounds to the whiffed versions
                    if (currentAttack.SparkType == HitSparkManager.SparkType.Launch)
                    {
                        GetComponent<AttackAudioManager>().PlaySound("Whiff_Heavy_01", 0);
                    }
                    else if (currentAttack.SparkType == HitSparkManager.SparkType.Mid || currentAttack.SparkType == HitSparkManager.SparkType.Low)
                    {
                        GetComponent<AttackAudioManager>().PlaySound("Whiff_Light_01", 0);
                    }
                }
            }
            else if (timer < currentAttack.Speed.y) // During attack active
            {
                state = AttackState.Active;
                hitbox.enabled = true;
                hitbox.offset = currentAttack.Range;
                hitbox.size = currentAttack.Size;
                if (currentAttack.DelayRecoil && currentAttack.AlwaysRecoil)
                {
                    physics.travel -= currentAttack.Recoil.x / WorldRules.physicsRate;
                    physics.launch -= currentAttack.Recoil.y / WorldRules.physicsRate;
                    currentAttack.RemoveRecoil();
                }
            }
            else if (timer < currentAttack.Speed.z + blockStun) // During attack recovery
            {
                state = AttackState.Recovery;
                hitbox.enabled = false;
                if (controller.gState == PlayerController.GroundStates.Sprint) physics.startSprint = false;
            }
            else // After attack completes
            {
                blockStun = 0;
                state = AttackState.Empty;
                timer = 0;
                stunLimit = 0;
                GetComponent<SpriteManager>().EnableFollowup(false);
                currentAttack = null;
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
                    case PlayerController.GroundStates.Dash:
                        if (attackType == controls.keyboardControls.Punch) { return new DashPunch(); }
                        if (attackType == controls.keyboardControls.Kick) { return new DashKick(); }
                        if (attackType == controls.keyboardControls.Throw) { return new DashThrow(); }
                        break;
                    case PlayerController.GroundStates.Sprint:
                        if (attackType == controls.keyboardControls.Punch) { return new SprintPunch(); }
                        if (attackType == controls.keyboardControls.Kick) { return new SprintKick(); }
                        if (attackType == controls.keyboardControls.Throw) { return new SprintThrow(); }
                        break;
                }
                break;

            case PlayerController.PlayerStates.Airborne:
                switch (controller.aState)
                {
                    case PlayerController.AirStates.Rising:
                        if (attackType == controls.keyboardControls.Punch) { return new RisingPunch(); }
                        break;
                    case PlayerController.AirStates.Falling:
                        if (attackType == controls.keyboardControls.Kick) { return new FallingKick(); }
                        break;
                }
                if (attackType == controls.keyboardControls.Throw) { return new AirThrow(); }
                break;

            case PlayerController.PlayerStates.Crouching:
                if (attackType == controls.keyboardControls.Punch) { return new CrouchPunch(); }
                if (attackType == controls.keyboardControls.Kick) { return new SlideKick(); }
                break;
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // todo This should never trigger, but triggers are always active I guess, which is odd because it doesn't trigger anyway
        if (currentAttack == null || currentAttack.attackType == BaseAttack.AttackType.Throw)
        {
            return;
        }

        bool blocked = false;

        // Weight stuff
        if (p2Physics == null)
        {
            opponentPhysics.travel += currentAttack.Knockback.x / WorldRules.physicsRate;
            opponentPhysics.launch += currentAttack.Knockback.y / WorldRules.physicsRate;
        }
        else
        {
            p2Physics.travel += currentAttack.Knockback.x / WorldRules.physicsRate;
            p2Physics.launch += currentAttack.Knockback.y / WorldRules.physicsRate;
        }

        if (!currentAttack.AlwaysRecoil || !currentAttack.DelayRecoil)
        {
            if (currentAttack.Recoil != Vector2.zero)
            {
                physics.travel -= currentAttack.Recoil.x / WorldRules.physicsRate;
                physics.launch -= currentAttack.Recoil.y / WorldRules.physicsRate;
                currentAttack.RemoveRecoil();
            }
            else
            {
                physics.Brake();
                if (p2Physics == null) opponentPhysics.Brake();
                else                         p2Physics.Brake();
            }
        }

        GameObject opponent;

        if (!WorldRules.PvP)
        {
            opponent = opponentPhysics.gameObject;
            if (opponent.GetComponent<AIController>().gState == AIController.GroundStates.Backdash && opponent.GetComponent<AIController>().blocking)
            {
                opponent.GetComponent<AISpriteManager>().EnableBlock();
                blocked = true;
                nextAttack = null;
                blockStun = currentAttack.Speed.x;
            }
            else
            {
                opponent.GetComponent<AIAttackController>().stunLimit = currentAttack.Stun;
            }
        }
        else
        {
            opponent = p2Physics.gameObject;
            if (opponent.GetComponent<PlayerController>().gState == PlayerController.GroundStates.Backdash && opponent.GetComponent<PlayerController>().blocking)
            {
                opponent.GetComponent<SpriteManager>().EnableBlock();
                blocked = true;
                nextAttack = null;
                blockStun = currentAttack.Speed.x;
            }
            else
            {
                opponent.GetComponent<PlayerAttackController>().stunLimit = currentAttack.Stun;
            }
        }

        // Hitspark stuff
        if (!blocked)
        {
            Vector2 sparkPos = new Vector2(transform.position.x + (!sprite.flipX ? transform.lossyScale.x : -transform.lossyScale.x), transform.position.y);
            if (currentAttack.SparkType == HitSparkManager.SparkType.Launch)
            {
                sparkPos.y += transform.lossyScale.y * 0.5f;
            }
            hitSpark.CreateHitSpark(currentAttack.SparkType, sparkPos.x, sparkPos.y, !sprite.flipX, physics.travel, controller.pState);

            // Other stuff
            opponent.GetComponent<HealthManager>().SendDamage(currentAttack.Damage);
            GetComponent<AttackAudioManager>().PlaySound(currentAttack.SoundName, 0);
            timer = currentAttack.Speed.y;
            if (currentAttack.SoundName == "Heavy_01")
            {
                Camera.main.GetComponent<CameraControl>().StartShake(16, 2u, 0.5f);
            }
        }
        else
        {
            timer = currentAttack.Speed.y;
            if (currentAttack.SoundName == "Heavy_01")
            {
                GetComponent<AttackAudioManager>().PlaySound("BlockHeavy", 0);
                Camera.main.GetComponent<CameraControl>().StartShake(4, 2u, 1.0f);
            }
            else
            {
                GetComponent<AttackAudioManager>().PlaySound("BlockLight", 0);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (currentAttack == null || currentAttack.attackType != BaseAttack.AttackType.Throw)
        {
            return;
        }

        if (!WorldRules.PvP)
        {
            if (opponentPhysics.GetComponent<AIController>().pState == AIController.PlayerStates.Crouching)
            {
                currentAttack.RemoveRecoil();
                currentAttack.RemoveKnockback();
                return;
            }
        }
        else
        {
            if (p2Physics.GetComponent<PlayerController>().pState == PlayerController.PlayerStates.Crouching)
            {
                currentAttack.RemoveRecoil();
                currentAttack.RemoveKnockback();
                return;
            }
        }

        if (timer < currentAttack.Speed.y)
        {
            if (!WorldRules.PvP)
            {
                opponentPhysics.GetComponent<AIAttackController>().CancelAttack();
                opponentPhysics.GetComponent<AIAttackController>().stunLimit = currentAttack.Stun;

                opponentPhysics.travel = currentAttack.Knockback.x / WorldRules.physicsRate;
                opponentPhysics.launch = currentAttack.Knockback.y / WorldRules.physicsRate;
            }
            else
            {
                p2Physics.GetComponent<PlayerAttackController>().CancelAttack();
                p2Physics.GetComponent<PlayerAttackController>().stunLimit = currentAttack.Stun;

                p2Physics.travel = currentAttack.Knockback.x / WorldRules.physicsRate;
                p2Physics.launch = currentAttack.Knockback.y / WorldRules.physicsRate;
            }
            currentAttack.RemoveKnockback();

            if (!currentAttack.AlwaysRecoil && !currentAttack.DelayRecoil)
            {
                physics.travel -= currentAttack.Recoil.x / WorldRules.physicsRate;
                physics.launch -= currentAttack.Recoil.y / WorldRules.physicsRate;
                currentAttack.RemoveRecoil();
            }
        }
    }

    public void SwapOpponentType()
    {
        if (!WorldRules.PvP)
        {
            opponentPhysics = physics.opponent.GetComponent<AIPhysics>();
            p2Physics = null;
        }
        else
        {
            p2Physics = physics.opponent.GetComponent<PlayerPhysics>();
            opponentPhysics = null;
        }
    }

    public void CancelAttack()
    {
        currentAttack = null;
        hitbox.enabled = false;
        timer = 0;
        state = AttackState.Empty;
    }
}
