using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    private Vector3 pos;              // Character height from the floor
    [HideInInspector]
    public float launch;              // When extra Y force is applied

    private float effectiveGravity;   // The gravity after forces are applied
    private float effectiveMinHeight; // The minHeight after removing the feet
    private float effectiveMaxHeight; // The maxHeight after removing the head

    // Start is called before the first frame update
    void Start()
    {
        launch = 0.0f;
        float halfHeight = transform.localScale.y * 0.5f;
        effectiveMinHeight = WorldRules.minHeight + halfHeight;
        effectiveMaxHeight = WorldRules.maxHeight - halfHeight;
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        effectiveGravity = launch + (WorldRules.gravity * Time.deltaTime);
        if (launch > 0.0f)
        {
            launch -= WorldRules.gravity;
            if (launch - effectiveGravity < 0.0f)
            {
                launch = 0.0f;
            }
        }

        // The player falling
        if (pos.y > effectiveMinHeight)
        {
            pos = new Vector3(0.0f, pos.y - effectiveGravity);
            if (pos.y - effectiveGravity < effectiveMinHeight)
            {
                pos.y = effectiveMinHeight;
            }
        }

        transform.position = pos;
    }
}
