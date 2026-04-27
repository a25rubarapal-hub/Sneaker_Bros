using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreboardUI : MonoBehaviour
{
    public TMP_Text puntuacionText;

    public GameObject inputPanel;
    public TMP_InputField nombreInput;

    private int puntuacionFinal;

    public void MostrarFinal(int puntuacion)
    {
        Debug.Log("MostrarFinal llamado con puntuación: " + puntuacion);
        puntuacionFinal = puntuacion;

        if (puntuacionText != null)
            puntuacionText.text = "Puntuación: " + puntuacion;

        if (inputPanel != null)
            inputPanel.SetActive(true);
    }

    public void GuardarNombre()
    {
        string nombre = nombreInput.text;

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("Escribe un nombre.");
            return;
        }

        Debug.Log($"Jugador: {nombre} | Puntos: {puntuacionFinal}");
    }

        public void VolverAlMenu()
    {
        Time.timeScale = 1f; // por si el juego estaba pausado

        SceneManager.LoadScene("MainMenu");
    }
}