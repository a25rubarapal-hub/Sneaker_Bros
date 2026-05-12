<<<<<<< Updated upstream
using UnityEngine;
=======
﻿using UnityEngine;
>>>>>>> Stashed changes

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

        bool jugadorCayendo = rb != null && rb.linearVelocity.y <= 0.1f;
        bool jugadorArriba = col.transform.position.y > transform.position.y - 0.2f;

        Debug.Log($"[HeadSensor] Contacto con Player | cayendo={jugadorCayendo} (vy={rb?.linearVelocity.y:F2}) | arriba={jugadorArriba}");

        if (jugadorCayendo && jugadorArriba)
        {
            Debug.Log("[HeadSensor] Condiciones OK → llamando RecibirGolpeEnCabeza()");
            boss.RecibirGolpeEnCabeza();
<<<<<<< Updated upstream

            if (bossAnimator != null)
=======
if (bossAnimator != null)
>>>>>>> Stashed changes
                bossAnimator.SetTrigger(triggerReacionar);

            if (mov != null)
            {
                float dirX = (col.transform.position.x < transform.position.x) ? -1f : 1f;
                mov.AplicarRebote(new Vector2(fuerzaReboteHorizontal * dirX, fuerzaReboteVertical), tiempoBloqueo);
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