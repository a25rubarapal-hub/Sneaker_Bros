using UnityEngine;

public class MonedaPick : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScore score = collision.GetComponent<PlayerScore>();

            if (score != null)
            {
                score.SumarMoneda();
            }

            Destroy(gameObject);
        }
    }
}