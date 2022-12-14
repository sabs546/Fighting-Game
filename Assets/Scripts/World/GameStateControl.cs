using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Bolt;

public class GameStateControl : MonoBehaviour
{
    // States =======================================================================
    public enum GameState { Menu, Fighting, Pause, RoundStart, RoundOver, GameOver };
    public static GameState gameState { get; private set; }

    // Fighters ===============
    [Header("Fighter Related")]
    [SerializeField]
    private GameObject p1;
    [SerializeField]
    private GameObject p2;
    [SerializeField]
    private GameObject CPU;
    // - Fighter Components -
    private Animator p1Animator;
    private Animator p2Animator;
    private Animator CPUAnimator;
    private Vector2 p1StartPos;
    private Vector2 p2StartPos;

    // UI ===========================================================
    [Header("Menu Related")]
    [SerializeField]
    private GameObject menuUI;       // Main menu
    [SerializeField]
    private GameObject fightUI;      // Healthbars
    [SerializeField]
    private GameObject pauseUI;      // Pause
    [SerializeField]
    private FreezeGame pauseSetting; // When pause stops the fighting
    [SerializeField]
    private GameObject roundStartUI; // Ready banner
    [SerializeField]
    private GameObject roundOverUI;  // Round over banner
    [SerializeField]
    private GameObject gameOverUI;   // Game over banner
    [SerializeField]
    private GameObject treeLine;     // To reset the birds

    // Audio ================================================================
    [Header("Audio")]
    [SerializeField]
    private Slider menuVolumeSlider;  // Settings slider
    [SerializeField]
    private Slider pauseVolumeSlider; // Pause slider, Also syncs the sliders
    [SerializeField]
    private AudioSource source;       // Main AudioSource
    [SerializeField]
    private AudioClip readyFX;        // Movement noise
    [SerializeField]
    private AudioClip fightFX;        // Fighting noise

    // Rounds ============================================================
    [Header("Round Related")]
    [SerializeField]
    private TextMeshProUGUI roundWinner;  // Name for the banner
    [SerializeField]
    private TextMeshProUGUI currentScore; // Round wins for the banner
    [SerializeField]
    private Image p1RoundCount;           // For the lives circle
    [SerializeField]
    private Image p2RoundCount;           // For the other likes circle
    [SerializeField]
    private ClockControl roundClock;      // To reset the timer
    [SerializeField]
    private Image whiteOut;               // When the sky lights up
    private int p1Wins;                   // Keeps track for current score
    private int p2Wins;                   // Keeps track for other score
    private float ticker;                 // For whiteout fading

    // Game Over =================================================
    [Header("Game Over")]
    public TextMeshProUGUI winnerTag; // To set the winners name
    private string winnerName;        // To store the winners name
    [SerializeField]
    private Button onlineStart;
    [SerializeField]
    private CreaterAndJoinRooms room;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Menu;
        p1StartPos = p1.transform.position;
        p2StartPos = CPU.transform.position;

        p1Animator = p1.GetComponent<Animator>();
        p2Animator = p2.GetComponent<Animator>();
        CPUAnimator = CPU.GetComponent<Animator>();

