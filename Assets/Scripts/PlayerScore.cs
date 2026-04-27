using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public int puntuacion = 0;
    public TextMeshProUGUI puntuacionText;

    public void SumarPuntos(int puntos)
    {
        puntuacion += puntos;

        if (puntuacionText != null)
            puntuacionText.text = puntuacion.ToString();
    }
}