using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    PlayerController controller;
    Animator animator;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.pState == PlayerController.PlayerStates.Grounded)
        {
            animator.SetInteger("YDir", 0);
        }
        else if (controller.pState == PlayerController.PlayerStates.Airborne)
        {
            if (controller.aState == PlayerController.AirStates.Rising)
            {
                animator.SetInteger("YDir", 1);
            }
            if (controller.aState == PlayerController.AirStates.Falling)
            {
                animator.SetInteger("YDir", -1);
            }
        }

        sprite.flipX = controller.currentSide == PlayerController.Side.Right ? true : false;
    }
}
