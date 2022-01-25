using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float dashDistance;
    public float jumpPower;

    public enum PlayerStates { Crouching, Grounded, Airborne };
    public enum GroundStates { Neutral, Dash, Backdash, Sprint };
    public enum AirStates    { Rising, Falling };
    public PlayerStates pState;
    public GroundStates gState;
    public AirStates    aState;

    private SetControls controls;
    private PlayerPhysics physics;

    // Start is called before the first frame update
    void Start()
    {
        controls = GetComponent<SetControls>();
        physics = GetComponent<PlayerPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        // --------------------------------------------------------
        // - Movement Inputs -
        // -------
        if (Input.GetKeyDown(controls.Up) && pState == PlayerStates.Grounded)
        {
            physics.launch += jumpPower;
        }
        if (Input.GetKeyDown(controls.Down))
        {

        }
        if (Input.GetKeyDown(controls.Left) && pState == PlayerStates.Grounded && gState == GroundStates.Neutral)
        {
            physics.travel -= dashDistance;
        }
        if (Input.GetKeyDown(controls.Right) && pState == PlayerStates.Grounded && gState == GroundStates.Neutral)
        {
            physics.travel += dashDistance;
        }
    }
}
