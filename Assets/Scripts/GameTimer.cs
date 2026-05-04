using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float tiempoInicial = 60f;
    public float tiempoActual;

    public bool timerActivo = true;

    public TMP_Text timerText;

    void Start()
    {
        tiempoActual = tiempoInicial;
    }

    void Update()
    {
        if (!timerActivo) return;

        tiempoActual -= Time.deltaTime;

        if (tiempoActual <= 0)
        {
            tiempoActual = 0;
            timerActivo = false;

            Debug.Log("Se acabó el tiempo");
            // aquí luego puedes llamar a Game Over o FinishLine
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(tiempoActual).ToString();
        }
    }

    public float GetTiempoRestante()
    {
        return tiempoActual;
    }
}