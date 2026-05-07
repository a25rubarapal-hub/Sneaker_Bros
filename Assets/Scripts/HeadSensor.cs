using UnityEngine;

public class HeadSensor : MonoBehaviour
{
    public Boss boss;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
            return;

        // Solo si el jugador está cayendo
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb != null && rb.linearVelocity.y < 0f)
        {
            boss.RecibirGolpeEnCabeza();
        }
    }
}
