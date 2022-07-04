using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeGame : MonoBehaviour
{
    [SerializeField]
    private GameObject p1;
    private PlayerAttackController p1AtkController;
    private PlayerPhysics p1Physics;
    private Animator p1Anim;
    [SerializeField]
    private GameObject p2;
    private PlayerAttackController p2AtkController;
    private PlayerPhysics p2Physics;
    private Animator p2Anim;
    [SerializeField]
    private GameObject CPU;
    private AIAttackController CPUAtkController;
    private AIPhysics CPUPhysics;
    private Animator CPUAnim;

    // Start is called before the first frame update
    void Awake()
    {
        p1AtkController = p1.GetComponent<PlayerAttackController>();
        p1Physics = p1.GetComponent<PlayerPhysics>();
        p1Anim = p1.GetComponent<Animator>();
        
        p2AtkController = p2.GetComponent<PlayerAttackController>();
        p2Physics = p2.GetComponent<PlayerPhysics>();
        p2Anim = p2.GetComponent<Animator>();
        
        CPUAtkController = CPU.GetComponent<AIAttackController>();
        CPUPhysics = CPU.GetComponent<AIPhysics>();
        CPUAnim = CPU.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        p1AtkController.enabled = false;
        p1Physics.enabled = false;
        p1Anim.enabled = false;

        if (WorldRules.PvP)
        {
            p2AtkController.enabled = false;
            p2Physics.enabled = false;
            p2Anim.enabled = false;
        }
        else
        {
            CPUAtkController.enabled = false;
            CPUPhysics.enabled = false;
            CPUAnim.enabled = false;
        }
    }

    private void OnDisable()
    {
        if (p1 == null) return;
        p1AtkController.enabled = true;
        p1Physics.enabled = true;
        p1Anim.enabled = true;

        if (WorldRules.PvP)
        {
            p2AtkController.enabled = true;
            p2Physics.enabled = true;
            p2Anim.enabled = true;
        }
        else
        {
            CPUAtkController.enabled = true;
            CPUPhysics.enabled = true;
            CPUAnim.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
