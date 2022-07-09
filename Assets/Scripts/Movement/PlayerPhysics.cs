using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    // Positional Values ===============================================
    private Vector3 pos;              // Character height from the floor
    private float   fTimeGravity;     // Gravity locked to 60fps
    private float   fTimeDrag;        // Drag locked to 60fps
    private float   fTimeFloorDrag;   // FloorDrag locked to 60fps

    // Controlled Values ======================================================
    [HideInInspector]
    public float launch;              // When extra Y force is applied
    [HideInInspector]
    public float travel;              // When extra X force is applied
    [HideInInspector]
    public bool  enableSprint;        // Check for dashes rather than sprinting
    [HideInInspector]
    public bool  enableCrouch;        // Check for a crouch input
    [HideInInspector]
    public bool  startSprint;         // Check for dashes rather than sprinting
    [HideInInspector]
    public int   airLock;             // Can't change direction in midair

    // Calculated Values ===============================================================
    private float effectiveGravity;   // The gravity after forces are applied
    private float effectiveMovement;  // X-Axis Movement decay
    private float effectiveMinHeight; // The minHeight after removing the feet
    private float effectiveMaxHeight; // The maxHeight after removing the head
    private float effectiveMaxLeft;   // The maxWidth after adding the half the player
    private float effectiveMaxRight;  // The maxWidth after removing the half the player

    private PlayerController controller;
    public  GameObject       opponent;

    [Header("Audio")]
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioClip dashWind;
    [SerializeField]
    private AudioClip jumpWind;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GetComponent<PlayerController>();
        fTimeGravity = WorldRules.gravity / WorldRules.physicsRate;
        fTimeDrag = WorldRules.drag / WorldRules.physicsRate;
        fTimeFloorDrag = WorldRules.floordrag / WorldRules.physicsRate;

        float halfHeight = transform.lossyScale.y * 0.5f;
        float halfWidth = transform.lossyScale.x * 0.5f;

        effectiveMinHeight = WorldRules.minHeight + halfHeight;
        effectiveMaxHeight = WorldRules.maxHeight - halfHeight;
        effectiveMaxLeft = -WorldRules.maxWidth + halfWidth;
        effectiveMaxRight = WorldRules.maxWidth - halfWidth;
    }

    void OnDisable()
    {
        startSprint = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = transform.position;                                                                                                  // We can apply all the forces to this first
        effectiveGravity += fTimeGravity;                                                                                          // Gravity is always applied
        if (controller.gState == PlayerController.GroundStates.Dash ||
            controller.gState == PlayerController.GroundStates.Backdash) launch *= 0.75f;
        effectiveGravity -= launch;                                                                                                // Launch applies next for no real reason
        if (launch == controller.jumpPower)
        {
            source.clip = jumpWind;
            source.Play();
        }
        launch = 0.0f;                                                                                                             // External forces like jumping are nullified
        
        controller.aState = effectiveGravity < 0.0f ? PlayerController.AirStates.Rising : PlayerController.AirStates.Falling;      // Check if you're rising or falling

        // --------------------------------------------------------
        // - Vertical Movement -
        // -------
        if (effectiveGravity != 0.0f)
        {
            controller.pState = PlayerController.PlayerStates.Airborne;
        }

        pos.y -= effectiveGravity * WorldRules.gameSpeed;                                                                          // Gravity acting
        // Hitting the floor
        if (pos.y - effectiveGravity < effectiveMinHeight)
        {
            pos.y = effectiveMinHeight;
            effectiveGravity = 0.0f;
            controller.pState = PlayerController.PlayerStates.Grounded;
        }

        // --------------------------------------------------------
        // - Horizontal Movement -
        // -------
        if (controller.pState == PlayerController.PlayerStates.Grounded)
        {
            // Moving left
            if (effectiveMovement < 0.0f)
            {
                effectiveMovement += fTimeFloorDrag;
                airLock = -1;
            }
            // Moving right
            else if (effectiveMovement > 0.0f)
            {
                effectiveMovement -= fTimeFloorDrag;
                airLock = 1;
            }
        }
        else if (controller.pState == PlayerController.PlayerStates.Airborne)
        {
            // Moving left
            if (effectiveMovement < 0.0f)
            {
                effectiveMovement += fTimeDrag;
                airLock = -1;
            }
            // Moving right
            else if (effectiveMovement > 0.0f)
            {
                effectiveMovement -= fTimeDrag;
                airLock = 1;
            }
        }

        // --------------------------------------------------------
        // - Sprinting -
        // -------
        if (enableSprint && startSprint)
        {
            controller.gState = PlayerController.GroundStates.Sprint;
            effectiveMovement = travel;
        }

        // --------------------------------------------------------
        // - Dashing -
        // -------
        if (controller.gState != PlayerController.GroundStates.Sprint)
        {
            effectiveMovement += travel;
            travel = 0.0f;
        }

        if (controller.gState != PlayerController.GroundStates.Stun)
        {
            // If you're moving away from the opponent
            if ((opponent.transform.position.x > pos.x && effectiveMovement < 0.0f) ||
                (opponent.transform.position.x < pos.x && effectiveMovement > 0.0f))
            {
                if (controller.gState != PlayerController.GroundStates.Sprint && controller.gState != PlayerController.GroundStates.Dash)
                {
                    if (controller.gState != PlayerController.GroundStates.Backdash && controller.gState != PlayerController.GroundStates.Dash)
                    {
                        source.clip = dashWind;
                        source.Play();
                    }
                    controller.gState = PlayerController.GroundStates.Backdash;
                }
                enableSprint = true;
            }

            // If you're toward from the opponent
            if ((opponent.transform.position.x > pos.x && effectiveMovement > 0.0f) ||
                (opponent.transform.position.x < pos.x && effectiveMovement < 0.0f))
            {
                if (controller.gState != PlayerController.GroundStates.Sprint)
                {
                    if (controller.gState != PlayerController.GroundStates.Backdash && controller.gState != PlayerController.GroundStates.Dash)
                    {
                        source.clip = dashWind;
                        source.Play();
                    }
                    controller.gState = PlayerController.GroundStates.Dash;
                    if (enableCrouch) controller.pState = PlayerController.PlayerStates.Crouching;
                }
                enableSprint = true;
                GetComponent<Animator>().SetBool("Guard", false);
            }

            // Come to a stop
            if (effectiveMovement < 0.01f && effectiveMovement > -0.01f)
            {
                if (controller.gState != PlayerController.GroundStates.Stun)
                {
                    controller.gState = PlayerController.GroundStates.Neutral;
                }
                effectiveMovement = 0.0f;
                enableSprint = false;
                enableCrouch = false;
                airLock = 0;
                GetComponent<Animator>().SetBool("Guard", false);
            }

            if (controller.gState != PlayerController.GroundStates.Backdash)
            {
                controller.blocking = false;
            }
        }

        // --------------------------------------------------------
        // - Collision -
        // -------
        pos.x += effectiveMovement * WorldRules.gameSpeed;

        // --------------------------------------------------------
        // - Walls -
        // -------
        if (pos.x < effectiveMaxLeft)
        {
            pos.x = effectiveMaxLeft;
            effectiveMovement = -effectiveMovement;
            effectiveGravity = -controller.jumpPower;
            startSprint = false;
            source.clip = jumpWind;
            source.Play();
        }
        else if (pos.x > effectiveMaxRight)
        {
            pos.x = effectiveMaxRight;
            effectiveMovement = -effectiveMovement;
            effectiveGravity = -controller.jumpPower;
            startSprint = false;
            source.clip = jumpWind;
            source.Play();
        }

        transform.position = pos;
        if (controller.gState != PlayerController.GroundStates.Sprint)
        {
            controller.currentSide = pos.x < opponent.transform.position.x ? PlayerController.Side.Left : PlayerController.Side.Right;
        }
        else
        {
                 if (effectiveMovement < 0) { controller.currentSide = PlayerController.Side.Right; }
            else if (effectiveMovement > 0) { controller.currentSide = PlayerController.Side.Left; }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!WorldRules.PvP) opponent.GetComponent<AIPhysics>().travel += effectiveMovement;
        else                 opponent.GetComponent<PlayerPhysics>().travel += effectiveMovement;
        startSprint = false;
    }

    public void Brake()
    {
        effectiveMovement = 0.0f;
    }

    public void SlowDown()
    {
        effectiveMovement *= 0.5f;
    }

    public void Hover()
    {
        effectiveGravity *= 0.5f;
    }

    public void SetOpponentType(GameObject type)
    {
        opponent = type;
    }
}
