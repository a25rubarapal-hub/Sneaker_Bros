using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGeneral : MonoBehaviour
{
    [Header("Escenas (Escribe el nombre exacto)")]
    public string escenaPlay;
    public string escenaTutorial;



    public void BotonPlay()
    {
        SceneManager.LoadScene(escenaPlay);
    }

    public void BotonTutorial()
    {
        SceneManager.LoadScene(escenaTutorial);
    }

    public void BotonSettings()
    {
        // Buscamos automáticamente al Manager Inmortal que ha sobrevivido a la muerte
        if (OpcionesManager.Instance != null && OpcionesManager.Instance.panelSettings != null)
        {
            OpcionesManager.Instance.panelSettings.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el OpcionesManager en la escena.");
        }
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}