using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private PlayerHealth playerHealth;

    private int lastHealth = -1;
    private int lastMaxHealth = -1;

    void Awake()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    void Start()
    {
        RefreshHearts();
    }

    void Update()
    {
        if (playerHealth == null) return;

        if (playerHealth.health != lastHealth || playerHealth.maxHealth != lastMaxHealth)
        {
            RefreshHearts();
        }
    }

    void RefreshHearts()
    {
        lastHealth = playerHealth.health;
        lastMaxHealth = playerHealth.maxHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            bool active = i < lastMaxHealth;
            hearts[i].enabled = active;

            if (active)
            {
                hearts[i].sprite = (i < lastHealth) ? fullHeart : emptyHeart;
            }
        }
    }
}