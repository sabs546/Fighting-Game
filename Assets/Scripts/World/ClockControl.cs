using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockControl : MonoBehaviour
{
    [SerializeField]
    private GameStateControl gameStateControl;
    [SerializeField]
    private Image clockCircle;
    private float timeLimit;
    private float timeLeft;  // Time remaining
    private bool  begin;     // When to start counting

    // Start is called before the first frame update
    private void OnEnable()
    {
        clockCircle.fillAmount = 0.0f;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (clockCircle.fillAmount < timeLeft / WorldRules.roundTimer && timeLeft == WorldRules.roundTimer)
        {
            clockCircle.fillAmount += 1.0f * Time.deltaTime;
        }

        if (begin && timeLeft > 0.0f && GameStateControl.gameState == GameStateControl.GameState.Fighting)
        {
            timeLeft -= Time.deltaTime;
            clockCircle.fillAmount = timeLeft / WorldRules.roundTimer;
        }
        else if (begin && timeLeft <= 0.0f)
        {
            begin = false;
            timeLeft = 0.0f;
            gameStateControl.SetGameState(GameStateControl.GameState.RoundOver);
        }
    }

    public void BeginTimer()
    {
        begin = true;
    }

    public void ResetTimer()
    {
        timeLeft = WorldRules.roundTimer;
        begin = false;
    }
}
