using UnityEngine;

public class HeadSensor : MonoBehaviour
{
    [Header("Referencias")]
    public Boss boss;
    public Animator bossAnimator;

    [Header("Configuración de Rebote")]
    [SerializeField] private float fuerzaReboteVertical = 10f; // El salto hacia arriba
    [SerializeField] private float fuerzaReboteHorizontal = 3f; // El empujón hacia el lado

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
            return;

        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();

        // Solo si el jugador está cayendo
        if (rb != null && rb.linearVelocity.y < 0f)
        {
            // 1. Reproducir animación
            if (bossAnimator != null)
            {
                bossAnimator.SetTrigger("RDaño");
            }

            // 2. Quitar vida
            if (boss != null)
            {
                boss.RecibirGolpeEnCabeza();
            }

            // 3. Calcular la dirección del rebote lateral
            // Si la posición X del jugador es menor que la del sensor, está a la izquierda (empujamos a -1).
            // Si es mayor, está a la derecha (empujamos a 1).
            float direccionX = (col.transform.position.x < transform.position.x) ? -1f : 1f;

            // 4. Aplicar el rebote estilo Mario con el empujón lateral
            rb.linearVelocity = new Vector2(fuerzaReboteHorizontal * direccionX, fuerzaReboteVertical);
        }
    }
}