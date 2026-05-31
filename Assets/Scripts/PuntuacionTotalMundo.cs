using UnityEngine;
using TMPro; // Usamos TextMeshPro para el texto

public class PuntuacionTotalMundo : MonoBehaviour
{
    public TextMeshProUGUI textoTotal;

    void OnEnable() // OnEnable se ejecuta justo cuando el panel se enciende
    {
        int sumaTotal = 0;

        // Sumamos los puntos guardados de los 5 niveles
        for (int i = 1; i <= 5; i++)
        {
            sumaTotal += PlayerPrefs.GetInt("Puntos_Nivel_" + i, 0);
        }

        // Lo mostramos en pantalla
        if (textoTotal != null)
        {
            textoTotal.text = "PUNTUACIÓN MUNDO 1: " + sumaTotal.ToString();
        }
    }
}