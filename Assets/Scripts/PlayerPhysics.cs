using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    // Positional Values ======================================================
    private Vector3 pos;              // Character height from the floor
    private int     physicsRate;      // The FPS that the logic should flow at
    private float   fTimeGravity;     // Gravity locked to 60fps
    private float   fTimeDrag;        // Drag locked to 60fps
    private float   fTimeFloorDrag;   // FloorDrag locked to 60fps

    // Controlled Values ======================================================
    [HideInInspector]
    public float launch;              // When extra Y force is applied
    [HideInInspector]
    public float travel;              // When extra X force is applied

    // Calculated Values ======================================================
    private float effectiveGravity;   // The gravity after forces are applied
    private float effectiveMovement;  // X-Axis Movement decay
    private float effectiveMinHeight; // The minHeight after removing the feet
    private float effectiveMaxHeight; // The maxHeight after removing the head

    private PlayerController controller;
    public  GameObject       opponent;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        physicsRate = 60;
        launch = 0.0f;
        travel = 0.0f;

        fTimeGravity = WorldRules.gravity / physicsRate;
        fTimeDrag = WorldRules.drag / physicsRate;
        fTimeFloorDrag = WorldRules.floordrag / physicsRate;

        float halfHeight = transform.localScale.y * 0.5f;
        effectiveMinHeight = WorldRules.minHeight + halfHeight;
        effectiveMaxHeight = WorldRules.maxHeight - halfHeight;
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;                                                                                                  // We can apply all the forces to this first
        effectiveGravity += fTimeGravity;                                                                                          // Gravity is always applied
        effectiveGravity -= launch;                                                                                                // Launch applies next for no real reason
        launch = 0.0f;                                                                                                             // External forces like jumping are nullified
        
        controller.aState = effectiveGravity < 0.0f ? PlayerController.AirStates.Rising : PlayerController.AirStates.Falling;      // Check if you're rising or falling

        // --------------------------------------------------------
        // - Vertical Movement -
        // -------
        if (effectiveGravity != 0.0f)
        {
            controller.pState = PlayerController.PlayerStates.Airborne;
        }

        pos.y -= effectiveGravity;                                                                                                 // Gravity acting
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
            }
            // Moving right
            else if (effectiveMovement > 0.0f)
            {
                effectiveMovement -= fTimeFloorDrag;
            }
        }
        else if (controller.pState == PlayerController.PlayerStates.Airborne)
        {
            // Moving left
            if (effectiveMovement < 0.0f)
            {
                effectiveMovement += fTimeDrag;
            }
            // Moving right
            else if (effectiveMovement > 0.0f)
            {
                effectiveMovement -= fTimeDrag;
            }
        }

        // Come to a stop
        if (effectiveMovement < 0.01f && effectiveMovement > -0.01f)
        {
            controller.gState = PlayerController.GroundStates.Neutral;
            effectiveMovement = 0.0f;
        }

        effectiveMovement += travel;
        travel = 0.0f;
        pos.x += effectiveMovement;

        // If your opponent is to the right
        if (opponent.transform.position.x > pos.x)
        {
            if (effectiveMovement < 0.0f)
            {
                controller.gState = PlayerController.GroundStates.Backdash;
            }
        }

        // If your opponent is to the left
        if (opponent.transform.position.x < pos.x)
        {
            if (effectiveMovement > 0.0f)
            {
                controller.gState = PlayerController.GroundStates.Dash;
            }
        }

        transform.position = pos;
    }
}
