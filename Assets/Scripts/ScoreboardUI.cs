using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreboardUI : MonoBehaviour
{
    public TMP_Text puntuacionText;
    public TMP_InputField nombreInput;

    public GameTimer gameTimer; // ⏱ NUEVO

    private int puntuacionFinal;

    public void MostrarFinal(int puntuacion)
    {
        Debug.Log("PUNTUACIÓN BASE: " + puntuacion);

        int bonusTiempo = 0;

        if (gameTimer != null)
        {
            float tiempoRestante = gameTimer.GetTiempoRestante();
            bonusTiempo = Mathf.CeilToInt(tiempoRestante) * 10; // 👈 ajusta multiplicador

            Debug.Log("BONUS TIEMPO: " + bonusTiempo);
        }

        puntuacionFinal = puntuacion + bonusTiempo;

        if (puntuacionText != null)
        {
            puntuacionText.text = "Puntuación: " + puntuacionFinal;
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

        int tiempoBonus = 0;

        if (gameTimer != null)
        {
            tiempoBonus = Mathf.CeilToInt(gameTimer.GetTiempoRestante()) * 10;
        }

        int puntuacionTotal = puntuacionFinal + tiempoBonus;

        Debug.Log("Jugador: " + nombre + " | Puntos finales: " + puntuacionTotal);

        VolverAlMenu();
    }

    // 🏠 VOLVER AL MENÚ
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}