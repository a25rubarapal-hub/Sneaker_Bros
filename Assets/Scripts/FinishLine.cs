using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameObject scoreboardScreen;
    public PlayerScore playerScore;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            Time.timeScale = 0f;

            scoreboardScreen.SetActive(true);

            ScoreboardUI ui = scoreboardScreen.GetComponent<ScoreboardUI>();

            if (ui != null && playerScore != null)
            {
                ui.MostrarFinal(playerScore.puntuacion);
            }
        }
    }
}