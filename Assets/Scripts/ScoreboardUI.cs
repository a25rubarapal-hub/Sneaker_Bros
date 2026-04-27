using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreboardUI : MonoBehaviour
{
    public TMP_Text puntuacionText;
    public TMP_InputField nombreInput;

    private int puntuacionFinal;

    public void MostrarFinal(int puntuacion)
    {
        puntuacionFinal = puntuacion;

        if (puntuacionText != null)
        {
            puntuacionText.text = "Puntuación: " + puntuacion;
        }

        if (nombreInput != null)
        {
            nombreInput.text = "";
        }
    }

    // 🔘 BOTÓN CONFIRMAR
    public void ConfirmarYVolver()
    {
        string nombre = nombreInput.text;

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("Escribe un nombre antes de continuar.");
            return;
        }

        Debug.Log("Jugador: " + nombre + " | Puntos: " + puntuacionFinal);

        VolverAlMenu();
    }

    // 🏠 VOLVER AL MENÚ
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}