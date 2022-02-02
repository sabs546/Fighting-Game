﻿using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;
    private Vector2 p1Pos;
    private Vector2 p2Pos;
    private Vector3 cameraPos;

    private Camera cam;
    public enum CameraState { Close, Normal, Far };
    public CameraState state;

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
            if (cam.orthographicSize < normal.zoom)
            {
                cam.orthographicSize += growSpeed / WorldRules.physicsRate;
                cam.transform.localScale += new Vector3(zoomSpeed / WorldRules.physicsRate, zoomSpeed / WorldRules.physicsRate, 0.0f);
            }
            if (cam.transform.position.y < normal.height)
            {
                cameraPos.y += growSpeed / WorldRules.physicsRate;
            }
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

        transform.position = cameraPos;
    }
}

[Serializable]
public struct CameraSetting
{
    public float distance;
    public float height;
    public float zoom;
}