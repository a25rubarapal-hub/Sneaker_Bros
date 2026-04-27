using UnityEngine;

public class MonedaPick : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCoins coins = collision.GetComponent<PlayerCoins>();

            if (coins != null)
            {
                coins.SumarMoneda();
            }

            Destroy(gameObject);
        }
    }
}