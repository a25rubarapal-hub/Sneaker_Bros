using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class JefeMario : MonoBehaviour
{
    enum Estado { Patrulla, Reaccion, Persecucion, Ataque, Espera, Retroceso }
    Estado estadoActual = Estado.Patrulla;

    [Header("Movimiento del jefe")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;
    public float velocidadRetroceso = 3f;
    public float distanciaAtaque = 1.5f;
    public float distanciaDeteccionCercana = 1.0f;

    [Header("Salud e invencibilidad")]
    public int maxHealth = 67;
    public float tiempoInvencible = 1f;

    [Header("Knockback que recibe el JUGADOR")]
    public float knockbackSaltoX = 4.5f;
    public float knockbackSaltoY = 9f;
    public float knockbackGolpeX = 12f;
    public float knockbackGolpeY = 5f;
    public float duracionKnockback = 0.5f;
    public float giroSaltoCabeza = 420f;

    [Header("Recuperación tras golpes")]
    public float tiempoPausaTrasImpacto = 1f;
    public float multiplicadorRetrocesoGolpe = 1.8f;

    [Header("Daño por contacto continuo")]
    public float tiempoEntreGolpesContacto = 0.45f;

    [Header("Evasión al salto del jugador")]
    public float saltoJugadorMinY = 0.2f;
    public float distanciaEvasionSalto = 2.2f;
    public float velocidadEvasionSalto = 5.5f;
    public float duracionEvasionSalto = 0.28f;

    [Header("Duración de cada estado (segundos)")]
    public float tiempoReaccion = 0.5f;
    public float tiempoAtaque = 0.5f;
    public float tiempoEspera = 1f;
    public float tiempoRetroceso = 0.6f;
    public float tiempoPatrullaMin = 2f;
    public float tiempoPatrullaMax = 5f;

    Rigidbody2D rb;
    Animator animator;
    Transform jugador;
    Rigidbody2D rbJugadorDetectado;

    bool mirandoDerecha = true;
    bool jugadorEnRango = false;
    float timer = 0f;

    int currentHealth;
    bool esInvencible = false;
    float timerInvencible = 0f;

    Rigidbody2D rbJugador;
    bool jugadorEnKnockback = false;
    float timerKnockback = 0f;
    float knockbackDirX = 0f;
    float knockbackFuerzaX = 0f;
    bool laserYaDisparado = false;
    float multiplicadorRetrocesoActual = 1f;
    float cooldownGolpeContacto = 0f;
    bool evadiendoSalto = false;
    float timerEvasionSalto = 0f;
    float dirEvasionSalto = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        IniciarPatrulla();
    }

    void Update()
    {
        ActualizarInvencibilidad();
        ActualizarCooldownGolpe();
        ActualizarKnockbackJugador();
        ActualizarEvasionSalto();
        ActualizarAnimacion();
        ActualizarEstado();
    }

    void FixedUpdate()
    {
        float velY = rb.linearVelocity.y;

        if (evadiendoSalto)
        {
            rb.linearVelocity = new Vector2(dirEvasionSalto * velocidadEvasionSalto, velY);
            return;
        }

        switch (estadoActual)
        {
            case Estado.Patrulla:
                rb.linearVelocity = new Vector2(mirandoDerecha ? velocidadPatrulla : -velocidadPatrulla, velY);
                break;

            case Estado.Persecucion:
                rb.linearVelocity = new Vector2(mirandoDerecha ? velocidadPersecucion : -velocidadPersecucion, velY);
                break;

            case Estado.Retroceso:
                float dirRetro = mirandoDerecha ? -1f : 1f;
                rb.linearVelocity = new Vector2(dirRetro * (velocidadRetroceso * multiplicadorRetrocesoActual), velY);
                break;

            case Estado.Reaccion:
            case Estado.Ataque:
            case Estado.Espera:
                rb.linearVelocity = new Vector2(0f, velY);
                break;
        }
    }

    void ActualizarInvencibilidad()
    {
        if (!esInvencible) return;

        timerInvencible -= Time.deltaTime;
        if (timerInvencible <= 0f)
            esInvencible = false;
    }

    void ActualizarCooldownGolpe()
    {
        if (cooldownGolpeContacto > 0f)
            cooldownGolpeContacto -= Time.deltaTime;
    }

    void ActualizarKnockbackJugador()
    {
        if (!jugadorEnKnockback) return;

        timerKnockback -= Time.deltaTime;

        if (rbJugador != null)
            rbJugador.linearVelocity = new Vector2(knockbackDirX * knockbackFuerzaX, rbJugador.linearVelocity.y);

        if (timerKnockback <= 0f)
            jugadorEnKnockback = false;
    }

    void ActualizarEvasionSalto()
    {
        if (evadiendoSalto)
        {
            timerEvasionSalto -= Time.deltaTime;
            if (timerEvasionSalto <= 0f)
                evadiendoSalto = false;
            return;
        }

        if (!jugadorEnRango || jugador == null || rbJugadorDetectado == null) return;
        if (estadoActual != Estado.Persecucion && estadoActual != Estado.Patrulla) return;

        float dist = Vector2.Distance(transform.position, jugador.position);
        if (dist > distanciaEvasionSalto) return;
        if (rbJugadorDetectado.linearVelocity.y <= saltoJugadorMinY) return;

        dirEvasionSalto = jugador.position.x >= transform.position.x ? -1f : 1f;
        evadiendoSalto = true;
        timerEvasionSalto = duracionEvasionSalto;
    }

    void ActualizarAnimacion()
    {
        animator.SetBool("Caminando", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    }

    void ActualizarEstado()
    {
        switch (estadoActual)
        {
            case Estado.Patrulla: LogicaPatrulla(); break;
            case Estado.Reaccion: LogicaReaccion(); break;
            case Estado.Persecucion: LogicaPersecucion(); break;
            case Estado.Ataque: LogicaAtaque(); break;
            case Estado.Espera: LogicaEspera(); break;
            case Estado.Retroceso: LogicaRetroceso(); break;
        }
    }

    void LogicaPatrulla()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Girar();
            IniciarPatrulla();
        }

        if (jugadorEnRango && jugador != null)
        {
            float dist = Vector2.Distance(transform.position, jugador.position);
            if (dist <= distanciaDeteccionCercana)
                IniciarReaccion();
        }
    }

    void LogicaReaccion()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            estadoActual = Estado.Persecucion;
    }

    void LogicaPersecucion()
    {
        if (jugador == null) return;

        if (jugador.position.x > transform.position.x && !mirandoDerecha) Girar();
        if (jugador.position.x < transform.position.x && mirandoDerecha) Girar();

        float dist = Vector2.Distance(transform.position, jugador.position);
        if (dist <= distanciaAtaque)
            IniciarAtaque();
    }

    void LogicaAtaque()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            estadoActual = Estado.Espera;
            timer = tiempoEspera;
        }
    }

    void LogicaEspera()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (jugadorEnRango)
                estadoActual = Estado.Persecucion;
            else
                IniciarPatrulla();
        }
    }

    void LogicaRetroceso()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            multiplicadorRetrocesoActual = 1f;
            estadoActual = Estado.Espera;
            timer = Mathf.Max(tiempoPausaTrasImpacto, tiempoEspera);
        }
    }

    void IniciarPatrulla()
    {
        estadoActual = Estado.Patrulla;
        timer = Random.Range(tiempoPatrullaMin, tiempoPatrullaMax);
    }

    void IniciarReaccion()
    {
        estadoActual = Estado.Reaccion;
        timer = tiempoReaccion;
        if (!laserYaDisparado)
        {
            animator.SetTrigger("Reacionar");
            laserYaDisparado = true;
        }
    }

    void IniciarAtaque()
    {
        estadoActual = Estado.Ataque;
        timer = tiempoAtaque;
    }

    public void TakeDamage(int damage, bool reproducirAnimHit = false)
    {
        if (esInvencible) return;

        currentHealth -= damage;

        if (reproducirAnimHit)
            animator.SetTrigger("Golpeando");

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return;
        }

        esInvencible = true;
        timerInvencible = tiempoInvencible;
    }

    void EmpujarJugador(Rigidbody2D rbJ, float fuerzaX, float fuerzaY, float dirX)
    {
        if (rbJ == null) return;

        rbJugador = rbJ;
        knockbackDirX = dirX;
        knockbackFuerzaX = fuerzaX;
        jugadorEnKnockback = true;
        timerKnockback = duracionKnockback;

        rbJ.linearVelocity = new Vector2(dirX * fuerzaX, fuerzaY);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (estadoActual == Estado.Patrulla && col.gameObject.CompareTag("Pared"))
        {
            Girar();
            IniciarPatrulla();
        }

        if (!col.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = col.gameObject.GetComponent<PlayerHealth>();
        Rigidbody2D rbJ = col.gameObject.GetComponent<Rigidbody2D>();

        bool saltaronEncima = col.contacts[0].normal.y < -0.5f;

        if (saltaronEncima)
        {
            TakeDamage(1, true);

            float ladoX = col.transform.position.x > transform.position.x ? 1f : -1f;
            EmpujarJugador(rbJ, knockbackSaltoX, knockbackSaltoY, ladoX);
            if (rbJ != null)
                rbJ.angularVelocity = -ladoX * giroSaltoCabeza;

            estadoActual = Estado.Retroceso;
            multiplicadorRetrocesoActual = multiplicadorRetrocesoGolpe;
            timer = tiempoRetroceso;
        }
        else
        {
            AplicarGolpeLateral(playerHealth, rbJ, col.transform);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        if (cooldownGolpeContacto > 0f) return;

        bool saltoEnCabeza = false;
        for (int i = 0; i < col.contactCount; i++)
        {
            if (col.contacts[i].normal.y < -0.5f)
            {
                saltoEnCabeza = true;
                break;
            }
        }
        if (saltoEnCabeza) return;

        PlayerHealth playerHealth = col.gameObject.GetComponent<PlayerHealth>();
        Rigidbody2D rbJ = col.gameObject.GetComponent<Rigidbody2D>();
        AplicarGolpeLateral(playerHealth, rbJ, col.transform);
    }

    void AplicarGolpeLateral(PlayerHealth playerHealth, Rigidbody2D rbJ, Transform playerTransform)
    {
        if (playerHealth != null)
            playerHealth.TakeDamage(1);

        float ladoX = playerTransform.position.x > transform.position.x ? 1f : -1f;
        EmpujarJugador(rbJ, knockbackGolpeX, knockbackGolpeY, ladoX);
        estadoActual = Estado.Espera;
        timer = Mathf.Max(tiempoPausaTrasImpacto, tiempoEspera);
        cooldownGolpeContacto = tiempoEntreGolpesContacto;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        jugador = col.transform;
        rbJugadorDetectado = col.attachedRigidbody;
        jugadorEnRango = true;

        if (estadoActual == Estado.Patrulla)
            IniciarReaccion();
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        jugadorEnRango = false;
        rbJugadorDetectado = null;

        if (estadoActual == Estado.Persecucion)
            IniciarPatrulla();
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

}