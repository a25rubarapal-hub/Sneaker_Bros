using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] private string triggerRecibir = "RDaño";
    [SerializeField] private string triggerReaccionar = "Reacionar";

    [Header("Vida y Muerte")]
    [SerializeField] private int vidaMaxima = 50;
    [SerializeField] private GameObject prefabMuerte;
    private int vidaActual;

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 3f;

    [Header("Duración de Estados")]
    [SerializeField] private float duracionAtaque = 0.8f;
    [SerializeField] private float duracionRecibir = 0.3f;
    [SerializeField] private float duracionReaccionar = 0.3f;
    [SerializeField] private float duracionIdle = 1f;

    [Header("Ataque y Daño")]
    [SerializeField] private int danioAtaque = 1;
    [SerializeField] private Collider2D hitboxDanio; // Hitbox que hace daño
    [SerializeField] private Collider2D radarAtaque; // <- NUEVA: Hitbox que detecta cuándo atacar
    [SerializeField] private float cooldownDanio = 2f;

    [Header("Radares")]
    [SerializeField] private string tagJugador = "Player";
    [SerializeField] private string tagPared = "Pared";
    [SerializeField] private float distanciaRayoPared = 1f;

    // ── Referencias ────────────────────────────────────────
    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D bossCollider;
    private float timerEstado = 0f;
    private float timerCooldownDanio = 0f;
    private bool movingRight = true;
    private bool caminandoActualmente = false;

    // ── Estado interno Radares ─────────────────────────────
    private Transform transformJugador;
    private bool persiguiendoJugador = false;
    private bool jugadorCercaParaAtacar = false;

    // ─────────────────────────────────────────────────────────
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<BoxCollider2D>();
        vidaActual = vidaMaxima;

        bossCollider.isTrigger = false;

        if (hitboxDanio != null) hitboxDanio.isTrigger = true;
        if (radarAtaque != null) radarAtaque.isTrigger = true; // Nos aseguramos de que sea trigger

        CambiarEstado(Estado.Walk);
    }

    void Update()
    {
        if (estadoActual == Estado.Dead) return;

        if (timerEstado > 0f) timerEstado -= Time.deltaTime;
        if (timerCooldownDanio > 0f) timerCooldownDanio -= Time.deltaTime;

        DetectarParedes();
        DetectarSiPuedeEmpezarAtaque(); // Ahora usa el nuevo Radar
        ActualizarLogicaEstado();

        AplicarDanioContinuo();

        if (caminandoActualmente) Mover();
    }

    // ─────────────────────────────────────────────────────────
    // SISTEMA DE DAÑO CONTINUO
    // ─────────────────────────────────────────────────────────
    void AplicarDanioContinuo()
    {
        if (hitboxDanio == null || timerCooldownDanio > 0f) return;

        List<Collider2D> collidersEnRango = new List<Collider2D>();
        hitboxDanio.Overlap(ContactFilter2D.noFilter, collidersEnRango);

        foreach (Collider2D col in collidersEnRango)
        {
            if (col.CompareTag(tagJugador))
            {
                PlayerHealth player = col.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(danioAtaque);
                    timerCooldownDanio = cooldownDanio;
                    Debug.Log($"[Boss] ¡Te di un golpe! Esperando {cooldownDanio} segundos para el próximo...");
                    break;
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    // DETECTAR PARA INICIAR ANIMACIÓN (NUEVO SISTEMA POR HITBOX)
    // ─────────────────────────────────────────────────────────
    void DetectarSiPuedeEmpezarAtaque()
    {
        if (radarAtaque == null)
        {
            jugadorCercaParaAtacar = false;
            return;
        }

        // Escaneamos quién está dentro de la Hitbox de Detección (Radar)
        List<Collider2D> collidersEnRango = new List<Collider2D>();
        radarAtaque.Overlap(ContactFilter2D.noFilter, collidersEnRango);

        jugadorCercaParaAtacar = false; // Por defecto asumimos que no está

        foreach (Collider2D col in collidersEnRango)
        {
            if (col.CompareTag(tagJugador))
            {
                jugadorCercaParaAtacar = true; // ¡Lo encontramos!
                break;
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
    }

    // ─────────────────────────────────────────────────────────
    // RADAR DEL JUGADOR (Para la persecución general)
    // ─────────────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            transformJugador = collision.transform;
            persiguiendoJugador = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            persiguiendoJugador = false;
            transformJugador = null;
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
                if (jugadorCercaParaAtacar)
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
                if (timerEstado <= 0f)
                {
                    if (jugadorCercaParaAtacar)
                    {
                        animator.Play("Attack", -1, 0f);
                        timerEstado = duracionAtaque;
                    }
                    else
                    {
                        CambiarEstado(Estado.Idle);
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
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                SetTrigger(triggerIdle);
                timerEstado = duracionIdle;
                break;

            case Estado.Walk:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                caminandoActualmente = true;
                SetBool(paramCaminar, true);
                timerEstado = Random.Range(2f, 4f);
                break;

            case Estado.Attack:
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                animator.Play("Attack");
                timerEstado = duracionAtaque;
                break;

            case Estado.Hit:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                SetTrigger(triggerRecibir);
                timerEstado = duracionRecibir;
                break;

            case Estado.React:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                SetTrigger(triggerReaccionar);
                timerEstado = duracionReaccionar;
                break;

            case Estado.Dead:
                break;
        }
    }

    public void Morir()
    {
        estadoActual = Estado.Dead;

        if (prefabMuerte != null)
        {
            GameObject clonMuerte = Instantiate(prefabMuerte, transform.position, transform.rotation);
            clonMuerte.transform.localScale = transform.localScale;
            clonMuerte.SetActive(true);
        }

        Destroy(gameObject);
    }

    void SetBool(string param, bool value)
    {
        if (animator) animator.SetBool(param, value);
    }

    void SetTrigger(string param)
    {
        if (animator) animator.SetTrigger(param);
    }

    public Estado ObtenerEstadoActual() => estadoActual;
    public int ObtenerVida() => vidaActual;
    public int ObtenerVidaMaxima() => vidaMaxima;
}