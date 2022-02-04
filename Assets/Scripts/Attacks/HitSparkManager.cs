using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSparkManager : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Animator animator;
    public Camera cam;

    public enum SparkType { Null, Low, Mid, Launch };
    public SparkType Type;
    private bool active;
    private float speed;

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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
        {
            active = false;
        }
    }

    public void CreateHitSpark(SparkType sparkType, float xPos, float yPos, bool flipX, float moveSpeed)
    {
        sprite.flipX = flipX;
        active = true;
        speed = moveSpeed;
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
            animator.SetTrigger("Slide");
            transform.position = new Vector3(xPos, yPos, 0.0f);
        }
    }
}
