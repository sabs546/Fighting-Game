using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStateControl : MonoBehaviour
{
    public enum GameState { Menu, Fighting, Pause, Unpause, GameOver };
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
    public GameObject gameOverUI;

    [Header("Misc")]
    public TextMeshProUGUI winnerTag;
    private string winnerName;
    [SerializeField]
    private UnityEngine.UI.Slider menuVolumeSlider;
    [SerializeField]
    private UnityEngine.UI.Slider pauseVolumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Menu;
        p1StartPos = p1.transform.position;
        p2StartPos = CPU.transform.position;

        p1Animator = p1.GetComponent<Animator>();
        p2Animator = p2.GetComponent<Animator>();
        CPUAnimator = CPU.GetComponent<Animator>();

        pauseSetting = pauseUI.GetComponent<FreezeGame>();
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
                fightUI.SetActive(true);
                break;
            case GameState.Pause:
                gameState = GameState.Pause;
                pauseUI.SetActive(true);
                pauseSetting.enabled = true;
                fightUI.SetActive(false);
                pauseVolumeSlider.value = WorldRules.volume;
                break;
            case GameState.GameOver:
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
        if (healthManager.currentHealth == 0)
        {
            return true;
        }
        winnerName = healthManager.nameTag.text;
        return false;
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
        else
        {
            SetGameState(GameState.Unpause);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
