using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float tiempoInicial = 60f;
    public float tiempoActual;

    public bool timerActivo = true;

    public TMP_Text timerText;

    public PlayerHealth playerHealth;

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

            // 💀 matar jugador
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(playerHealth.maxHealth);
            }
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