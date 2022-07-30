using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierControl : MonoBehaviour
{
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sprite.color.a > 0.0f)
        {
            sprite.color = new Color(1.0f, 1.0f, 1.0f, sprite.color.a - Time.deltaTime);
            if (sprite.color.a < 0.0f)
            {
                sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
        }
    }
}
