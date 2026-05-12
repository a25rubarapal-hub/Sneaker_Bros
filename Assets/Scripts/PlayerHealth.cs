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
    public GameObject secondImage;

    [Header("Animación")]
    public float shrinkDuration = 2f;

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

        if (secondImage != null)
            secondImage.SetActive(false);
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

        // Mostrar ambas imágenes a la vez
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        if (secondImage != null)
            secondImage.SetActive(true);

        // Animar encogimiento de la primera
        Vector3 originalScale = gameOverScreen.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;

            gameOverScreen.transform.localScale =
                Vector3.Lerp(originalScale, targetScale, elapsed / shrinkDuration);

            yield return null;
        }

        gameOverScreen.transform.localScale = targetScale;

        yield return new WaitForSeconds(2f);

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