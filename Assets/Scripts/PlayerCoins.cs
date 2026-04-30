using UnityEngine;
using TMPro;

public class PlayerCoins : MonoBehaviour
{
    public int monedas = 0;

    public TextMeshProUGUI monedasText;

    public PlayerHealth playerHealth;
    public PlayerScore playerScore;

    [Header("Sonido")]
    public AudioClip sonido10Monedas;
    [Range(0f, 1f)] public float volumen = 1f;

    public void SumarMoneda()
    {
        monedas++;

        // 💯 puntos
        if (playerScore != null)
            playerScore.SumarPuntos(100);

        // ❤️ vida + sonido cada 10 monedas
        if (monedas % 10 == 0)
        {
            if (playerHealth != null)
                playerHealth.Heal(1);

            if (sonido10Monedas != null)
            {
                AudioSource audioTemp = gameObject.AddComponent<AudioSource>();
                audioTemp.clip = sonido10Monedas;
                audioTemp.volume = volumen;
                audioTemp.spatialBlend = 0f;
                audioTemp.Play();
                Destroy(audioTemp, sonido10Monedas.length);
            }
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (monedasText != null)
        {
            monedasText.text = monedas.ToString();
        }
        else
        {
            Debug.LogWarning("MonedasText NO asignado en PlayerCoins");
        }
    }
}