using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartsParent;

    public int maxHealth = 3;
    public int currentHealth = 3;

    Image[] hearts;

    void Start()
    {
        hearts = new Image[maxHealth];

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject h = Instantiate(heartPrefab, heartsParent);
            hearts[i] = h.GetComponent<Image>();
        }

        UpdateHearts();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentHealth;
        }
    }
}