using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelNiveles : MonoBehaviour
{
    [Header("Botones Físicos (Para ocultarlos)")]
    public GameObject botonNivel2;
    public GameObject botonNivel3;
    public GameObject botonNivel4;
    public GameObject botonNivel5;

    [Header("Nombres de las Escenas")]
    public string nombreNivel1 = "Nivel_1";
    public string nombreNivel2 = "Nivel_2";
    public string nombreNivel3 = "Nivel_3";
    public string nombreNivel4 = "Nivel_4";
    public string nombreNivel5 = "Nivel_5";

    [Header("Menú Principal")]
    public string escenaMenuPrincipal = "MainMenu";

    void Start()
    {
        // 1. Por defecto, ocultamos todos los botones excepto el del Nivel 1
        if (botonNivel2 != null) botonNivel2.SetActive(false);
        if (botonNivel3 != null) botonNivel3.SetActive(false);
        if (botonNivel4 != null) botonNivel4.SetActive(false);
        if (botonNivel5 != null) botonNivel5.SetActive(false);

        // 2. Leemos la memoria para saber hasta dónde hemos llegado
        int nivelAlcanzado = PlayerPrefs.GetInt("NivelAlcanzado", 1);

        // 3. Encendemos los botones según el progreso guardado
        if (nivelAlcanzado >= 2 && botonNivel2 != null) botonNivel2.SetActive(true);
        if (nivelAlcanzado >= 3 && botonNivel3 != null) botonNivel3.SetActive(true);
        if (nivelAlcanzado >= 4 && botonNivel4 != null) botonNivel4.SetActive(true);
        if (nivelAlcanzado >= 5 && botonNivel5 != null) botonNivel5.SetActive(true);
    }

    // --- FUNCIONES DE LOS BOTONES ---

    public void BotonNivel1() { Time.timeScale = 1f; SceneManager.LoadScene(nombreNivel1); }
    public void BotonNivel2() { Time.timeScale = 1f; SceneManager.LoadScene(nombreNivel2); }
    public void BotonNivel3() { Time.timeScale = 1f; SceneManager.LoadScene(nombreNivel3); }
    public void BotonNivel4() { Time.timeScale = 1f; SceneManager.LoadScene(nombreNivel4); }
    public void BotonNivel5() { Time.timeScale = 1f; SceneManager.LoadScene(nombreNivel5); }

    public void BotonVolver() { Time.timeScale = 1f; SceneManager.LoadScene(escenaMenuPrincipal); }

    // --- BOTÓN SECRETO PARA PRUEBAS ---
       
    public void ResetearDatos()
    {
        // DeleteAll borra de golpe el "NivelAlcanzado" y todas las puntuaciones
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("¡Borrón y cuenta nueva! Progreso y puntos eliminados de fábrica.");
    }
}