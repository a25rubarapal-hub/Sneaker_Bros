using UnityEngine;
using TMPro;

public class PlayerCoins : MonoBehaviour
{
    public int monedas = 0;

    public TextMeshProUGUI monedasText;

    public PlayerHealth playerHealth;
    public PlayerScore playerScore;

    public void SumarMoneda()
    {
        monedas++;

        // 💯 puntos
        if (playerScore != null)
            playerScore.SumarPuntos(100);

        // ❤️ vida
        if (monedas % 10 == 0 && playerHealth != null)
            playerHealth.Heal(1);

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (monedasText != null)
        {
            monedasText.text = monedas.ToString();
        }
        else
        {
            Debug.LogWarning("MonedasText NO asignado en PlayerCoins");
        }
    }
}