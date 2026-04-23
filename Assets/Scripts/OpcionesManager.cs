using UnityEngine;
using UnityEngine.UI;

public class OpcionesManager : MonoBehaviour
{
    [Header("UI")]
    public Toggle toggleFullscreen;
    public Slider sliderVolumen;

    void Start()
    {
        // Fullscreen al iniciar según estado actual del juego
        toggleFullscreen.isOn = Screen.fullScreen;

        // Volumen al máximo al iniciar
        sliderVolumen.value = 1f;
        AudioListener.volume = 1f;
    }

    // FULLSCREEN
    public void CambiarPantallaCompleta(bool esFullscreen)
    {
        Screen.fullScreen = esFullscreen;
    }

    // VOLUMEN
    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
    }
}