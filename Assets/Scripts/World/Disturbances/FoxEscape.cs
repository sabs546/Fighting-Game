using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxEscape : MonoBehaviour
{
    public enum State { Resting, DisturbedLeft, DisturbedRight };
    public State state;
    
    public int currentBush;
    [SerializeField]
    private int targetBush;
    [SerializeField]
    private BushDisturbance[] availableBushes;

    [SerializeField]
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Resting;
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.Resting)
        {
            if (state == State.DisturbedLeft)
            {
                if (targetBush > currentBush && transform.position.x < availableBushes[targetBush].transform.position.x)
                {
                    transform.position = new Vector2(transform.position.x + (speed * Time.deltaTime), transform.position.y);
                }
                else
                {
                    state = State.Resting;
                    currentBush = targetBush;
                    HideAway();
                }
            }
            if (state == State.DisturbedRight)
            {
                if (targetBush < currentBush && transform.position.x > availableBushes[targetBush].transform.position.x)
                {
                    transform.position = new Vector2(transform.position.x - (speed * Time.deltaTime), transform.position.y);
                }
                else
                {
                    state = State.Resting;
                    currentBush = targetBush;
                    HideAway();
                }
            }
        }
    }

    public void DisturbLeft()
    {
        state = State.DisturbedLeft;
        targetBush = currentBush + 1;
        GetComponent<SpriteRenderer>().flipX = true;
        GetComponent<Animator>().speed = 1.0f;
        transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);
        if (targetBush > availableBushes.Length - 1)
        {
            state = State.DisturbedRight;
            targetBush = currentBush - 1;
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public void DisturbRight()
    {
        state = State.DisturbedRight;
        targetBush = currentBush - 1;
        GetComponent<SpriteRenderer>().flipX = false;
        GetComponent<Animator>().speed = 1.0f;
        transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);
        if (targetBush < 0)
        {
            state = State.DisturbedLeft;
            targetBush = currentBush + 1;
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void HideAway()
    {
        GetComponent<Animator>().speed = 0.0f;
        transform.localScale = Vector3.zero;
    }
}
