using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterEscape : MonoBehaviour
{
    private GameObject critterType;
    public float xSpeed { private get; set; }
    public float ySpeed { private get; set; }
    public float lifeTime { private get; set; }
    public float delay { private get; set; }
    
    private void Update()
    {
        if (delay <= 0.0f)
        {
            lifeTime -= Time.deltaTime;
            transform.position = new Vector2(transform.position.x + (xSpeed * Time.deltaTime), transform.position.y + (ySpeed * Time.deltaTime));
            if (lifeTime <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            delay -= Time.deltaTime;
        }
    }
}
