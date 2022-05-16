using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateControl : MonoBehaviour
{
    public enum GameState { Menu, Fighting, GameOver };
    public GameState gameState { get; private set; }

    public GameObject P1;
    public GameObject P2;
    public GameObject CPU;

    private Vector2 P1StartPos;
    private Vector2 P2StartPos;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Menu;
        P1StartPos = P1.transform.position;
        P2StartPos = CPU.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                gameState = GameState.Menu;
                GetComponent<CameraControl>().state = CameraControl.CameraState.Menu;
                P1.transform.position = P1StartPos;
                P2.transform.position = P2StartPos;
                CPU.transform.position = P2StartPos;
                break;
            case GameState.Fighting:
                gameState = GameState.Fighting;
                break;
            case GameState.GameOver:
                gameState = GameState.GameOver;
                P1.GetComponent<PlayerController>().enabled = false;
                P2.GetComponent<PlayerController>().enabled = false;
                CPU.GetComponent<AIController>().enabled = false;
                break;
        }
    }
}
