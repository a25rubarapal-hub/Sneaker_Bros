using UnityEngine;
using UnityEngine.UI; // ¡Súper importante para usar 'Image' y 'Sprite'!

public class OpcionesManager : MonoBehaviour
{
    public static OpcionesManager Instance;

    [Header("Elementos de la UI")]
    public GameObject panelSettings;
    public Slider sliderVolumen;

    [Header("Imágenes y Sprites de Botones")]
    public Image botonMuteImage;         // El componente Image del botón de silenciar
    public Sprite spriteSonidoActivo;     // Sprite cuando se escucha el juego
    public Sprite spriteSonidoMuteado;    // Sprite cuando está silenciado

    public Image botonFullscreenImage;   // El componente Image del botón de pantalla completa
    public Sprite spritePantallaCompleta; // Sprite de pantalla completa activa
    public Sprite spriteModoVentana;      // Sprite de modo ventana activo

    private bool estaMuteado = false;
    private bool esFullscreen = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        CargarOpciones();
        if (panelSettings != null) panelSettings.SetActive(false);
    }


    // ==========================================
    //        1. SLIDER DE VOLUMEN
    // ==========================================
    public void CambiarVolumen(float valor)
    {
        if (!estaMuteado)
        {
            AudioListener.volume = valor;
        }

        PlayerPrefs.SetFloat("guardado_volumen", valor);
        PlayerPrefs.Save();
    }

    // ==========================================
    //        2. BOTÓN DE MUTEAR
    // ==========================================
    public void BotonMutear()
    {
        estaMuteado = !estaMuteado;

        if (estaMuteado)
        {
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = sliderVolumen != null ? sliderVolumen.value : 1f;
        }

        // Actualizamos el aspecto visual del botón inmediatamente
        ActualizarSpritesUI();

        PlayerPrefs.SetInt("guardado_mute", estaMuteado ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ==========================================
    //        3. BOTÓN DE PANTALLA COMPLETA
    // ==========================================
    public void BotonPantallaCompleta()
    {
        esFullscreen = !esFullscreen;
        Screen.fullScreen = esFullscreen;

        // Actualizamos el aspecto visual del botón inmediatamente
        ActualizarSpritesUI();

        PlayerPrefs.SetInt("guardado_fullscreen", esFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
    // ==========================================
    //        4. BOTÓN DE CERRAR SETTINGS
    // ==========================================
    public void BotonCerrar()
    {
        if (panelSettings != null)
        {
            panelSettings.SetActive(false); // Esto apaga el panel y lo oculta
        }
    }

    // ==========================================
    //        ACTUALIZACIÓN VISUAL DE SPRITES
    // ==========================================
    private void ActualizarSpritesUI()
    {
        // Cambiar la imagen del botón de Mute según su estado
        if (botonMuteImage != null)
        {
            botonMuteImage.sprite = estaMuteado ? spriteSonidoMuteado : spriteSonidoActivo;
        }

        // Cambiar la imagen del botón de Fullscreen según su estado
        if (botonFullscreenImage != null)
        {
            botonFullscreenImage.sprite = esFullscreen ? spritePantallaCompleta : spriteModoVentana;
        }
    }

    // ==========================================
    //        CARGAR DATOS GUARDADOS
    // ==========================================
    private void CargarOpciones()
    {
        // 1. Cargar Fullscreen (Esto sí lo recordamos)
        if (PlayerPrefs.HasKey("guardado_fullscreen"))
        {
            esFullscreen = PlayerPrefs.GetInt("guardado_fullscreen") == 1;
            Screen.fullScreen = esFullscreen;
        }
        else
        {
            esFullscreen = Screen.fullScreen;
        }

        // 2. FORZAR VOLUMEN AL INICIAR (Ignoramos lo que hubiera guardado)
        estaMuteado = false;       // Nos aseguramos de que no esté silenciado
        AudioListener.volume = 1f; // Ponemos el volumen interno al máximo (1)

        if (sliderVolumen != null)
        {
            sliderVolumen.value = 1f; // Movemos la barra visual al máximo también
        }

        // 3. Actualizamos las imágenes de los botones para que el altavoz se vea encendido
        ActualizarSpritesUI();
    }
}