using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth { get; private set; }
    public TMPro.TextMeshProUGUI nameTag;
    public RectTransform healthBar;
    public RectTransform backBar;
    public GameStateControl gameStateControl;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (backBar.localScale.x > healthBar.localScale.x)
        {
            backBar.localScale = new Vector3(backBar.localScale.x - (0.1f * Time.deltaTime), 1.0f, 1.0f);
        }
    }

    public void SendDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GetComponent<Animator>().SetTrigger("Die");
            WorldRules.gameSpeed = 0.5f;
            Camera.main.GetComponent<CameraControl>().StartShake(64, 8, 1.0f);
            GetComponent<Animator>().speed = WorldRules.gameSpeed;
            gameStateControl.GetComponent<GameStateControl>().SetGameState(GameStateControl.GameState.GameOver);
        }
        if (TryGetComponent(out PlayerAttackController pAttackController))
        {
            pAttackController.CancelAttack();
        }
        else if (TryGetComponent(out AIAttackController AIAttackController))
        {
            AIAttackController.CancelAttack();
        }
        healthBar.localScale = new Vector3(currentHealth / (float)maxHealth, 1.0f, 1.0f);
    }

    public void SetMaxHealth(Slider slider)
    {
        maxHealth = (int)slider.value;
        currentHealth = maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        WorldRules.gameSpeed = 1.0f;
        GetComponent<Animator>().speed = WorldRules.gameSpeed;
        healthBar.localScale = new Vector3(currentHealth / maxHealth, 1.0f, 1.0f);
        backBar.localScale = healthBar.localScale;
    }
}
