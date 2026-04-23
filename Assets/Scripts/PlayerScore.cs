using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public int puntuacion = 0;
    public TextMeshProUGUI puntuacionText;

    public void SumarMoneda()
    {
        puntuacion++;
        puntuacionText.text = puntuacion.ToString();
    }
}