using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject panelInicio; // Arrastra aquí el HUD bloqueado
    public GameObject panelJuego;  // Arrastra aquí el HUD del juego

    void Start()
    {
        // Al comenzar, bloqueamos el juego
        BloquearJuego();
    }

    public void BloquearJuego()
    {
        // Congelar el tiempo del juego
        Time.timeScale = 0f;

        // Mostrar menú de inicio y ocultar HUD de juego
        panelInicio.SetActive(true);
        panelJuego.SetActive(false);

        // Opcional: Mostrar y liberar el cursor del ratón
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ComenzarJuego()
    {
        // Reanudar el tiempo del juego
        Time.timeScale = 1f;

        // Intercambiar HUDs
        panelInicio.SetActive(false);
        panelJuego.SetActive(true);

        // Opcional: Bloquear cursor para el juego (si es un shooter o similar)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }
}