using UnityEngine;

public class HeadSensor : MonoBehaviour
{
    [Header("Referencias")]
    public Boss boss;
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string triggerReacionar = "Reacionar";

    [Header("Rebote Ofensivo (Salto)")]
    [SerializeField] private float fuerzaReboteVertical = 10f;
    [SerializeField] private float fuerzaReboteHorizontal = 3f;
    [SerializeField] private float tiempoBloqueo = 0.2f;

    private void Awake()
    {
        if (bossAnimator == null && boss != null)
            bossAnimator = boss.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (boss == null)
        {
            Debug.LogError("[HeadSensor] Boss no asignado en el Inspector.");
            return;
        }

        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        Movimiento mov = col.GetComponent<Movimiento>();

        // Usamos 'velocity' puro para evitar errores de compilación
        bool jugadorCayendo = false;
        if (rb != null)
        {
            jugadorCayendo = rb.linearVelocity.y <= 0.1f;
        }

        bool jugadorArriba = col.transform.position.y > transform.position.y - 0.2f;

        string vyAproximada = (rb != null) ? rb.linearVelocity.y.ToString("F2") : "null";
        Debug.Log("[HeadSensor] Contacto con Player | cayendo=" + jugadorCayendo + " (vy=" + vyAproximada + ") | arriba=" + jugadorArriba);

        if (jugadorCayendo && jugadorArriba)
        {
            Debug.Log("[HeadSensor] Condiciones OK → llamando RecibirGolpeEnCabeza()");
            boss.RecibirGolpeEnCabeza();

            if (bossAnimator != null)
                bossAnimator.SetTrigger(triggerReacionar);

            if (mov != null)
            {
                float dirX = (col.transform.position.x < transform.position.x) ? -1f : 1f;
                Vector2 fuerza = new Vector2(fuerzaReboteHorizontal * dirX, fuerzaReboteVertical);

                mov.AplicarRebote(fuerza, tiempoBloqueo);
            }
            else
            {
                Debug.LogWarning("[HeadSensor] El jugador no tiene componente Movimiento.");
            }
        }
        else
        {
            Debug.Log("[HeadSensor] Condiciones NO cumplidas → no se aplica golpe.");
        }
    }
}