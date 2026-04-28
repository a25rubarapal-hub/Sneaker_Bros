using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class JefeMario : MonoBehaviour
{
    enum Estado { Patrulla, Reaccion, Persecucion, Ataque, Espera, Retroceso, BusquedaUltimaPos }
    Estado estadoActual = Estado.Patrulla;

    [Header("Movimiento del jefe")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;
    public float velocidadRetroceso = 7f;
    public float distanciaAtaque = 1.5f;
    public float distanciaDeteccionCercana = 1.0f;

    [Header("Salud e invencibilidad")]
    public int maxHealth = 67;
    public float tiempoInvencible = 0.15f;

    [Header("Knockback que recibe el JUGADOR")]
    public float knockbackSaltoX = 4.5f;
    public float knockbackSaltoY = 9f;
    public float knockbackGolpeX = 12f;
    public float knockbackGolpeY = 5f;
    public float duracionKnockback = 0.5f;
    public float giroSaltoCabeza = 420f;

    [Header("Recuperación tras golpes")]
    public float tiempoPausaTrasImpacto = 1f;
    public float multiplicadorRetrocesoGolpe = 5f;

    [Header("Daño por contacto continuo")]
    public float tiempoEntreGolpesContacto = 0.45f;

    [Header("Evasión al salto del jugador")]
    public float saltoJugadorMinY = 0.2f;
    public float distanciaEvasionSalto = 2.2f;
    public float velocidadEvasionSalto = 5.5f;
    public float duracionEvasionSalto = 0.28f;
    public float multiplicadorEvasionHaciaAtras = 1.35f;

    [Header("Duración de cada estado (segundos)")]
    public float tiempoReaccion = 0.5f;
    public float tiempoAtaque = 0.5f;
    public float tiempoEspera = 1f;
    public float tiempoRetroceso = 1.2f;
    public float tiempoPatrullaMin = 2f;
    public float tiempoPatrullaMax = 5f;

    [Header("Animacion de ataque")]
    public string nombreEstadoAnimAtaque = "Attack";
    public string nombreEstadoAnimIdle = "idle";
    public float duracionAtaqueNoInterrumpible = 2f;

    [Header("Lectura de terreno")]
    public LayerMask mascaraTerreno;
    public float distanciaChequeoPared = 0.7f;
    public float avanceChequeoSuelo = 0.55f;
    public float distanciaChequeoSuelo = 1.2f;

    [Header("Colliders (2D)")]
    [SerializeField] BoxCollider2D bodyCollider;
    [SerializeField] BoxCollider2D damageCollider;
    [SerializeField] Collider2D radarCollider;
    [SerializeField] Vector2 radarOffsetPorDefecto = new Vector2(0f, 0.2f);
    [SerializeField] Vector2 radarSizePorDefecto = new Vector2(4.5f, 2.4f);
    [SerializeField] Vector2 damageOffsetPorDefecto = new Vector2(0.7f, 0.2f);
    [SerializeField] Vector2 damageSizePorDefecto = new Vector2(1.2f, 1.6f);

    // ── Radar de pared: ahora Collider2D genérico (admite CircleCollider2D u otros) ──
    [Header("Radar de Pared (Tilemap)")]
    [Tooltip("Asigna aquí el Collider2D (p.ej. CircleCollider2D) que actuará como radar de pared")]
    [SerializeField] Collider2D wallRadar;
    [Tooltip("Tilemap específico que el jefe debe evitar (asígnalo en el Inspector)")]
    [SerializeField] Tilemap tilemapObjetivo;
    [Tooltip("Si está activo, el jefe ignora el radar de pared mientras persigue al jugador")]
    public bool ignorarParedEnPersecucion = false;

    [Header("Búsqueda - Último lugar conocido")]
    [Tooltip("Velocidad al ir al último lugar conocido del jugador")]
    public float velocidadBusqueda = 3f;
    [Tooltip("Distancia mínima al punto para darlo como alcanzado")]
    public float distanciaLlegadaBusqueda = 0.4f;
    [Tooltip("Segundos sin detectar al jugador antes de volver a patrullar")]
    public float tiempoMaxBusqueda = 20f;

    [Header("Parámetros de Animator")]
    [SerializeField] string paramCaminando = "Caminando";
    [SerializeField] string triggerAtacar = "Atacar";
    [SerializeField] string triggerReaccionar = "Reacionar";

    [Header("Decisiones IA 2D")]
    public float distanciaAtaqueSaltoJugador = 1.25f;

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
    float dirRetrocesoActual = -1f;
    float dirPersecucionActual = 0f;
    bool ataqueTriggerPendiente = false;
    Collider2D objetivoColliderAtaque;
    Transform objetivoTransformAtaque;
    Rigidbody2D objetivoRbAtaque;
    PlayerHealth objetivoHealthAtaque;
    bool esperandoFinAnimAtaque = false;
    bool entroEnEstadoAnimAtaque = false;
    bool ataqueNoInterrumpible = false;
    float timerAtaqueNoInterrumpible = 0f;
    int hashCaminando;
    int hashAtacar;
    int hashReaccionar;

    Vector2 ultimaPosJugador;
    bool ultimaPosValida = false;
    float timerBusqueda = 0f;
    TilemapCollider2D tilemapColliderObjetivo;

    bool wallRadarTocaPared = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        InicializarHashesAnimator();
        AutoAsignarColliders();
        AsegurarConfiguracionHitboxes();
        ObtenerColliderTilemap();
        ActualizarObjetivoDesdeRadar();
        IniciarPatrulla();
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
            AsegurarConfiguracionHitboxes();
    }

    void ObtenerColliderTilemap()
    {
        if (tilemapObjetivo != null)
            tilemapColliderObjetivo = tilemapObjetivo.GetComponent<TilemapCollider2D>();
    }

    void InicializarHashesAnimator()
    {
        hashCaminando  = string.IsNullOrWhiteSpace(paramCaminando)   ? 0 : Animator.StringToHash(paramCaminando);
        hashAtacar     = string.IsNullOrWhiteSpace(triggerAtacar)    ? 0 : Animator.StringToHash(triggerAtacar);
        hashReaccionar = string.IsNullOrWhiteSpace(triggerReaccionar)? 0 : Animator.StringToHash(triggerReaccionar);
    }

    void AutoAsignarColliders()
    {
        BoxCollider2D[] boxes = GetComponents<BoxCollider2D>();

        if (bodyCollider == null)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i] != null && !boxes[i].isTrigger)
                {
                    bodyCollider = boxes[i];
                    break;
                }
            }
        }

        if (damageCollider != null)
            goto BuscarRadar;

        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i] != null && boxes[i].isTrigger && boxes[i] != bodyCollider)
            {
                damageCollider = boxes[i];
                break;
            }
        }