        ticker = 0.0f;
    }

    private void Update()
    {
        if (gameState == GameState.RoundOver || gameState == GameState.GameOver)
        {
            if (WorldRules.gameSpeed < 1.0f)
            {
                ticker = 1.0f;
                WorldRules.gameSpeed += 0.1f * Time.deltaTime;
                p1Animator.speed = WorldRules.gameSpeed;
                if (WorldRules.PvP) p2Animator.speed = WorldRules.gameSpeed;
                else                CPUAnimator.speed = WorldRules.gameSpeed;
                if (WorldRules.gameSpeed > 1.0f)
                {
                    WorldRules.gameSpeed = 1.0f;
                }
                whiteOut.color = new Color(1.0f, 1.0f, 1.0f, WorldRules.gameSpeed * 2.0f);
            }
        }
        if (gameState == GameState.RoundStart)
        {
            if (ticker > 0.0f)
            {
                ticker -= Time.deltaTime;
                if (ticker < 0.0f)
                {
                    ticker = 0.0f;
                }
                whiteOut.color = new Color(1.0f, 1.0f, 1.0f, ticker);
            }
        }
        if (gameState == GameState.Fighting || gameState == GameState.Menu)
        {
            if (whiteOut.color.a > 0.0f)
            {
                whiteOut.color = new Color(whiteOut.color.r, whiteOut.color.g, whiteOut.color.b, whiteOut.color.a - Time.deltaTime);
                if (whiteOut.color.a < 0.0f)
                {
                    whiteOut.color = new Color(whiteOut.color.r, whiteOut.color.g, whiteOut.color.b, 0.0f);
                }
            }
        }
    }

    public void SetGameState(GameState state)
    {
        GameObject opponent;
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
                p1.SetActive(false);
                p1.SetActive(true);

                p1RoundCount.fillAmount = 1.0f;
                p2RoundCount.fillAmount = 1.0f;
                p1Wins = 0;
                p2Wins = 0;

                if (WorldRules.PvP)
                {
                    p2.transform.position = p2StartPos;
                    p2.GetComponent<HealthManager>().ResetHealth();
                    p2.GetComponent<PlayerController>().currentSide = PlayerController.Side.Right;
                    p2.GetComponent<PlayerPhysics>().enabled = false;
                    p2.SetActive(false);
                    p2.SetActive(true);
                }
                else
                {
                    CPU.transform.position = p2StartPos;
                    CPU.GetComponent<HealthManager>().ResetHealth();
                    CPU.GetComponent<AIController>().currentSide = AIController.Side.Right;
                    CPU.GetComponent<AIPhysics>().enabled = false;
                    CPU.SetActive(false);
                    CPU.SetActive(true);
                }

                Disturbance[] treeLineDisturbances = treeLine.GetComponentsInChildren<Disturbance>();
                foreach (Disturbance disturbance in treeLineDisturbances)
                {
                    disturbance.enabled = true;
                }
                break;
            case GameState.Fighting:
                fightUI.SetActive(true);
                if (WorldRules.roundTimer > 0.0f)
                {
                    roundClock.ResetTimer();
                }
                if (gameState == GameState.RoundStart)
                {
                    p1.GetComponent<PlayerAttackController>().enabled = true;
                    p2.GetComponent<PlayerAttackController>().enabled = true;
                    CPU.GetComponent<AIAttackController>().enabled = true;
                    source.clip = fightFX;
                    source.Play();
                    roundClock.BeginTimer();
                }
                gameState = GameState.Fighting;
                menuUI.SetActive(false);
                pauseUI.SetActive(false);
                pauseSetting.enabled = false;
                roundStartUI.SetActive(false);
                break;
            case GameState.Pause:
                gameState = GameState.Pause;
                pauseUI.SetActive(true);
                if (WorldRules.online) pauseSetting.enabled = false;
                fightUI.SetActive(false);
                pauseVolumeSlider.value = WorldRules.volume;
                break;
            case GameState.RoundStart:
                gameState = GameState.RoundStart;
                source.clip = readyFX;
                source.Play();
                p1.GetComponent<HealthManager>().ResetHealth();
                p1.GetComponent<PlayerAttackController>().enabled = false;
                p1.GetComponent<PlayerController>().enabled = true;
                if (WorldRules.PvP)
                {
                    p2.GetComponent<HealthManager>().ResetHealth();
                    p2.GetComponent<PlayerAttackController>().enabled = false;
                    p2.GetComponent<PlayerController>().enabled = true;
                }
                else
                {
                    CPU.GetComponent<HealthManager>().ResetHealth();
                    CPU.GetComponent<AIAttackController>().enabled = false;
                    CPU.GetComponent<AIController>().enabled = true;
                }
                
                roundOverUI.SetActive(false);
                roundStartUI.SetActive(true);
                fightUI.SetActive(true);
                if (WorldRules.roundTimer > 0.0f)
                {
                    roundClock.ResetTimer();
                }
                break;
            case GameState.RoundOver:
                // todo cleanup
                gameState = GameState.RoundOver;
                opponent = WorldRules.PvP ? p2 : CPU;
                winnerName = "NOBODY";
                bool nobodyDied = true;

                if (CheckDead(p1))
                {
                    p2Wins++;
                    winnerName = opponent.GetComponent<HealthManager>().nameTag.text;
                    p1RoundCount.fillAmount = 1.0f - ((float)p2Wins / (float)WorldRules.roundLimit);
                    nobodyDied = false;
                }
                if (CheckDead(opponent))
                {
                    p1Wins++;
                    winnerName = p1.GetComponent<HealthManager>().nameTag.text;
                    p2RoundCount.fillAmount = 1.0f - ((float)p1Wins / (float)WorldRules.roundLimit);
                    nobodyDied = false;
                }

                if (roundClock.gameObject.activeSelf && nobodyDied)
                {
                    switch (CheckLowest(p1, opponent))
                    {
                        case -1:
                            p1Wins++;
                            winnerName = p1.GetComponent<HealthManager>().nameTag.text;
                            p2RoundCount.fillAmount = 1.0f - ((float)p1Wins / (float)WorldRules.roundLimit);
                            break;
                        case 0:
                            p1Wins++;
                            p2Wins++;
                            p1RoundCount.fillAmount = 1.0f - ((float)p2Wins / (float)WorldRules.roundLimit);
                            p2RoundCount.fillAmount = 1.0f - ((float)p1Wins / (float)WorldRules.roundLimit);
                            break;
                        case 1:
                            p2Wins++;
                            winnerName = opponent.GetComponent<HealthManager>().nameTag.text;
                            p1RoundCount.fillAmount = 1.0f - ((float)p2Wins / (float)WorldRules.roundLimit);
                            break;
                    }
                }

                p1.GetComponent<PlayerController>().enabled = false;
                if (opponent.TryGetComponent(out PlayerController pController)) pController.enabled = false;
                else if (opponent.TryGetComponent(out AIController aiController)) aiController.enabled = false;

                if (p1Wins == WorldRules.roundLimit || p2Wins == WorldRules.roundLimit)
                {
                    SetGameState(GameState.GameOver);
                }
                else
                {
                    roundWinner.text = winnerName + " WINS";
                    currentScore.text = p1Wins + " - " + p2Wins;
                    roundOverUI.SetActive(true);
                }
                break;
            case GameState.GameOver:
                // todo also cleanup
                gameState = GameState.GameOver;
                fightUI.SetActive(false);
                pauseUI.SetActive(false);
                pauseSetting.enabled = false;
                roundOverUI.SetActive(false);
                gameOverUI.SetActive(true);
                opponent = WorldRules.PvP ? p2 : CPU;

                p1.GetComponent<PlayerController>().enabled = false;
                if (WorldRules.PvP)
                {
                    opponent.GetComponent<PlayerController>().enabled = false;
                }
                else
                {
                    opponent.GetComponent<AIController>().enabled = false;
                }

                winnerName = "NOBODY";
                if (p1Wins < p2Wins)
                {
                    winnerName = opponent.GetComponent<HealthManager>().nameTag.text;
                }
                else if (p1Wins > p2Wins)
                {
                    winnerName = p1.GetComponent<HealthManager>().nameTag.text;
                }

                winnerTag.SetText(winnerName);
                break;
        }
    }

    private bool CheckDead(GameObject fighter)
    {
        HealthManager healthManager = fighter.GetComponent<HealthManager>();
        return healthManager.currentHealth == 0 ? true : false;
    }

    // Spefically for ties
    private int CheckLowest(GameObject p1, GameObject p2)
    {
        float p1Health = p1.GetComponent<HealthManager>().currentHealth;
        float p2Health = p2.GetComponent<HealthManager>().currentHealth;
        if (p1Health > p2Health)
        {
            return -1;
        }
        else if (p2Health > p1Health)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void EndGame()
    {
        p1Animator.enabled = true;
        p1.GetComponent<HealthManager>().Kill();
        if (WorldRules.PvP)
        {
            p2Animator.enabled = true;
            p2.GetComponent<HealthManager>().Kill();
        }
        else
        {
            CPUAnimator.enabled = true;
            CPU.GetComponent<HealthManager>().Kill();
        }

        if (WorldRules.online)
        {
            room.LeaveRoom();
            onlineStart.interactable = false;
        }

        SetGameState(GameState.GameOver);
    }

    public void IncorrectEndGame()
    {
        p1Animator.enabled = true;
        p2Animator.enabled = true;
        p1.GetComponent<HealthManager>().Kill();
        p2.GetComponent<HealthManager>().Kill();
        room.LeaveRoom();
        onlineStart.interactable = false;
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

    public void AlignVolume()
    {
        source.volume = WorldRules.volume / 100.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
