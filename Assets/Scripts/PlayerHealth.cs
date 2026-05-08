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

    [Header("Rebote de Daño")]
    public float fuerzaDañoX = 15f; // Mucho hacia atrás
    public float fuerzaDañoY = 3f;  // Poco hacia arriba
    public float tiempoBloqueoDaño = 0.3f; // Tiempo que pierdes el control al sufrir daño

    [Header("UI Death Screen")]
    public GameObject gameOverScreen;

    [Header("Score System")]
    public PlayerScore playerScore;

    [Header("Sonidos")]
    public AudioClip sonidoMuerte;
    public AudioClip sonidoDaño;
    [Range(0f, 1f)] public float volumenMuerte = 1f;
    [Range(0f, 1f)] public float volumenDaño = 1f;
    public AudioSource musicaFondo;

    void Start()
    {
        health = maxHealth;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
    }

    // ── MODIFICADO: Ahora puede recibir quién es el atacante ──
    public void TakeDamage(int damage, Transform atacante = null)
    {
        if (isInvincible || isDead) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0)
        {
            StartCoroutine(DieSequence());
            return;
        }

        // --- Lógica del Empujón ---
        Movimiento mov = GetComponent<Movimiento>();
        if (mov != null)
        {
            // Si el atacante no se especifica, te empuja por la espalda.
            float dirX = -Mathf.Sign(transform.localScale.x);

            // Si sabemos quién nos atacó, nos empuja al lado contrario.
            if (atacante != null)
            {
                dirX = (transform.position.x < atacante.position.x) ? -1f : 1f;
            }

            mov.AplicarRebote(new Vector2(fuerzaDañoX * dirX, fuerzaDañoY), tiempoBloqueoDaño);
        }

        // Sonido de daño
        if (sonidoDaño != null)
        {
            AudioSource audioTemp = gameObject.AddComponent<AudioSource>();
            audioTemp.clip = sonidoDaño;
            audioTemp.volume = volumenDaño;
            audioTemp.spatialBlend = 0f;
            audioTemp.Play();
            Destroy(audioTemp, sonidoDaño.length);
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

        if (musicaFondo != null)
            musicaFondo.Stop();

        if (sonidoMuerte != null)
        {
            AudioSource audioTemp = gameObject.AddComponent<AudioSource>();
            audioTemp.clip = sonidoMuerte;
            audioTemp.volume = volumenMuerte;
            audioTemp.spatialBlend = 0f;
            audioTemp.Play();
        }

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