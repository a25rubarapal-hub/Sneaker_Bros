using UnityEngine;
using System.Collections.Generic; // Necesario para usar List<>

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class Boss : MonoBehaviour
{
    public enum Estado { Idle, Walk, Attack, Hit, React, Dead }
    private Estado estadoActual = Estado.Idle;

    [Header("Parámetros de Animator")]
    [SerializeField] private string paramCaminar = "Caminar";
    [SerializeField] private string triggerAtacar = "Atacar";
    [SerializeField] private string triggerIdle = "Idle";
    [SerializeField] private string triggerRecibir = "Recibir";
    [SerializeField] private string triggerReaccionar = "Reaccionar";

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 50;
    private int vidaActual;

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 3f;

    [Header("Duración de Estados")]
    [SerializeField] private float duracionAtaque = 0.8f; // IMPORTANTE: Debe durar lo mismo que tu animación de ataque
    [SerializeField] private float duracionRecibir = 0.3f;
    [SerializeField] private float duracionReaccionar = 0.3f;
    [SerializeField] private float duracionIdle = 1f;

    [Header("Ataque")]
    [SerializeField] private int danioAtaque = 1;
    [SerializeField] private float distanciaDeteccionAtaque = 1.5f;
    [SerializeField] private Collider2D hitboxAtaque; // <- Aquí asignas la Hitbox hija en el Inspector

    [Header("Radares")]
    [SerializeField] private string tagJugador = "Player";
    [SerializeField] private string tagPared = "Pared";
    [SerializeField] private float distanciaRayoPared = 1f;

    // ── Referencias ────────────────────────────────────────
    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D bossCollider; // Este es el collider principal (físico)
    private float timerEstado = 0f;
    private bool movingRight = true;
    private bool caminandoActualmente = false;

    // ── Estado interno Radares ─────────────────────────────
    private Transform transformJugador;
    private bool persiguiendoJugador = false;
    private bool jugadorEnRangoAtaque = false;

    // ─────────────────────────────────────────────────────────
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<BoxCollider2D>();
        vidaActual = vidaMaxima;

        // CORRECCIÓN: Evita que el boss se caiga al vacío.
        bossCollider.isTrigger = false;

        if (hitboxAtaque != null)
        {
            hitboxAtaque.isTrigger = true;
        }

        CambiarEstado(Estado.Walk);
    }

    void Update()
    {
        if (estadoActual == Estado.Dead) return;

        if (timerEstado > 0f) timerEstado -= Time.deltaTime;

        DetectarParedes();
        ActualizarLogicaEstado();
        DetectarJugadorEnRangoAtaque();

        if (caminandoActualmente) Mover();
    }

    // ─────────────────────────────────────────────────────────
    // DETECTAR JUGADOR EN RANGO DE ATAQUE
    // ─────────────────────────────────────────────────────────

    void DetectarJugadorEnRangoAtaque()
    {
        if (hitboxAtaque == null || transformJugador == null)
        {
            jugadorEnRangoAtaque = false;
            return;
        }

        List<Collider2D> collidersEnRango = new List<Collider2D>();
        hitboxAtaque.Overlap(new ContactFilter2D().NoFilter(), collidersEnRango);

        jugadorEnRangoAtaque = collidersEnRango.Exists(col => col.CompareTag(tagJugador));
    }

    // ─────────────────────────────────────────────────────────
    // APLICAR DAÑO (Justo al terminar la animación)
    // ─────────────────────────────────────────────────────────

    void AplicarDanioFinalAnimacion()
    {
        if (hitboxAtaque == null) return;

        List<Collider2D> collidersEnRango = new List<Collider2D>();
        hitboxAtaque.Overlap(new ContactFilter2D().NoFilter(), collidersEnRango);

        foreach (Collider2D col in collidersEnRango)
        {
            if (col.CompareTag(tagJugador))
            {
                // Mismo sistema que tienes en MovimientoNPC.cs
                PlayerHealth player = col.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(danioAtaque);
                    Debug.Log($"[Boss] ¡DAÑO APLICADO! {danioAtaque} al jugador al terminar el ataque.");
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    // RADAR DE PAREDES
    // ─────────────────────────────────────────────────────────
    void DetectarParedes()
    {
        float dir = movingRight ? 1f : -1f;
        Vector2 origen = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.right * dir, distanciaRayoPared);
        Debug.DrawRay(origen, Vector2.right * dir * distanciaRayoPared,
                      (hit.collider != null && hit.collider.CompareTag(tagPared)) ? Color.red : Color.green);
    }

    // ─────────────────────────────────────────────────────────
    // RADAR DEL JUGADOR — Necesita un segundo Collider "Is Trigger" en Unity
    // ─────────────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            transformJugador = collision.transform;
            persiguiendoJugador = true;
            Debug.Log("[Boss] Jugador detectado");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            persiguiendoJugador = false;
            jugadorEnRangoAtaque = false;
            transformJugador = null;
            Debug.Log("[Boss] Jugador salió del rango");
        }
    }

    // ─────────────────────────────────────────────────────────
    // MOVIMIENTO
    // ─────────────────────────────────────────────────────────
    void Mover()
    {
        if (persiguiendoJugador && transformJugador != null)
            movingRight = transformJugador.position.x > transform.position.x;

        float dir = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * dir * velocidadMovimiento * Time.deltaTime);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!persiguiendoJugador && collision.gameObject.CompareTag(tagPared))
            movingRight = !movingRight;
    }

    // ─────────────────────────────────────────────────────────
    // RECIBIR GOLPE
    // ─────────────────────────────────────────────────────────
    public void RecibirGolpeEnCabeza()
    {
        if (estadoActual == Estado.Dead) return;

        vidaActual -= 1;
        Debug.Log($"Boss golpeado. Vida: {vidaActual}/{vidaMaxima}");

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
        else
        {
            CambiarEstado(Estado.Hit);
        }
    }

    // ─────────────────────────────────────────────────────────
    // ESTADOS
    // ─────────────────────────────────────────────────────────
    void ActualizarLogicaEstado()
    {
        switch (estadoActual)
        {
            case Estado.Idle:
                if (timerEstado <= 0f)
                {
                    CambiarEstado(Estado.Walk);
                }
                break;

            case Estado.Walk:
                if (jugadorEnRangoAtaque)
                {
                    CambiarEstado(Estado.Attack);
                    break;
                }
                if (timerEstado <= 0f)
                {
                    CambiarEstado(Estado.Idle);
                }
                break;

            case Estado.Attack:
                // Cuando el temporizador llega a 0, significa que la animación de ataque terminó
                if (timerEstado <= 0f)
                {
                    // 1. Verificamos si el jugador sigue ahí para aplicarle el daño
                    AplicarDanioFinalAnimacion();

                    // 2. Volvemos a detectar si el jugador sigue en rango para atacar de nuevo
                    DetectarJugadorEnRangoAtaque();

                    if (jugadorEnRangoAtaque)
                    {
                        SetTrigger(triggerAtacar);
                        timerEstado = duracionAtaque; // Reiniciamos el tiempo para el nuevo ataque
                    }
                    else
                    {
                        CambiarEstado(Estado.Idle); // Si se alejó, volvemos a Idle
                    }
                }
                break;

            case Estado.Hit:
            case Estado.React:
                if (timerEstado <= 0f)
                {
                    CambiarEstado(Estado.Walk);
                }
                break;
        }
    }

    void CambiarEstado(Estado nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;

        estadoActual = nuevoEstado;
        caminandoActualmente = false;
        SetBool(paramCaminar, false);

        switch (estadoActual)
        {
            case Estado.Idle:
                SetTrigger(triggerIdle);
                timerEstado = duracionIdle;
                Debug.Log("[Boss] Estado: IDLE");
                break;

            case Estado.Walk:
                caminandoActualmente = true;
                SetBool(paramCaminar, true);
                timerEstado = Random.Range(2f, 4f);
                Debug.Log("[Boss] Estado: WALK");
                break;

            case Estado.Attack:
                SetTrigger(triggerAtacar);
                timerEstado = duracionAtaque; // Arranca el contador que dura lo que la animación
                Debug.Log("[Boss] Estado: ATTACK");
                break;

            case Estado.Hit:
                SetTrigger(triggerRecibir);
                timerEstado = duracionRecibir;
                Debug.Log("[Boss] Estado: HIT");
                break;

            case Estado.React:
                SetTrigger(triggerReaccionar);
                timerEstado = duracionReaccionar;
                Debug.Log("[Boss] Estado: REACT");
                break;

            case Estado.Dead:
                Debug.Log("[Boss] Estado: DEAD");
                break;
        }
    }

    public void Morir()
    {
        estadoActual = Estado.Dead;
        Destroy(gameObject);
    }

    void SetBool(string param, bool value)
    {
        if (animator)
            animator.SetBool(param, value);
    }

    void SetTrigger(string param)
    {
        if (animator)
            animator.SetTrigger(param);
    }

    public Estado ObtenerEstadoActual() => estadoActual;
    public int ObtenerVida() => vidaActual;
    public int ObtenerVidaMaxima() => vidaMaxima;
}