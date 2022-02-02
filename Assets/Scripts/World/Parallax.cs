using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public CameraControl cameraControl;
    public float parallaxStrength;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector2(cameraControl.transform.position.x / parallaxStrength, transform.position.y);
    }
}
