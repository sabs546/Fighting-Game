using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSparkManager : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Animator animator;
    public Camera cam;

    public enum SparkType { Low, Mid, Launch };
    public SparkType Type;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void CreateHitSpark(SparkType sparkType, float xPos, float yPos, bool flipX)
    {
        sprite.flipX = flipX;
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
    }
}
