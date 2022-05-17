using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStateControl : MonoBehaviour
{
    public enum GameState { Menu, Fighting, GameOver };
    public GameState gameState { get; private set; }

    public GameObject p1;
    public GameObject p2;
    public GameObject CPU;

    private Vector2 p1StartPos;
    private Vector2 p2StartPos;

    public GameObject menuUI;
    public GameObject fightUI;
    public GameObject gameOverUI;

    public TextMeshProUGUI winnerTag;
    private string winnerName;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Menu;
        p1StartPos = p1.transform.position;
        p2StartPos = CPU.transform.position;
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
                gameOverUI.SetActive(false);
                menuUI.SetActive(true);
                p1.transform.position = p1StartPos;
                p1.GetComponent<HealthManager>().ResetHealth();
                p1.GetComponent<Animator>().SetTrigger("Revive");
                p1.GetComponent<PlayerPhysics>().enabled = false;
                if (WorldRules.PvP)
                {
                    p2.transform.position = p2StartPos;
                    p2.GetComponent<HealthManager>().ResetHealth();
                    p2.GetComponent<Animator>().SetTrigger("Revive");
                    p2.GetComponent<PlayerPhysics>().enabled = false;
                }
                else
                {
                    CPU.transform.position = p2StartPos;
                    CPU.GetComponent<HealthManager>().ResetHealth();
                    CPU.GetComponent<Animator>().SetTrigger("Revive");
                    CPU.GetComponent<AIPhysics>().enabled = false;
                }
                break;
            case GameState.Fighting:
                gameState = GameState.Fighting;
                menuUI.SetActive(false);
                fightUI.SetActive(true);
                break;
            case GameState.GameOver:
                gameState = GameState.GameOver;
                fightUI.SetActive(false);
                gameOverUI.SetActive(true);

                if (CheckDead(p1))
                {
                    if (WorldRules.PvP)
                    {
                        winnerName = p2.GetComponent<HealthManager>().nameTag.text;
                        p2.GetComponent<PlayerController>().enabled = false;
                    }
                    else
                    {
                        winnerName = CPU.GetComponent<HealthManager>().nameTag.text;
                        CPU.GetComponent<AIController>().enabled = false;
                    }
                }
                winnerTag.SetText(winnerName);

                p1.GetComponent<PlayerController>().enabled = false;
                break;
        }
    }

    private bool CheckDead(GameObject fighter)
    {
        HealthManager healthManager = fighter.GetComponent<HealthManager>();
        if (healthManager.currentHealth == 0)
        {
            return true;
        }
        winnerName = healthManager.nameTag.text;
        return false;
    }

    public void TriggerMenu()
    {
        SetGameState(GameState.Menu);
    }
}
