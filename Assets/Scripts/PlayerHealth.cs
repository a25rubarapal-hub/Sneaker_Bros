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
    public GameObject gameOverScreen; // 🔹 arrastra aquí la imagen

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

        // parar jugador
        GetComponent<Movimiento>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // 🔹 espera 1 segundo antes de mostrar pantalla
        yield return new WaitForSeconds(1f);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        // 🔹 espera otro segundo
        yield return new WaitForSeconds(1f);

        // 🔹 cargar menú principal
        SceneManager.LoadScene("MainMenu");
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
}