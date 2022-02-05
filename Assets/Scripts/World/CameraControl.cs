using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;
    public GameObject menuScreen;
    private Vector2 p1Pos;
    private Vector2 p2Pos;
    private Vector3 cameraPos;

    private Camera cam;
    public enum CameraState { Menu, Close, Normal, Far };
    public CameraState state;

    public CameraSetting menu;
    public CameraSetting close;
    public CameraSetting normal;
    public CameraSetting far;

    private float centreDistanceX;
    private float xDistance;
    private float yDistance;
    public float growSpeed;
    public float zoomSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cameraPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        p1Pos = p1.transform.position;
        p2Pos = p2.transform.position;

        centreDistanceX = p1Pos.x + p2Pos.x;
        xDistance = Math.Abs(p1Pos.x - p2Pos.x);
        yDistance = p1Pos.y + p2Pos.y;

        cameraPos.x = centreDistanceX / 2;

        if (state != CameraState.Menu)
        {
            if (xDistance < close.distance)
            {
                state = CameraState.Close;
            }
            else if (xDistance > far.distance)
            {
                state = CameraState.Far;
            }
            else
            {
                state = CameraState.Normal;
            }

            if (state == CameraState.Close)
            {
                if (cam.orthographicSize > close.zoom)
                {
                    cam.orthographicSize -= growSpeed / WorldRules.physicsRate;
                    cam.transform.localScale -= new Vector3(zoomSpeed / WorldRules.physicsRate, zoomSpeed / WorldRules.physicsRate, 0.0f);
                }
                else if (cam.orthographicSize < close.zoom)
                {
                    cam.orthographicSize += growSpeed / WorldRules.physicsRate;
                    cam.transform.localScale += new Vector3(zoomSpeed / WorldRules.physicsRate, zoomSpeed / WorldRules.physicsRate, 0.0f);
                    menuScreen.SetActive(false);
                }
                else if (!p1.GetComponent<PlayerController>().enabled)
                {
                    // When the camera stops moving down you gain control, it looks dumb but just bear with me it's a simple game
                    p1.GetComponent<PlayerPhysics>().enabled = true;
                    p1.GetComponent<PlayerController>().enabled = true;
                    p1.GetComponent<SpriteManager>().enabled = true;
                    p2.GetComponent<PlayerPhysics>().enabled = true;
                    p2.GetComponent<PlayerController>().enabled = true;
                    p2.GetComponent<SpriteManager>().enabled = true;
                }

                if (cam.transform.position.y > close.height)
                {
                    cameraPos.y -= growSpeed / WorldRules.physicsRate;
                }
            }
            else if (state == CameraState.Far)
            {
                if (cam.orthographicSize < far.zoom)
                {
                    cam.orthographicSize += growSpeed / WorldRules.physicsRate;
                    cam.transform.localScale += new Vector3(zoomSpeed / WorldRules.physicsRate, zoomSpeed / WorldRules.physicsRate, 0.0f);
                }
                if (cam.transform.position.y < far.height)
                {
                    cameraPos.y += growSpeed / WorldRules.physicsRate;
                }
            }
            else
            {
                // From close
                if (cam.orthographicSize < normal.zoom)
                {
                    cam.orthographicSize += growSpeed / WorldRules.physicsRate;
                    cam.transform.localScale += new Vector3(zoomSpeed / WorldRules.physicsRate, zoomSpeed / WorldRules.physicsRate, 0.0f);
                }
                if (cam.transform.position.y < normal.height)
                {
                    cameraPos.y += growSpeed / WorldRules.physicsRate;
                }

                // From far
                if (cam.orthographicSize > normal.zoom)
                {
                    cam.orthographicSize -= growSpeed / WorldRules.physicsRate;
                    cam.transform.localScale -= new Vector3(zoomSpeed / WorldRules.physicsRate, zoomSpeed / WorldRules.physicsRate, 0.0f);
                }
                if (cam.transform.position.y > normal.height)
                {
                    cameraPos.y -= growSpeed / WorldRules.physicsRate;
                }
            }

            // Camera locking
            switch (state)
            {
                case CameraState.Close:
                    if (cameraPos.y > close.height - 0.01f && cameraPos.y < close.height + 0.01f) cameraPos.y = close.height;
                    if (cam.orthographicSize > close.zoom - 0.01f && cam.orthographicSize < close.zoom + 0.01f) cam.orthographicSize = close.zoom;
                    break;
                case CameraState.Normal:
                    if (cameraPos.y > normal.height - 0.01f && cameraPos.y < normal.height + 0.01f) cameraPos.y = normal.height;
                    if (cam.orthographicSize > normal.zoom - 0.01f && cam.orthographicSize < normal.zoom + 0.01f) cam.orthographicSize = normal.zoom;
                    break;
                case CameraState.Far:
                    if (cameraPos.y > far.height - 0.01f && cameraPos.y < far.height + 0.01f) cameraPos.y = far.height;
                    if (cam.orthographicSize > far.zoom - 0.01f && cam.orthographicSize < far.zoom + 0.01f) cam.orthographicSize = far.zoom;
                    break;
            }
        }
        transform.position = cameraPos;
    }

    public void StartGame()
    {
        state = CameraState.Normal;
    }
}

[Serializable]
public struct CameraSetting
{
    public float distance;
    public float height;
    public float zoom;
}