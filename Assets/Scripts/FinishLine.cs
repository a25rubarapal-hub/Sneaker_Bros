using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameObject scoreboardScreen;
    public PlayerScore playerScore;

    [Header("Sonido")]
    public AudioClip sonidoVictoria;
    [Range(0f, 1f)] public float volumen = 1f;
    public AudioSource musicaFondo;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            if (musicaFondo != null)
            {
                musicaFondo.ignoreListenerPause = false;
                musicaFondo.Stop();
            }

            // Suena el audio de victoria
            if (sonidoVictoria != null)
            {
                AudioSource audioTemp = gameObject.AddComponent<AudioSource>();
                audioTemp.clip = sonidoVictoria;
                audioTemp.volume = volumen;
                audioTemp.spatialBlend = 0f;
                audioTemp.ignoreListenerPause = true;
                audioTemp.Play();
                Destroy(audioTemp, sonidoVictoria.length);
            }

            // ⏸ Pausar el juego
            Time.timeScale = 0f;

            if (scoreboardScreen != null)
                scoreboardScreen.SetActive(true);

            ScoreboardUI ui = scoreboardScreen.GetComponentInChildren<ScoreboardUI>();

            if (ui != null && playerScore != null)
            {
                ui.MostrarFinal(playerScore.puntuacion);
            }
            else
            {
                Debug.LogWarning("Falta referencia en FinishLine");
            }
        }
    }
}