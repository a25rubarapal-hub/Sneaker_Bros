using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panel de Pausa (Arrastra tu menú visual aquí)")]
    public GameObject panelPausa;

    private bool juegoPausado = false;

    void Start()
    {
        Debug.Log("¡El Gestor de Pausa ha arrancado al cargar el nivel!");

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Al pulsar Escape...
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Has pulsado ESCAPE en el nivel.");

            // 1. Si los ajustes de piedra están abiertos, los cerramos
            if (OpcionesManager.Instance != null &&
                OpcionesManager.Instance.panelSettings != null &&
                OpcionesManager.Instance.panelSettings.activeInHierarchy)
            {
                Debug.Log("Cerrando el panel de ajustes globales...");
                OpcionesManager.Instance.panelSettings.SetActive(false);
            }
            // 2. Si los ajustes están cerrados, controlamos la pausa
            else
            {
                // ¡AQUÍ ESTÁ EL CAMBIO!
                // Solo activamos la pausa si el juego NO está pausado.
                // Si ya está pausado, el Escape no hará absolutamente nada.
                if (!juegoPausado)
                {
                    Debug.Log("Poniendo la pausa...");
                    Pausar();
                }
                else
                {
                    Debug.Log("El juego ya está pausado. Usa el botón Continuar en pantalla.");
                }
            }
        }
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    public void Pausar()
    {
        juegoPausado = true;
        Time.timeScale = 0f; // Congela el tiempo
        if (panelPausa != null) panelPausa.SetActive(true);
    }

    public void Reanudar()
    {
        juegoPausado = false;
        Time.timeScale = 1f; // Descongela el tiempo
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    public void BotonAjustes()
    {
        if (OpcionesManager.Instance != null && OpcionesManager.Instance.panelSettings != null)
        {
            OpcionesManager.Instance.panelSettings.SetActive(true);
        }
    }

    public void BotonMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void BotonSalir()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}