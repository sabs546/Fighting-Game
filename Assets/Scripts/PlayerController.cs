using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpPower;

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
        if (Input.GetKeyDown(controls.Up))
        {
            physics.launch += 10.0f;
        }
    }
}
