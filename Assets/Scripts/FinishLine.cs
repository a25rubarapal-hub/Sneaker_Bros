using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameObject scoreboardScreen; // Panel del scoreboard
    public PlayerScore playerScore;     // Referencia al PlayerScore

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            // ⏸ Pausar el juego
            Time.timeScale = 0f;

            // 📊 Activar scoreboard
            if (scoreboardScreen != null)
                scoreboardScreen.SetActive(true);

            // 🧠 Pasar puntuación al UI
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