BuscarRadar:
        if (radarCollider == null)
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D c = colliders[i];
                if (c == null || c == bodyCollider || c == damageCollider)
                    continue;
                if (c.isTrigger)
                {
                    radarCollider = c;
                    break;
                }
            }
        }
    }

    void AsegurarConfiguracionHitboxes()
    {
        AutoAsignarColliders();

        if (bodyCollider != null)
            bodyCollider.isTrigger = false;

        if (radarCollider != null)
        {
            radarCollider.isTrigger = true;
            if (radarCollider is CircleCollider2D radarCircle)
            {
                if (radarCircle.radius <= 0.01f)
                    radarCircle.radius = Mathf.Max(radarSizePorDefecto.x, radarSizePorDefecto.y) * 0.5f;
                if (radarCircle.offset == Vector2.zero)
                    radarCircle.offset = radarOffsetPorDefecto;
            }
            else if (radarCollider is BoxCollider2D radarBox)
            {
                if (radarBox.size.x <= 0.01f || radarBox.size.y <= 0.01f)
                    radarBox.size = radarSizePorDefecto;
                if (radarBox.offset == Vector2.zero)
                    radarBox.offset = radarOffsetPorDefecto;
            }
        }

        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;
            if (damageCollider.size.x <= 0.01f || damageCollider.size.y <= 0.01f)
                damageCollider.size = damageSizePorDefecto;
            if (damageCollider.offset == Vector2.zero)
                damageCollider.offset = damageOffsetPorDefecto;
        }

        // wallRadar: funciona con cualquier Collider2D (CircleCollider2D, BoxCollider2D, etc.)
        if (wallRadar != null)
            wallRadar.isTrigger = true;
    }

    bool ComprobarWallRadarTocaPared()
    {
        if (wallRadar == null) return false;
        if (tilemapColliderObjetivo == null) return false;

        return wallRadar.IsTouching(tilemapColliderObjetivo);
    }

    bool ParedDetectadaActiva()
    {
        if (!wallRadarTocaPared) return false;
        if (ignorarParedEnPersecucion && estadoActual == Estado.Persecucion) return false;
        return true;
    }

    float DireccionEscapePared()
    {
        return mirandoDerecha ? -1f : 1f;
    }

    void Update()
    {
        if (!jugadorEnRango || jugador == null)
            ActualizarObjetivoDesdeRadar();

        if (jugadorEnRango && jugador != null)
        {
            ultimaPosJugador = jugador.position;
            ultimaPosValida = true;
            timerBusqueda = 0f;
        }

        wallRadarTocaPared = ComprobarWallRadarTocaPared();

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

        if (ataqueNoInterrumpible)
        {
            rb.linearVelocity = new Vector2(0f, velY);
            return;
        }

        if (ParedDetectadaActiva() && estadoActual != Estado.Ataque && estadoActual != Estado.Reaccion)
        {
            float dirEscape = DireccionEscapePared();
            MirarHaciaDireccion(dirEscape);
            rb.linearVelocity = new Vector2(dirEscape * velocidadPatrulla, velY);
            return;
        }

        if (evadiendoSalto)
        {
            float dirFinal = dirEvasionSalto;
            if (wallRadarTocaPared)
                dirFinal = DireccionEscapePared();

            rb.linearVelocity = new Vector2(dirFinal * (velocidadEvasionSalto * multiplicadorEvasionHaciaAtras), velY);
            return;
        }

        switch (estadoActual)
        {
            case Estado.Patrulla:
                if (!PuedeMoverEnDireccion(mirandoDerecha ? 1f : -1f))
                    Girar();
                rb.linearVelocity = new Vector2(mirandoDerecha ? velocidadPatrulla : -velocidadPatrulla, velY);
                break;

            case Estado.Persecucion:
                if (Mathf.Abs(dirPersecucionActual) < 0.01f)
                    dirPersecucionActual = mirandoDerecha ? 1f : -1f;
                rb.linearVelocity = new Vector2(dirPersecucionActual * velocidadPersecucion, velY);
                break;

            case Estado.BusquedaUltimaPos:
                if (ultimaPosValida)
                {
                    float dirBusq = ultimaPosJugador.x >= transform.position.x ? 1f : -1f;
                    MirarHaciaDireccion(dirBusq);
                    rb.linearVelocity = new Vector2(dirBusq * velocidadBusqueda, velY);
                }
                else
                {
                    rb.linearVelocity = new Vector2(0f, velY);
                }
                break;

            case Estado.Retroceso:
                if (!PuedeMoverEnDireccion(dirRetrocesoActual))
                    dirRetrocesoActual *= -1f;
                rb.linearVelocity = new Vector2(dirRetrocesoActual * (velocidadRetroceso * multiplicadorRetrocesoActual), velY);
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
        if (ataqueNoInterrumpible) return;

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

        if (dist <= distanciaAtaqueSaltoJugador && cooldownGolpeContacto <= 0f)
        {
            IniciarAtaque();
            return;
        }

        dirEvasionSalto = jugador.position.x >= transform.position.x ? -1f : 1f;

        if (wallRadarTocaPared && dirEvasionSalto != DireccionEscapePared())
            dirEvasionSalto *= -1f;

        if (!PuedeMoverEnDireccion(dirEvasionSalto))
            dirEvasionSalto *= -1f;

        evadiendoSalto = PuedeMoverEnDireccion(dirEvasionSalto);
        timerEvasionSalto = duracionEvasionSalto;
    }

    void ActualizarAnimacion()
    {
        if (ataqueNoInterrumpible)
        {
            SetCaminando(false);
            return;
        }

        bool moviendose = estadoActual == Estado.Patrulla
            || estadoActual == Estado.Persecucion
            || estadoActual == Estado.Retroceso
            || estadoActual == Estado.BusquedaUltimaPos
            || evadiendoSalto
            || ParedDetectadaActiva();

        bool caminando = moviendose && Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        SetCaminando(caminando);
    }

    void SetCaminando(bool valor)
    {
        if (animator == null || hashCaminando == 0) return;
        animator.SetBool(hashCaminando, valor);
    }

    void ActualizarEstado()
    {
        switch (estadoActual)
        {
            case Estado.Patrulla:          LogicaPatrulla();          break;
            case Estado.Reaccion:          LogicaReaccion();          break;
            case Estado.Persecucion:       LogicaPersecucion();       break;
            case Estado.Ataque:            LogicaAtaque();            break;
            case Estado.Espera:            LogicaEspera();            break;
            case Estado.Retroceso:         LogicaRetroceso();         break;
            case Estado.BusquedaUltimaPos: LogicaBusquedaUltimaPos(); break;
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
            IniciarReaccion();
    }

    void LogicaReaccion()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            estadoActual = Estado.Persecucion;
    }

    void LogicaPersecucion()
    {
        if (jugador == null)
        {
            IniciarBusquedaUltimaPos();
            return;
        }

        float dirDeseada = jugador.position.x >= transform.position.x ? 1f : -1f;
        if (PuedeMoverEnDireccion(dirDeseada))
            dirPersecucionActual = dirDeseada;
        else
            dirPersecucionActual = PuedeMoverEnDireccion(-dirDeseada) ? -dirDeseada : 0f;

        if (Mathf.Abs(dirPersecucionActual) > 0.01f)
            MirarHaciaDireccion(dirPersecucionActual);

        float dist = Vector2.Distance(transform.position, jugador.position);
        if (dist <= distanciaAtaque)
            IniciarAtaque();
    }

    void LogicaBusquedaUltimaPos()
    {
        if (jugadorEnRango && jugador != null)
        {
            estadoActual = Estado.Persecucion;
            return;
        }

        timerBusqueda += Time.deltaTime;

        if (timerBusqueda >= tiempoMaxBusqueda)
        {
            ultimaPosValida = false;
            timerBusqueda = 0f;
            IniciarPatrulla();
            return;
        }

        if (!ultimaPosValida)
        {
            IniciarPatrulla();
            return;
        }

        float distAlPunto = Mathf.Abs(transform.position.x - ultimaPosJugador.x);
        if (distAlPunto <= distanciaLlegadaBusqueda)
        {
            estadoActual = Estado.Espera;
            timer = tiempoEspera;
        }
    }

    void IniciarBusquedaUltimaPos()
    {
        if (!ultimaPosValida)
        {
            IniciarPatrulla();
            return;
        }
        estadoActual = Estado.BusquedaUltimaPos;
        timerBusqueda = 0f;
    }

    void LogicaAtaque()
    {
        if (ataqueNoInterrumpible)
        {
            timerAtaqueNoInterrumpible -= Time.deltaTime;
            if (timerAtaqueNoInterrumpible > 0f)
                return;

            ataqueNoInterrumpible = false;
            timerAtaqueNoInterrumpible = 0f;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            esperandoFinAnimAtaque = false;
            ResolverAtaquePorTriggerSiCorresponde();
            ataqueNoInterrumpible = false;
            timerAtaqueNoInterrumpible = 0f;
            estadoActual = Estado.Espera;
            timer = tiempoEspera;
            return;
        }

        if (esperandoFinAnimAtaque && !AnimacionAtaqueTerminada())
            return;

        ResolverAtaquePorTriggerSiCorresponde();
        ataqueNoInterrumpible = false;
        timerAtaqueNoInterrumpible = 0f;
        estadoActual = Estado.Espera;
        timer = tiempoEspera;
    }

    void LogicaEspera()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (jugadorEnRango && jugador != null)
                estadoActual = Estado.Persecucion;
            else if (ultimaPosValida)
                IniciarBusquedaUltimaPos();
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
            if (jugadorEnRango && jugador != null)
            {
                estadoActual = Estado.Persecucion;
                float dirDeseada = jugador.position.x >= transform.position.x ? 1f : -1f;
                dirPersecucionActual = dirDeseada;
                MirarHaciaDireccion(dirDeseada);
            }
            else
            {
                estadoActual = Estado.Espera;
                timer = Mathf.Max(tiempoPausaTrasImpacto, tiempoEspera);
            }
        }
    }

    void IniciarPatrulla()
    {
        if (jugadorEnRango && jugador != null)
        {
            estadoActual = Estado.Persecucion;
            timer = 0f;
            return;
        }

        estadoActual = Estado.Patrulla;
        timer = Random.Range(tiempoPatrullaMin, tiempoPatrullaMax);
    }

    void IniciarReaccion()
    {
        if (ataqueTriggerPendiente || estadoActual == Estado.Ataque)
            return;

        estadoActual = Estado.Reaccion;
        timer = tiempoReaccion;
        if (!laserYaDisparado)
        {
            if (animator != null && hashReaccionar != 0)
                animator.SetTrigger(hashReaccionar);
            laserYaDisparado = true;
        }
    }

    void IniciarAtaque()
    {
        estadoActual = Estado.Ataque;
        timer = Mathf.Max(tiempoAtaque, 0.05f);
        dirPersecucionActual = 0f;
        esperandoFinAnimAtaque = true;
        entroEnEstadoAnimAtaque = false;
        ataqueNoInterrumpible = true;
        timerAtaqueNoInterrumpible = Mathf.Max(0.05f, duracionAtaqueNoInterrumpible);
        SetCaminando(false);

        if (animator != null && hashAtacar != 0)
        {
            if (hashReaccionar != 0)
                animator.ResetTrigger(hashReaccionar);
            animator.SetTrigger(hashAtacar);
        }
    }

    void IniciarAtaquePorTrigger(Collider2D objetivo)
    {
        if (objetivo == null) return;
        if (ataqueTriggerPendiente) return;
        if (cooldownGolpeContacto > 0f) return;

        objetivoColliderAtaque = objetivo;
        objetivoTransformAtaque = objetivo.transform;
        objetivoRbAtaque = objetivo.attachedRigidbody;
        objetivoHealthAtaque = objetivo.GetComponent<PlayerHealth>();
        ataqueTriggerPendiente = true;
        IniciarAtaque();
        esperandoFinAnimAtaque = true;
        entroEnEstadoAnimAtaque = false;
    }

    void ResolverAtaquePorTriggerSiCorresponde()
    {
        if (!ataqueTriggerPendiente) return;

        bool objetivoValido = objetivoColliderAtaque != null
            && objetivoColliderAtaque.CompareTag("Player")
            && TocaHitboxDanio(objetivoColliderAtaque);

        if (objetivoValido)
            AplicarGolpeLateral(objetivoHealthAtaque, objetivoRbAtaque, objetivoTransformAtaque);

        ataqueTriggerPendiente = false;
        objetivoColliderAtaque = null;
        objetivoTransformAtaque = null;
        objetivoRbAtaque = null;
        objetivoHealthAtaque = null;
        ataqueNoInterrumpible = false;
        timerAtaqueNoInterrumpible = 0f;
        ForzarSalidaAnimacionAtaque();
    }

    void ForzarSalidaAnimacionAtaque()
    {
        if (animator == null) return;

        if (hashAtacar != 0)
            animator.ResetTrigger(hashAtacar);
        if (hashReaccionar != 0)
            animator.ResetTrigger(hashReaccionar);
        SetCaminando(false);

        if (!string.IsNullOrEmpty(nombreEstadoAnimIdle))
            animator.CrossFadeInFixedTime(nombreEstadoAnimIdle, 0.05f, 0);
    }

    bool AnimacionAtaqueTerminada()
    {
        if (animator == null || string.IsNullOrEmpty(nombreEstadoAnimAtaque))
            return true;

        AnimatorStateInfo estado = animator.GetCurrentAnimatorStateInfo(0);
        bool estaEnAtaque = estado.IsName(nombreEstadoAnimAtaque);
        if (estaEnAtaque)
        {
            entroEnEstadoAnimAtaque = true;
            if (!animator.IsInTransition(0) && estado.normalizedTime >= 1f)
            {
                esperandoFinAnimAtaque = false;
                return true;
            }
            return false;
        }

        if (entroEnEstadoAnimAtaque)
        {
            esperandoFinAnimAtaque = false;
            return true;
        }

        return false;
    }

    float DireccionEsquiveLateralAleatoria()
    {
        return Random.value < 0.5f ? -1f : 1f;
    }

    bool PuedeMoverEnDireccion(float dirX)
    {
        if (mascaraTerreno.value == 0)
            return true;

        Vector2 origenPies = rb.position + Vector2.down * 0.2f;
        Vector2 dirHorizontal = dirX >= 0f ? Vector2.right : Vector2.left;
        Vector2 origenPared = origenPies + Vector2.up * 0.55f;

        bool hayPared = Physics2D.Raycast(origenPared, dirHorizontal, distanciaChequeoPared, mascaraTerreno);
        bool haySueloDelante = Physics2D.Raycast(origenPies + dirHorizontal * avanceChequeoSuelo, Vector2.down, distanciaChequeoSuelo, mascaraTerreno);

        return !hayPared && haySueloDelante;
    }

    bool TocaRadar(Collider2D col)
    {
        return radarCollider != null && radarCollider.isTrigger && radarCollider.IsTouching(col);
    }

    bool TocaHitboxDanio(Collider2D col)
    {
        return damageCollider != null && damageCollider.isTrigger && damageCollider.IsTouching(col);
    }

    void ActualizarObjetivoDesdeRadar()
    {
        if (radarCollider == null || !radarCollider.isTrigger)
        {
            jugadorEnRango = false;
            rbJugadorDetectado = null;
            return;
        }

        Collider2D[] resultados = new Collider2D[8];
        ContactFilter2D filtro = new ContactFilter2D();
#pragma warning disable CS0618
        filtro.NoFilter();
#pragma warning restore CS0618
        int total = radarCollider.Overlap(filtro, resultados);

        jugadorEnRango = false;
        jugador = null;
        rbJugadorDetectado = null;

        for (int i = 0; i < total; i++)
        {
            Collider2D c = resultados[i];
            if (c != null && c.CompareTag("Player"))
            {
                jugadorEnRango = true;
                jugador = c.transform;
                rbJugadorDetectado = c.attachedRigidbody;
                break;
            }
        }
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
        if (ataqueNoInterrumpible) return;

        if (estadoActual == Estado.Patrulla && col.gameObject.CompareTag("Pared"))
        {
            Girar();
            IniciarPatrulla();
        }

        if (!col.gameObject.CompareTag("Player")) return;

        Rigidbody2D rbJ = col.gameObject.GetComponent<Rigidbody2D>();
        bool saltaronEncima = EsGolpeEnCabeza(col);

        if (saltaronEncima)
        {
            TakeDamage(1, true);

            float ladoX = col.transform.position.x > transform.position.x ? 1f : -1f;
            EmpujarJugador(rbJ, knockbackSaltoX, knockbackSaltoY, ladoX);
            if (rbJ != null)
                rbJ.angularVelocity = -ladoX * giroSaltoCabeza;

            estadoActual = Estado.Retroceso;
            multiplicadorRetrocesoActual = multiplicadorRetrocesoGolpe;
            dirRetrocesoActual = DireccionEsquiveLateralAleatoria();
            timer = tiempoRetroceso;
        }
    }

    bool EsGolpeEnCabeza(Collision2D col)
    {
        if (col == null || col.contactCount <= 0)
            return false;

        float limiteCabezaY = bodyCollider != null
            ? bodyCollider.bounds.center.y + bodyCollider.bounds.extents.y * 0.55f
            : transform.position.y + 0.45f;

        for (int i = 0; i < col.contactCount; i++)
        {
            ContactPoint2D p = col.GetContact(i);
            if (p.normal.y < -0.5f && p.point.y >= limiteCabezaY)
                return true;
        }

        return false;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        bool saltoEnCabeza = false;
        for (int i = 0; i < col.contactCount; i++)
        {
            if (col.contacts[i].normal.y < -0.5f)
            {
                saltoEnCabeza = true;
                break;
            }
        }
        if (saltoEnCabeza)
            return;
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
        if (ataqueNoInterrumpible) return;

        if (damageCollider == null)
            AutoAsignarColliders();

        if (TocaHitboxDanio(col))
        {
            IniciarAtaquePorTrigger(col);
            return;
        }

        if (TocaRadar(col))
        {
            ActualizarObjetivoDesdeRadar();
            if (estadoActual == Estado.Patrulla || estadoActual == Estado.BusquedaUltimaPos)
                IniciarReaccion();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        if (ataqueNoInterrumpible) return;
        if (cooldownGolpeContacto > 0f) return;
        if (damageCollider == null)
            AutoAsignarColliders();

        if (TocaHitboxDanio(col))
            IniciarAtaquePorTrigger(col);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        if (ataqueNoInterrumpible) return;

        ActualizarObjetivoDesdeRadar();

        if (!jugadorEnRango)
        {
            if (estadoActual == Estado.Persecucion)
                IniciarBusquedaUltimaPos();
        }
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void MirarHaciaDireccion(float dirX)
    {
        if (dirX > 0f && !mirandoDerecha)
            Girar();
        else if (dirX < 0f && mirandoDerecha)
            Girar();
    }
}