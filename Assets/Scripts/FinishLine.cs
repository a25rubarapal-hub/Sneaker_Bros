using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameObject scoreboardScreen;
    public PlayerScore playerScore;

    [Header("Sonido")]
    public AudioClip sonidoVictoria;
    [Range(0f, 1f)] public float volumen = 1f;
    public AudioSource musicaFondo;

    [Header("Progreso y Puntuación")]
    public int nivelADesbloquear = 2;
    public int nivelActual = 1; // NUEVO: Para saber qué nivel estamos jugando

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            // --- GUARDAR EL PROGRESO ---
            int progresoActual = PlayerPrefs.GetInt("NivelAlcanzado", 1);
            if (nivelADesbloquear > progresoActual)
            {
                PlayerPrefs.SetInt("NivelAlcanzado", nivelADesbloquear);
            }

            // --- NUEVO: GUARDAR LA PUNTUACIÓN DE ESTE NIVEL ---
            if (playerScore != null)
            {
                // Guarda los puntos bajo un nombre único, ej: "Puntos_Nivel_1"
                PlayerPrefs.SetInt("Puntos_Nivel_" + nivelActual, playerScore.puntuacion);
            }
            PlayerPrefs.Save();
            // --------------------------------------------------

            if (musicaFondo != null)
            {
                musicaFondo.ignoreListenerPause = false;
                musicaFondo.Stop();
            }

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

            Time.timeScale = 0f;

            if (scoreboardScreen != null)
                scoreboardScreen.SetActive(true);

            ScoreboardUI ui = scoreboardScreen.GetComponentInChildren<ScoreboardUI>();

            if (ui != null && playerScore != null)
            {
                ui.MostrarFinal(playerScore.puntuacion);
            }
        }
    }
}