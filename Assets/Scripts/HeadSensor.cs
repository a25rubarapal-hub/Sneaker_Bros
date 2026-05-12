using UnityEngine;

public class HeadSensor : MonoBehaviour
{
    [Header("Referencias")]
    public Boss boss;
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string triggerReaccionar = "Reaccionar";

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
        if (boss == null) return;

        Rigidbody2D rbJugador = col.GetComponent<Rigidbody2D>();
        Movimiento mov = col.GetComponent<Movimiento>();

        bool jugadorCayendo = rbJugador != null && rbJugador.linearVelocity.y <= 0.1f;
        bool jugadorArriba = col.transform.position.y > transform.position.y - 0.2f;

        if (jugadorCayendo && jugadorArriba)
        {
            boss.RecibirGolpeEnCabeza();

            if (bossAnimator != null)
                bossAnimator.SetTrigger(triggerReaccionar);

            if (mov != null)
            {
                float dirX = (col.transform.position.x < transform.position.x) ? -1f : 1f;
                // Como pasamos un Vector2 limpio (UnityEngine.Vector2), ya no dará el error CS1501
                mov.AplicarRebote(new Vector2(fuerzaReboteHorizontal * dirX, fuerzaReboteVertical), tiempoBloqueo);
            }
        }
    }
}