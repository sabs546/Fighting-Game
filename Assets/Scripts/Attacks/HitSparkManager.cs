using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSparkManager : MonoBehaviour
{
    // Scripts ======================================
    private SpriteRenderer sprite; // Spark visuals
    private Animator animator;     // Spark animation

    // Values =======================================================================
    public enum SparkType { Null, Low, Mid, Launch }; // Helps choose the animation
    public SparkType Type;                            // Holds that value
    private bool active;                              // Tells the spark when to move
    private float speed;                              // How fast the spark moves

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        active = false;
        speed = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            transform.position = new Vector3(transform.position.x + (speed / WorldRules.physicsRate), transform.position.y);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty")) // Otherwise it really would just fly off forever
        {
            active = false;
        }
    }

    public void CreateHitSpark(SparkType sparkType, float xPos, float yPos, bool flipX, float moveSpeed, PlayerController.PlayerStates state = PlayerController.PlayerStates.Grounded)
    {
        sprite.flipX = flipX;
        active = true;
        speed = moveSpeed; // Movespeed is usually how fast the player is moving at the time of the hit
        if (sparkType == SparkType.Mid)
        {
            animator.SetTrigger("Mid");
            transform.position = new Vector3(xPos, yPos, 0.0f);
        }

        if (sparkType == SparkType.Launch)
        {
            animator.SetTrigger("Launch");
            transform.position = new Vector3(xPos, yPos, 0.0f);
        }

        if (sparkType == SparkType.Low)
        {
            if (state == PlayerController.PlayerStates.Airborne)
            {
                animator.SetTrigger("Down");
                transform.position = new Vector3(xPos, yPos, 0.0f);
            }
            else
            {
                animator.SetTrigger("Slide");
                transform.position = new Vector3(xPos, yPos, 0.0f);
            }
        }
    }
}
