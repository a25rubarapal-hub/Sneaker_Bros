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
    [SerializeField] private float duracionRecibir = 0.5f;
    [SerializeField] private float duracionReaccionar = 0.3f;
    [SerializeField] private float duracionIdle = 1f;

    [Header("Ataque y Daño")]
    [SerializeField] private int danioAtaque = 1;
    [SerializeField] private Collider2D hitboxDanio;
    [SerializeField] private float cooldownDanio = 2f;
    [SerializeField] private float distanciaParaAtacar = 1.5f;

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

        if (hitboxDanio != null)
        {
            hitboxDanio.isTrigger = true;
        }

        CambiarEstado(Estado.Walk);
    }

    void Update()
    {
        if (estadoActual == Estado.Dead) return;

        if (timerEstado > 0f) timerEstado -= Time.deltaTime;
        if (timerCooldownDanio > 0f) timerCooldownDanio -= Time.deltaTime;

        DetectarParedes();
        DetectarSiPuedeEmpezarAtaque();
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
        hitboxDanio.Overlap(new ContactFilter2D().NoFilter(), collidersEnRango);

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
    // DETECTAR PARA INICIAR ANIMACIÓN
    // ─────────────────────────────────────────────────────────
    void DetectarSiPuedeEmpezarAtaque()
    {
        if (transformJugador == null)
        {
            jugadorCercaParaAtacar = false;
            return;
        }

        float distancia = Vector2.Distance(transform.position, transformJugador.position);
        jugadorCercaParaAtacar = (distancia <= distanciaParaAtacar);
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
    // RADAR DEL JUGADOR
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
            jugadorCercaParaAtacar = false;
            transformJugador = null;
        }
    }

    // ─────────────────────────────────────────────────────────
    // MOVIMIENTO
    // ──────────────────────────────────────────���──────────────
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
            // Force reset del estado para asegurar que la animación se dispare
            estadoActual = Estado.Idle;
            CambiarEstado(Estado.Hit);
            Debug.Log($"[Boss] Animación de daño disparada. Timer: {timerEstado}");
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
                        SetTrigger(triggerAtacar);
                        timerEstado = duracionAtaque;
                    }
                    else
                    {
                        CambiarEstado(Estado.Idle);
                    }
                }
                break;

            case Estado.Hit:
                // El estado Hit se mantiene hasta que el timer se agote
                if (timerEstado <= 0f)
                {
                    // Si el jugador sigue cerca, vuelve a atacar; si no, vuelve a caminar
                    if (jugadorCercaParaAtacar)
                    {
                        CambiarEstado(Estado.Walk);
                    }
                    else
                    {
                        CambiarEstado(Estado.Walk);
                    }
                }
                break;

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
                SetTrigger(triggerAtacar);
                timerEstado = duracionAtaque;
                break;

            case Estado.Hit:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                SetTrigger(triggerRecibir);
                timerEstado = duracionRecibir;
                Debug.Log($"[Boss] Estado Hit activado. Timer establecido a: {timerEstado}");
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

        // 1. Invocamos al Clon de Muerte
        if (prefabMuerte != null)
        {
            GameObject clonMuerte = Instantiate(prefabMuerte, transform.position, transform.rotation);
            clonMuerte.transform.localScale = transform.localScale;

            // Fuerza al clon a mostrarse sin importar cómo estaba el original
            clonMuerte.SetActive(true);
        }

        // 2. Destruimos al Boss original
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