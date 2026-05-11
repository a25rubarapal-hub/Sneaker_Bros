using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreboardUI : MonoBehaviour
{
    public TMP_Text puntuacionText;
    public TMP_InputField nombreInput;

    public GameTimer gameTimer;

    private int puntuacionFinal;

    public void MostrarFinal(int puntuacion)
    {
        Debug.Log("PUNTUACIÓN BASE: " + puntuacion);

        int bonusTiempo = 0;

        if (gameTimer != null)
        {
            float tiempoRestante = gameTimer.GetTiempoRestante();
            bonusTiempo = Mathf.CeilToInt(tiempoRestante) * 10;
            Debug.Log("BONUS TIEMPO: " + bonusTiempo);
        }

        puntuacionFinal = puntuacion + bonusTiempo;

        if (puntuacionText != null)
            puntuacionText.text = "Puntuación: " + puntuacionFinal;

        if (nombreInput != null)
            nombreInput.text = "";
    }

    public void ConfirmarYVolver()
    {
        string nombre = nombreInput.text;

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("Escribe un nombre antes de continuar.");
            return;
        }

<<<<<<< Updated upstream
        int tiempoBonus = 0;

        if (gameTimer != null)
        {
            tiempoBonus = Mathf.CeilToInt(gameTimer.GetTiempoRestante()) * 10;
        }

        int puntuacionTotal = puntuacionFinal + tiempoBonus;

        // Actualizar datos en Game_manager
        if (Game_manager.Instance != null)
        {
            Game_manager.Instance.playerName = nombre;
            Game_manager.Instance.totalScore = puntuacionTotal; // Asegurar que tenga el total final
            Game_manager.Instance.SaveGameData();
        }

        Debug.Log("Jugador: " + nombre + " | Puntos finales: " + puntuacionTotal);
=======
        if (Game_manager.Instance != null)
        {
            Game_manager.Instance.playerName = nombre;
            Game_manager.Instance.totalScore = puntuacionFinal;
            Game_manager.Instance.SaveGameData();
        }

        Debug.Log("Jugador: " + nombre + " | Puntos finales: " + puntuacionFinal);
>>>>>>> Stashed changes

        StartCoroutine(EsperarYVolver());
    }

    IEnumerator EsperarYVolver()
    {
        yield return new WaitForSecondsRealtime(2f);
        VolverAlMenu();
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}