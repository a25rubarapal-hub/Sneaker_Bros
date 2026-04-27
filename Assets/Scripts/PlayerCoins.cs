using UnityEngine;

public class PlayerCoins : MonoBehaviour
{
    public int monedas = 0;

    public PlayerHealth playerHealth;
    public PlayerScore playerScore;

    public void SumarMoneda()
    {
        monedas++;

        // 💯 puntuación
        if (playerScore != null)
            playerScore.SumarPuntos(100);

        // ❤️ vida cada 10 monedas
        if (monedas % 10 == 0)
        {
            if (playerHealth != null)
                playerHealth.Heal(1);
        }
    }
}