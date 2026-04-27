using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int health;

    private bool isInvincible = false;
    private bool isDead = false;

    public float invincibleTime = 1f;

    [Header("UI Death Screen")]
    public GameObject gameOverScreen;

    [Header("Score System")]
    public PlayerScore playerScore;

    void Start()
    {
        health = maxHealth;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0)
        {
            StartCoroutine(DieSequence());
            return;
        }

        StartCoroutine(Invincibility());
    }

    IEnumerator DieSequence()
    {
        isDead = true;

        GetComponent<Movimiento>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("MainMenu");
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        int oldHealth = health;

        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        // recompensa si realmente sube vida
        if (health > oldHealth)
        {
            if (playerScore != null)
                playerScore.SumarPuntos(200);
        }
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
}