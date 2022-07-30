using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushDisturbance : MonoBehaviour
{
    private CameraControl cameraControl;
    private Transform sound;
    [SerializeField]
    private int bushID;
    [SerializeField]
    private float triggerRange;
    private int triggerSide;
    [SerializeField]
    private FoxEscape fox;
    [SerializeField]
    private Transform parentTransform;

    // Start is called before the first frame update
    void Start()
    {
        triggerSide = 0;
        cameraControl = Camera.main.GetComponent<CameraControl>();
        sound = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (cameraControl.startShaking && fox.currentBush == bushID && fox.state == FoxEscape.State.Resting)
        {
            triggerSide = CheckInRange();
            if (triggerSide == -1) fox.DisturbLeft();
            else if (triggerSide == 1) fox.DisturbRight();
        }
    }

    public int CheckInRange()
    {
        float gPos = transform.TransformPoint(Vector3.zero).x;
        if (sound.position.x > gPos && sound.position.x < gPos + triggerRange)
        {
            return 1;
        }
        else if (sound.position.x < gPos && sound.position.x > gPos - triggerRange)
        {
            return -1;
        }
        return 0;
    }
}
