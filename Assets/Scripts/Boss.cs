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
    [SerializeField] private string paramAtacar = "Atacar";
    [SerializeField] private string paramIdle = "Idle";
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
    [SerializeField] private Collider2D hitboxDanio;
    [SerializeField] private Collider2D radarAtaque;
    [SerializeField] private float cooldownDanio = 2f;

    [Header("Cooldown entre ataques")]
    [SerializeField] private float cooldownEntreAtaques = 1.2f;
    private float timerCooldownAtaque = 0f;

    [Header("Radares")]
    [SerializeField] private string tagJugador = "Player";
    [SerializeField] private string tagPared = "Pared";

    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D bossCollider;
    private float timerEstado = 0f;
    private float timerCooldownDanio = 0f;
    private bool movingRight = true;
    private bool caminandoActualmente = false;

    private Transform transformJugador;
    private Collider2D colliderJugador;
    private bool persiguiendoJugador = false;

    [HideInInspector] public bool jugadorCercaParaAtacar = false;

    private const string ANIM_STATE_ATTACK = "Atacar";

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<BoxCollider2D>();
        vidaActual = vidaMaxima;

        bossCollider.isTrigger = false;
        if (hitboxDanio != null) hitboxDanio.isTrigger = true;
        if (radarAtaque != null) radarAtaque.isTrigger = true;

        BuscarJugador();
        CambiarEstado(Estado.Walk);
    }

    void BuscarJugador()
    {
        GameObject jugadorObj = GameObject.FindGameObjectWithTag(tagJugador);
        if (jugadorObj != null)
        {
            transformJugador = jugadorObj.transform;
            colliderJugador = jugadorObj.GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        if (estadoActual == Estado.Dead) return;

        if (transformJugador == null || colliderJugador == null) BuscarJugador();

        if (timerEstado > 0f) timerEstado -= Time.deltaTime;
        if (timerCooldownDanio > 0f) timerCooldownDanio -= Time.deltaTime;
        if (timerCooldownAtaque > 0f) timerCooldownAtaque -= Time.deltaTime;

        DetectarSiPuedeEmpezarAtaque();
        ActualizarLogicaEstado();

        if (estadoActual == Estado.Attack)
        {
            SetBool(paramIdle, false);
            SetBool(paramCaminar, false);
            SetBool(paramAtacar, true);
        }
        AplicarDanioContinuo();

        if (caminandoActualmente) Mover();
    }

    void AplicarDanioContinuo()
    {
        if (hitboxDanio == null || colliderJugador == null || timerCooldownDanio > 0f) return;

        if (hitboxDanio.bounds.Intersects(colliderJugador.bounds))
        {
            PlayerHealth player = colliderJugador.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(danioAtaque, transform);
                timerCooldownDanio = cooldownDanio;
            }
        }
    }

    void DetectarSiPuedeEmpezarAtaque()
    {
        jugadorCercaParaAtacar = false;
        if (radarAtaque == null || colliderJugador == null) return;

        if (radarAtaque.bounds.Intersects(colliderJugador.bounds))
        {
            jugadorCercaParaAtacar = true;
            persiguiendoJugador = true;
        }
        else if (transformJugador != null)
        {
            float distancia = Vector2.Distance(transform.position, transformJugador.position);
            persiguiendoJugador = (distancia < 5f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (persiguiendoJugador || !collision.gameObject.CompareTag(tagPared)) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.x > 0.5f && !movingRight) { movingRight = true; break; }
            else if (contact.normal.x < -0.5f && movingRight) { movingRight = false; break; }
        }
    }

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

    bool AnimacionAtaqueTerminada()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(ANIM_STATE_ATTACK) && info.normalizedTime >= 1f;
    }

    void ActualizarLogicaEstado()
    {
        switch (estadoActual)
        {
            case Estado.Idle:
            case Estado.Walk:
                if (jugadorCercaParaAtacar && timerCooldownAtaque <= 0f)
                    CambiarEstado(Estado.Attack);
                else if (timerEstado <= 0f)
                    CambiarEstado(estadoActual == Estado.Idle ? Estado.Walk : Estado.Idle);
                break;

            case Estado.Attack:
                bool clipTerminado = AnimacionAtaqueTerminada();
                bool timerAgotado = timerEstado <= 0f;

                if (clipTerminado || timerAgotado)
                {
                    timerCooldownAtaque = cooldownEntreAtaques;
                    CambiarEstado(Estado.Idle);
                }
                break;

            case Estado.Hit:
            case Estado.React:
                if (timerEstado <= 0f) CambiarEstado(Estado.Walk);
                break;
        }
    }

    void CambiarEstado(Estado nuevoEstado)
    {
        // Permitir interrumpir Attack, Hit o React con un nuevo Hit
        bool esInterrupcion = nuevoEstado == Estado.Hit &&
                              (estadoActual == Estado.Attack ||
                               estadoActual == Estado.Hit ||
                               estadoActual == Estado.React);

        if (estadoActual == nuevoEstado && !esInterrupcion) return;

        Debug.Log($"[Boss] CambiarEstado: {estadoActual} → {nuevoEstado}");
        estadoActual = nuevoEstado;
        caminandoActualmente = false;

        SetBool(paramIdle, false);
        SetBool(paramCaminar, false);
        SetBool(paramAtacar, false);

        switch (estadoActual)
        {
            case Estado.Idle:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                SetBool(paramIdle, true);
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
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                SetBool(paramAtacar, true);
                timerEstado = duracionAtaque;
                break;

            case Estado.Hit:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                // Resetea el trigger antes de lanzarlo para evitar colas acumuladas
                animator.ResetTrigger(triggerRecibir);
                SetTrigger(triggerRecibir);
                timerEstado = duracionRecibir;
                Debug.Log($"[Boss] Trigger '{triggerRecibir}' lanzado. Vida restante: {vidaActual}");
                break;

            case Estado.React:
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                animator.ResetTrigger(triggerReaccionar);
                SetTrigger(triggerReaccionar);
                timerEstado = duracionReaccionar;
                break;

            case Estado.Dead:
                break;
        }
    }

    public void RecibirGolpeEnCabeza()
    {
        if (estadoActual == Estado.Dead) return;

        vidaActual -= 1;
        Debug.Log($"[Boss] RecibirGolpeEnCabeza() | Vida: {vidaActual}/{vidaMaxima} | Estado actual: {estadoActual}");

        if (vidaActual <= 0) { vidaActual = 0; Morir(); }
        else { CambiarEstado(Estado.Hit); }
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

    void SetBool(string param, bool value) { if (animator) animator.SetBool(param, value); }
    void SetTrigger(string param) { if (animator) animator.SetTrigger(param); }

    public Estado ObtenerEstadoActual() => estadoActual;
    public int ObtenerVida() => vidaActual;
    public int ObtenerVidaMaxima() => vidaMaxima;
}