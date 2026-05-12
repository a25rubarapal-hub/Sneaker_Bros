using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Boss : MonoBehaviour
{
    public enum Estado
    {
        Idle,
        Walk,
        Attack,
        Hit,
        React,
        Dead
    }

    private Estado estadoActual = Estado.Idle;

    [Header("Vida")]
    public int vidaMaxima = 50;
    private int vidaActual;

    [Header("Muerte")]
    public GameObject prefabMuerte;

    [Header("Movimiento")]
    public float velocidadMovimiento = 3f;

    [Header("Duraciones")]
    public float duracionIdle = 1f;
    public float duracionAtaque = 0.8f;
    public float duracionHit = 0.3f;
    public float duracionReact = 0.3f;

    [Header("Ataque")]
    public int danioAtaque = 1;
    public float cooldownDanio = 2f;
    public float cooldownEntreAtaques = 1.2f;

    [Header("Colliders")]
    public Collider2D hitboxDanio;
    public Collider2D radarAtaque;

    [Header("Tags")]
    public string tagJugador = "Player";
    public string tagPared = "Pared";

    [Header("Animator Parameters")]
    public string paramIdle = "idle";
    public string paramWalk = "Walk";
    public string paramAttack = "Atacar";
    public string paramHit = "RDaño";
    public string paramReact = "Reaccionar";

    [Header("Nombre del State de Ataque")]
    public string nombreStateAtaque = "Attack";

    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D bossCollider;

    private Transform jugadorTransform;
    private Collider2D jugadorCollider;

    private bool movingRight = true;
    private bool caminando = false;
    private bool persiguiendoJugador = false;

    private float timerEstado = 0f;
    private float timerCooldownDanio = 0f;
    private float timerCooldownAtaque = 0f;

    [HideInInspector]
    public bool jugadorCercaParaAtacar = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<BoxCollider2D>();

        vidaActual = vidaMaxima;

        bossCollider.isTrigger = false;

        if (hitboxDanio != null)
            hitboxDanio.isTrigger = true;

        if (radarAtaque != null)
            radarAtaque.isTrigger = true;

        BuscarJugador();

        DebugAnimator();

        CambiarEstado(Estado.Walk);
    }

    void Update()
    {
        if (estadoActual == Estado.Dead)
            return;

        if (jugadorTransform == null || jugadorCollider == null)
            BuscarJugador();

        if (timerEstado > 0f)
            timerEstado -= Time.deltaTime;

        if (timerCooldownDanio > 0f)
            timerCooldownDanio -= Time.deltaTime;

        if (timerCooldownAtaque > 0f)
            timerCooldownAtaque -= Time.deltaTime;

        DetectarJugador();

        ActualizarEstados();

        AplicarDanio();
    }

    void FixedUpdate()
    {
        if (estadoActual == Estado.Dead)
            return;

        if (caminando)
        {
            Mover();
        }
        else if (estadoActual != Estado.Attack)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void BuscarJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag(tagJugador);

        if (jugador != null)
        {
            jugadorTransform = jugador.transform;
            jugadorCollider = jugador.GetComponent<Collider2D>();
        }
    }

    void DetectarJugador()
    {
        jugadorCercaParaAtacar = false;

        if (radarAtaque == null || jugadorCollider == null)
            return;

        if (radarAtaque.IsTouching(jugadorCollider))
        {
            jugadorCercaParaAtacar = true;
            persiguiendoJugador = true;
        }
        else if (jugadorTransform != null)
        {
            float distancia =
                Vector2.Distance(
                    transform.position,
                    jugadorTransform.position
                );

            persiguiendoJugador = distancia < 5f;
        }
    }

    void ActualizarEstados()
    {
        switch (estadoActual)
        {
            case Estado.Idle:

                if (jugadorCercaParaAtacar &&
                    timerCooldownAtaque <= 0f)
                {
                    CambiarEstado(Estado.Attack);
                }
                else if (timerEstado <= 0f)
                {
                    CambiarEstado(Estado.Walk);
                }

                break;

            case Estado.Walk:

                if (jugadorCercaParaAtacar &&
                    timerCooldownAtaque <= 0f)
                {
                    CambiarEstado(Estado.Attack);
                }
                else if (timerEstado <= 0f)
                {
                    CambiarEstado(Estado.Idle);
                }

                break;

            case Estado.Attack:

                if (AnimacionAtaqueTerminada() ||
                    timerEstado <= 0f)
                {
                    timerCooldownAtaque =
                        cooldownEntreAtaques;

                    CambiarEstado(Estado.Idle);
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
        if (estadoActual == nuevoEstado)
            return;

        estadoActual = nuevoEstado;

        caminando = false;

        animator.SetBool(paramIdle, false);
        animator.SetBool(paramWalk, false);
        animator.SetBool(paramAttack, false);

        switch (estadoActual)
        {
            case Estado.Idle:

                rb.constraints =
                    RigidbodyConstraints2D.FreezeRotation;

                animator.SetBool(paramIdle, true);

                timerEstado = duracionIdle;

                Debug.Log("Estado: IDLE");

                break;

            case Estado.Walk:

                rb.constraints =
                    RigidbodyConstraints2D.FreezeRotation;

                caminando = true;

                animator.SetBool(paramWalk, true);

                timerEstado = Random.Range(2f, 4f);

                Debug.Log("Estado: WALK");

                break;

            case Estado.Attack:

                rb.constraints =
                    RigidbodyConstraints2D.FreezeRotation |
                    RigidbodyConstraints2D.FreezePositionX;

                rb.linearVelocity =
                    new Vector2(0f, rb.linearVelocity.y);

                animator.SetBool(paramAttack, true);

                timerEstado = duracionAtaque;

                Debug.Log("Estado: ATTACK");

                break;

            case Estado.Hit:

                rb.constraints =
                    RigidbodyConstraints2D.FreezeRotation;

                animator.ResetTrigger(paramHit);
                animator.SetTrigger(paramHit);

                timerEstado = duracionHit;

                Debug.Log("Estado: HIT");

                break;

            case Estado.React:

                rb.constraints =
                    RigidbodyConstraints2D.FreezeRotation;

                animator.ResetTrigger(paramReact);
                animator.SetTrigger(paramReact);

                timerEstado = duracionReact;

                Debug.Log("Estado: REACT");

                break;

            case Estado.Dead:

                Debug.Log("Estado: DEAD");

                break;
        }
    }

    bool AnimacionAtaqueTerminada()
    {
        AnimatorStateInfo info =
            animator.GetCurrentAnimatorStateInfo(0);

        return info.IsName(nombreStateAtaque) &&
               info.normalizedTime >= 1f;
    }

    void Mover()
    {
        if (persiguiendoJugador &&
            jugadorTransform != null)
        {
            movingRight =
                jugadorTransform.position.x >
                transform.position.x;
        }

        float dir = movingRight ? 1f : -1f;

        rb.linearVelocity =
            new Vector2(
                velocidadMovimiento * dir,
                rb.linearVelocity.y
            );

        Vector3 escala = transform.localScale;

        escala.x = Mathf.Abs(escala.x) * dir;

        transform.localScale = escala;
    }

    void AplicarDanio()
    {
        if (hitboxDanio == null)
            return;

        if (jugadorCollider == null)
            return;

        if (timerCooldownDanio > 0f)
            return;

        if (hitboxDanio.IsTouching(jugadorCollider))
        {
            PlayerHealth player =
                jugadorCollider.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(danioAtaque);

                timerCooldownDanio = cooldownDanio;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (persiguiendoJugador)
            return;

        if (!collision.gameObject.CompareTag(tagPared))
            return;

        foreach (ContactPoint2D contacto in collision.contacts)
        {
            if (contacto.normal.x > 0.5f && !movingRight)
            {
                movingRight = true;
                break;
            }
            else if (contacto.normal.x < -0.5f && movingRight)
            {
                movingRight = false;
                break;
            }
        }
    }

    public void RecibirGolpeEnCabeza()
    {
        if (estadoActual == Estado.Dead)
            return;

        vidaActual--;

        Debug.Log("Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            CambiarEstado(Estado.Hit);
        }
    }

    public void Morir()
    {
        estadoActual = Estado.Dead;

        if (prefabMuerte != null)
        {
            GameObject muerte =
                Instantiate(
                    prefabMuerte,
                    transform.position,
                    transform.rotation
                );

            muerte.transform.localScale =
                transform.localScale;

            muerte.SetActive(true);
        }

        Destroy(gameObject);
    }

    void DebugAnimator()
    {
        Debug.Log("===== PARÁMETROS ANIMATOR =====");

        foreach (AnimatorControllerParameter param
                 in animator.parameters)
        {
            Debug.Log(
                "Nombre: " +
                param.name +
                " | Tipo: " +
                param.type
            );
        }

        Debug.Log("================================");
    }
}