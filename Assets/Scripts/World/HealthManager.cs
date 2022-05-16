using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;
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
        healthBar.localScale = new Vector3(currentHealth / 100.0f, 1.0f, 1.0f);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GetComponent<Animator>().SetTrigger("Die");
            gameStateControl.GetComponent<GameStateControl>().SetGameState(GameStateControl.GameState.GameOver);
        }
    }
}
