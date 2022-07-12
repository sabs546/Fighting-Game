using UnityEngine;

public class AIPhysics : MonoBehaviour
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

    private AIController controller;
    public  GameObject   opponent;

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
        controller = GetComponent<AIController>();
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
        pos = transform.position;                                                                                          // We can apply all the forces to this first
        effectiveGravity += fTimeGravity;                                                                                  // Gravity is always applied
        if (controller.gState == AIController.GroundStates.Dash ||
            controller.gState == AIController.GroundStates.Backdash) launch *= 0.75f;
        effectiveGravity -= launch;                                                                                        // Launch applies next for no real reason
        if (launch == controller.jumpPower)
        {
            source.clip = jumpWind;
            source.Play();
        }
        launch = 0.0f;                                                                                                     // External forces like jumping are nullified
        
        controller.aState = effectiveGravity < 0.0f ? AIController.AirStates.Rising : AIController.AirStates.Falling;      // Check if you're rising or falling

        pos.y -= effectiveGravity * WorldRules.gameSpeed;                                                                  // Gravity acting
        // Hitting the floor
        if (pos.y - effectiveGravity < effectiveMinHeight)
        {
            pos.y = effectiveMinHeight;
            effectiveGravity = 0.0f;
            if (controller.pState != AIController.PlayerStates.Crouching)
            {
                controller.pState = AIController.PlayerStates.Grounded;
            }
        }

        // --------------------------------------------------------
        // - Vertical Movement -
        // -------
        if (effectiveGravity != 0.0f)
        {
            controller.pState = AIController.PlayerStates.Airborne;
        }

        // --------------------------------------------------------
        // - Horizontal Movement -
        // -------
        if (controller.pState == AIController.PlayerStates.Grounded || controller.pState == AIController.PlayerStates.Crouching)
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
        else if (controller.pState == AIController.PlayerStates.Airborne)
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
            controller.gState = AIController.GroundStates.Sprint;
            effectiveMovement = travel;
        }

        // --------------------------------------------------------
        // - Dashing -
        // -------
        if (controller.gState != AIController.GroundStates.Sprint)
        {
            effectiveMovement += travel;
            if (travel == controller.dashDistance || travel == -controller.dashDistance)
            {
                source.clip = dashWind;
                source.Play();
            }
            travel = 0.0f;
        }

        if (controller.gState != AIController.GroundStates.Stun)
        {
            // If you're moving away from the opponent
            if ((opponent.transform.position.x > pos.x && effectiveMovement < 0.0f) ||
                (opponent.transform.position.x < pos.x && effectiveMovement > 0.0f))
            {
                if (controller.gState != AIController.GroundStates.Sprint && controller.gState != AIController.GroundStates.Dash)
                {
                    controller.gState = AIController.GroundStates.Backdash;
                }
                enableSprint = true;
            }

            // If you're toward from the opponent
            if ((opponent.transform.position.x > pos.x && effectiveMovement > 0.0f) ||
                (opponent.transform.position.x < pos.x && effectiveMovement < 0.0f))
            {
                if (controller.gState != AIController.GroundStates.Sprint)
                {
                    controller.gState = AIController.GroundStates.Dash;
                    if (enableCrouch)
                    {
                        controller.pState = AIController.PlayerStates.Crouching;
                        GetComponent<BoxCollider2D>().enabled = false;
                    }
                }
                enableSprint = true;
            }

            // Come to a stop
            if (effectiveMovement < 0.01f && effectiveMovement > -0.01f)
            {
                if (controller.gState != AIController.GroundStates.Stun)
                {
                    controller.gState = AIController.GroundStates.Neutral;
                }
                effectiveMovement = 0.0f;
                enableSprint = false;
                enableCrouch = false;
                airLock = 0;
                GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        if (controller.gState != AIController.GroundStates.Backdash)
        {
            controller.DisableBlock();
        }

        // To stop crouching from locking the AI up
        if (controller.gState == AIController.GroundStates.Neutral && controller.pState == AIController.PlayerStates.Crouching)
        {
            controller.pState = AIController.PlayerStates.Grounded;
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
        if (controller.gState != AIController.GroundStates.Sprint)
        {
            controller.currentSide = pos.x < opponent.transform.position.x ? AIController.Side.Left : AIController.Side.Right;
        }
        else
        {
                 if (effectiveMovement < 0) { controller.currentSide = AIController.Side.Right; }
            else if (effectiveMovement > 0) { controller.currentSide = AIController.Side.Left; }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        opponent.GetComponent<PlayerPhysics>().travel += effectiveMovement;
        startSprint = false;
    }

    public void Brake()
    {
        effectiveMovement = 0.0f;
    }
}
