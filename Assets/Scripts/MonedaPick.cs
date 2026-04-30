using UnityEngine;

public class MonedaPick : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip sonidoMoneda;
    [Range(0f, 1f)] public float volumen = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCoins coins = collision.GetComponent<PlayerCoins>();

            if (coins != null)
            {
                coins.SumarMoneda();
            }

            if (sonidoMoneda != null)
            {
                AudioSource.PlayClipAtPoint(sonidoMoneda, transform.position, volumen);
            }

            Destroy(gameObject);
        }
    }
}