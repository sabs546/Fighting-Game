using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStateControl : MonoBehaviour
{
    public enum GameState { Menu, Fighting, Pause, RoundStart, RoundOver, GameOver };
    public static GameState gameState { get; private set; }

    [Header("Fighter Related")]
    public GameObject p1;
    public GameObject p2;
    public GameObject CPU;

    private Animator p1Animator;
    private Animator p2Animator;
    private Animator CPUAnimator;

    private Vector2 p1StartPos;
    private Vector2 p2StartPos;

    [Header("Menu Related")]
    public GameObject menuUI;
    public GameObject fightUI;
    public GameObject pauseUI;
    public FreezeGame pauseSetting;
    public GameObject roundStartUI;
    public GameObject roundOverUI;
    public GameObject gameOverUI;

    [Header("Misc")]
    [SerializeField]
    private Slider menuVolumeSlider;
    [SerializeField]
    private Slider pauseVolumeSlider;

    [Header("Round Over")]
    [SerializeField]
    private TextMeshProUGUI roundWinner;
    [SerializeField]
    private TextMeshProUGUI currentScore;
    [SerializeField]
    private Image p1RoundCount;
    [SerializeField]
    private Image p2RoundCount;
    [SerializeField]
    private int roundLimit;
    private int p1Wins;
    private int p2Wins;

    [Header("Game Over")]
    public TextMeshProUGUI winnerTag;
    private string winnerName;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Menu;
        p1StartPos = p1.transform.position;
        p2StartPos = CPU.transform.position;

        p1Animator = p1.GetComponent<Animator>();
        p2Animator = p2.GetComponent<Animator>();
        CPUAnimator = CPU.GetComponent<Animator>();

        p1Wins = 0;
        p2Wins = 0;
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
                fightUI.SetActive(false);
                pauseUI.SetActive(false);
                pauseSetting.enabled = false;
                gameOverUI.SetActive(false);
                menuUI.SetActive(true);
                menuVolumeSlider.value = WorldRules.volume;

                p1.transform.position = p1StartPos;
                p1.GetComponent<HealthManager>().ResetHealth();
                p1.GetComponent<PlayerController>().currentSide = PlayerController.Side.Left;
                p1.GetComponent<PlayerPhysics>().enabled = false;
                p1Animator.SetTrigger("Revive");
                p1.SetActive(false);
                p1.SetActive(true);
                if (WorldRules.PvP)
                {
                    p2.transform.position = p2StartPos;
                    p2.GetComponent<HealthManager>().ResetHealth();
                    p2.GetComponent<PlayerController>().currentSide = PlayerController.Side.Right;
                    p2.GetComponent<PlayerPhysics>().enabled = false;
                    p2Animator.SetTrigger("Revive");
                    p2.SetActive(false);
                    p2.SetActive(true);
                }
                else
                {
                    CPU.transform.position = p2StartPos;
                    CPU.GetComponent<HealthManager>().ResetHealth();
                    CPU.GetComponent<AIController>().currentSide = AIController.Side.Right;
                    CPU.GetComponent<AIPhysics>().enabled = false;
                    CPUAnimator.SetTrigger("Revive");
                    CPU.SetActive(false);
                    CPU.SetActive(true);
                }
                break;
            case GameState.Fighting:
                gameState = GameState.Fighting;
                menuUI.SetActive(false);
                pauseUI.SetActive(false);
                pauseSetting.enabled = false;
                roundStartUI.SetActive(false);
                fightUI.SetActive(true);
                break;
            case GameState.Pause:
                gameState = GameState.Pause;
                pauseUI.SetActive(true);
                pauseSetting.enabled = true;
                fightUI.SetActive(false);
                roundStartUI.SetActive(false);
                pauseVolumeSlider.value = WorldRules.volume;
                break;
            case GameState.RoundStart:
                gameState = GameState.RoundStart;
                p1.GetComponent<HealthManager>().ResetHealth();
                if (WorldRules.PvP) p2.GetComponent<HealthManager>().ResetHealth();
                else               CPU.GetComponent<HealthManager>().ResetHealth();
                roundOverUI.SetActive(false);
                roundStartUI.SetActive(true);
                fightUI.SetActive(true);
                break;
            case GameState.RoundOver:
                // todo cleanup
                gameState = GameState.RoundOver;
                GameObject opponent = WorldRules.PvP ? p2 : CPU;
                winnerName = string.Empty;

                if (CheckDead(p1))
                {
                    p2Wins++;
                    winnerName = opponent.GetComponent<HealthManager>().nameTag.text;
                }
                if (CheckDead(opponent))
                {
                    p1Wins++;
                    winnerName = winnerName == string.Empty ? p1.GetComponent<HealthManager>().nameTag.text : "NOBODY";
                }

                p1.GetComponent<PlayerController>().enabled = false;
                if (opponent.TryGetComponent(out PlayerController pController)) pController.enabled = false;
                else if (opponent.TryGetComponent(out AIController aiController)) aiController.enabled = false;

                roundWinner.text = winnerName + " WINS";
                currentScore.text = p1Wins + " - " + p2Wins;
                roundOverUI.SetActive(true);
                break;
            case GameState.GameOver:
                // todo also cleanup
                gameState = GameState.GameOver;
                fightUI.SetActive(false);
                pauseUI.SetActive(false);
                pauseSetting.enabled = false;
                gameOverUI.SetActive(true);

                winnerName = "NOBODY";
                if (CheckDead(p1))
                {
                    if (WorldRules.PvP && !CheckDead(p2))
                    {
                        winnerName = p2.GetComponent<HealthManager>().nameTag.text;
                        p2.GetComponent<PlayerController>().enabled = false;
                    }
                    else if (!CheckDead(CPU))
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
        return healthManager.currentHealth == 0 ? true : false;
    }

    public void TriggerNewRound()
    {
        p1.GetComponent<HealthManager>().ResetHealth();
        p1Animator.SetTrigger("Revive");
        if (WorldRules.PvP)
        {
            p2.GetComponent<HealthManager>().ResetHealth();
            p2Animator.SetTrigger("Revive");
        }
        else
        {
            CPU.GetComponent<HealthManager>().ResetHealth();
            CPUAnimator.SetTrigger("Revive");
        }
        SetGameState(GameState.RoundStart);
    }

    public void EndGame()
    {
        p1Animator.enabled = true;
        p1.GetComponent<HealthManager>().SendDamage(200);
        if (WorldRules.PvP)
        {
            p2Animator.enabled = true;
            p2.GetComponent<HealthManager>().SendDamage(200);
        }
        else
        {
            CPUAnimator.enabled = true;
            CPU.GetComponent<HealthManager>().SendDamage(200);
        }
        SetGameState(GameState.GameOver);
    }

    public void TriggerMenu()
    {
        SetGameState(GameState.Menu);
    }

    public void PauseGame()
    {
        if (gameState == GameState.Pause)
        {
            SetGameState(GameState.Pause);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
