using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public int puntuacion = 0;
    public TextMeshProUGUI puntuacionText;

    public void SumarPuntos(int puntos)
    {
        puntuacion += puntos;

        // Reportar a Game_manager
        if (Game_manager.Instance != null)
        {
            Game_manager.Instance.totalScore += puntos;
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (puntuacionText != null)
        {
            puntuacionText.text = puntuacion.ToString();
        }
    }
}