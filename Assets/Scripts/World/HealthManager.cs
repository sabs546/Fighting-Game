using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }
    public TMPro.TextMeshProUGUI nameTag;
    public RectTransform healthBar;
    public GameStateControl gameStateControl;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.localScale = new Vector3(currentHealth / maxHealth, 1.0f, 1.0f);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GetComponent<Animator>().SetTrigger("Die");
            gameStateControl.GetComponent<GameStateControl>().SetGameState(GameStateControl.GameState.GameOver);
        }
    }

    public void SetMaxHealth(Slider slider)
    {
        maxHealth = (int)slider.value;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthBar.localScale = new Vector3(currentHealth / maxHealth, 1.0f, 1.0f);
    }
}
