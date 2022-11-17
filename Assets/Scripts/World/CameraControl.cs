using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // UI Stuff ================================================
    public GameObject menuScreen;    // To enable/disable
    public GameObject fightUI;       // To enable/disable
    [SerializeField]
    private ClockControl roundClock; // To start the round timer

    // Positioning ====================================================================
    public GameObject p1;      // Needed to calculate distance
    public GameObject p2;
    public GameObject cpu;
    private Vector2 p1Pos;     // Saves time when getting position
    private Vector2 p2Pos;
    private Vector3 cameraPos; // Alter these values then apply them to the camera once
    private Vector3 shakePos;  // Temp pos used when shaking so it can return to normal
    public bool startShaking { get; private set; } // For other scripts to know

    // States ===========================================================
    public enum CameraState { Menu, Close, Normal, Far };
    public CameraState state; // Keeps track of where the camera is going
    private Camera cam;       // To access orthographicSize
    private GameStateControl gameStateControl;
    private int startDelay;   // Online start delay

    // State Value Storage ===================================
    public CameraSetting menu;   // Store menu screen values
    public CameraSetting close;  // When you're at the closest
    public CameraSetting normal; // Everywhere else
    public CameraSetting far;    // How did you get here?
    public float growSpeed;      // Background resize speed
    public float zoomSpeed;      // Camera zoom speed

    // Calculated Values ========================================
    private float centreDistanceX; // To follow left/right
    private float xDistance;       // Player to player X distance
    private float yDistance;       // Player to player Y distance
    private uint  ticker;          // Timer used to count frames for the camera shake
    private uint  shakeSpeed;      // How fast the camera shakes (Less is faster)
    private float shakePower;      // How far the camera shakes

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        gameStateControl = GetComponent<GameStateControl>();
        cameraPos = transform.position;
        ticker = 0;
        startShaking = false;
        startDelay = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameStateControl.gameState == GameStateControl.GameState.Pause && !WorldRules.online)
        {
            return;
        }

        p1Pos = p1.transform.position;
        p2Pos = WorldRules.PvP ? p2.transform.position : cpu.transform.position;

        centreDistanceX = p1Pos.x + p2Pos.x;
        xDistance = Math.Abs(p1Pos.x - p2Pos.x);
        yDistance = p1Pos.y + p2Pos.y;

        cameraPos.x = centreDistanceX / 2;

        if (state != CameraState.Menu && GameStateControl.gameState < GameStateControl.GameState.RoundStart)
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
                }
                else if (!p1.GetComponent<PlayerController>().enabled)
                {
                    // When the camera stops moving down you gain control, it looks dumb but just bear with me it's a simple game
                    p1.GetComponent<PlayerPhysics>().enabled = true;
                    p1.GetComponent<PlayerController>().enabled = true;
                    p1.GetComponent<SpriteManager>().enabled = true;
                    if (WorldRules.PvP)
                    {
                        p2.GetComponent<PlayerPhysics>().enabled = true;
                        p2.GetComponent<PlayerController>().enabled = true;
                        p2.GetComponent<SpriteManager>().enabled = true;
                    }
                    else
                    {
                        cpu.GetComponent<AIPhysics>().enabled = true;
                        cpu.GetComponent<AIController>().enabled = true;
                        cpu.GetComponent<AISpriteManager>().enabled = true;
                    }
                    roundClock.BeginTimer();
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

            // Camera locking, once you get here it has pretty much found a state and is just jittering in and out
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
        else if (GameStateControl.gameState == GameStateControl.GameState.Menu)
        {
            if (cam.orthographicSize != menu.zoom || cameraPos.y != menu.height)
            {
                if (cam.orthographicSize > menu.zoom)
                {
                    cam.orthographicSize -= growSpeed / WorldRules.physicsRate;
                    cam.transform.localScale -= new Vector3(zoomSpeed / WorldRules.physicsRate, zoomSpeed / WorldRules.physicsRate, 0.0f);
                }
                else if (cam.orthographicSize < menu.zoom)
                {
                    cam.orthographicSize += growSpeed / WorldRules.physicsRate;
                    cam.transform.localScale += new Vector3(zoomSpeed / WorldRules.physicsRate, zoomSpeed / WorldRules.physicsRate, 0.0f);
                }

                cameraPos.y = menu.height;
                if (cam.orthographicSize > menu.zoom - 0.01f && cam.orthographicSize < menu.zoom + 0.01f)
                {
                    MenuController menuController = menuScreen.GetComponent<MenuController>();
                    if (menuController.currentPage == 0) menuController.menuBlock.interactable = true;
                    cam.orthographicSize = menu.zoom;
                }
            }

            if (startDelay > 0)
            {
                startDelay--;
            }
            else if (startDelay == 0)
            {
                startDelay = -1;
                StartGame();
            }
        }
        transform.position = cameraPos;

        if (ticker > 0)
        {
            ticker--;
            if (ticker % shakeSpeed != 0)
            {
                shakePos = cameraPos;
                shakePos.x = UnityEngine.Random.Range(cameraPos.x - shakePower, cameraPos.x + shakePower);
                shakePos.y = UnityEngine.Random.Range(cameraPos.y - shakePower, cameraPos.y + shakePower);
                shakePower *= 0.9f; // Slowly null it down to the centre
                transform.position = shakePos;
            }
            else
            {
                startShaking = false;
            }
        }
    }

    public void StartGame()
    {
        state = CameraState.Normal;
        gameStateControl.SetGameState(GameStateControl.GameState.Fighting);
    }

    public void StartGame(int delay)
    {
        startDelay = delay;
    }

    public void ResetToMenu()
    {
        state = CameraState.Menu;
    }

    public void StartShake(uint length, uint speed, float power)
    {
        ticker = length;
        shakeSpeed = speed;
        shakePower = power;
        startShaking = true;
    }
}

[Serializable]
public struct CameraSetting
{
    public float distance;
    public float height;
    public float zoom;
}