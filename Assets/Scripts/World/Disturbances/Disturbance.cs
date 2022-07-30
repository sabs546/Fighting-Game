using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disturbance : MonoBehaviour
{
    private CameraControl cameraControl;
    private Transform sound;
    [SerializeField]
    private GameObject critter;
    [SerializeField]
    private CritterInfo[] critters;
    [SerializeField]
    private float triggerRange;
    private bool inRange;
    private bool disturbed;

    private void Start()
    {
        disturbed = false;
        inRange = false;
        cameraControl = Camera.main.GetComponent<CameraControl>();
        sound = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (cameraControl.startShaking)
        {
            Disturb();
        }
    }

    public void Disturb()
    {
        if (CheckInRange())
        {
            foreach (CritterInfo info in critters)
            {
                CritterEscape escapee = Instantiate(critter, transform).GetComponent<CritterEscape>();
                escapee.xSpeed = info.xSpeed;
                escapee.ySpeed = info.ySpeed;
                escapee.lifeTime = info.lifeTime;
                escapee.delay = info.delay;
                if (info.xSpeed < 0.0f) escapee.GetComponent<SpriteRenderer>().flipX = true;
            }
            enabled = false;
        }
    }

    public bool CheckInRange()
    {
        if (transform.position.x > sound.position.x - triggerRange && transform.position.x < sound.position.x + triggerRange)
        {
            return true;
        }
        return false;
    }
}

[System.Serializable]
struct CritterInfo
{
    public float xSpeed;
    public float ySpeed;
    public float lifeTime;
    public float delay;
}
