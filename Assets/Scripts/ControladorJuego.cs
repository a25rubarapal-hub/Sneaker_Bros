using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorJuego : MonoBehaviour
{

public void ComenzarJuego()
{
    Debug.Log("BOTÓN FUNCIONA");
    SceneManager.LoadScene("SneackersBros");
}

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}