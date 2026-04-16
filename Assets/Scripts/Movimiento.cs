using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerPlatformerFinal : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 7f;
    public float fuerzaSalto = 8f;
    public float fuerzaCaidaRapida = 15f;
    public float velocidadAgachado = 0.5f;

    [Header("Deslizamiento en Pared (Wall Slide)")]
    public float velocidadDeslizamientoPared = 2f; // Qué tan lento resbala por la pared
    public Vector2 tamañoCajaPared = new Vector2(0.2f, 0.8f);
    public float offsetPared = 0.4f;

    [Header("Detección de Suelo")]
    public LayerMask capaSuelo;
    public Vector2 tamañoCajaSuelo = new Vector2(0.5f, 0.1f);
    public float offsetPies = 0.6f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    private float movimientoX;
    private bool enSuelo;
    private bool enPared;
    private bool deslizandoEnPared;
    private bool quiereSaltar;
    private bool estaAgachado;
    private bool quiereCaidaRapida;
    private bool mirandoDerecha = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. DETECCIÓN DE SUELO Y PARED
        ActualizarColisiones();

        // 2. LEER TECLADO
        movimientoX = Input.GetAxisRaw("Horizontal");
        bool pulsandoS = Input.GetKey(KeyCode.S);

        // 3. LÓGICA DE AGACHARSE Y CAÍDA RÁPIDA
        if (pulsandoS)
        {
            if (enSuelo)
            {
                estaAgachado = true;
                quiereCaidaRapida = false;
            }
            else
            {
                estaAgachado = false;
                quiereCaidaRapida = true;
            }
        }
        else
        {
            estaAgachado = false;
            quiereCaidaRapida = false;
        }

        // 4. LÓGICA DE DESLIZAMIENTO EN PARED
        // Si estamos tocando una pared, NO estamos en el suelo, y el jugador está presionando hacia la pared
        if (enPared && !enSuelo && movimientoX != 0)
        {
            deslizandoEnPared = true;
        }
        else
        {
            deslizandoEnPared = false;
        }

        // 5. VOLTEAR AL PERSONAJE 
        if (movimientoX > 0 && !mirandoDerecha) Girar();
        else if (movimientoX < 0 && mirandoDerecha) Girar();

        // 6. LEER SALTO
        if (Input.GetKeyDown(KeyCode.W) && enSuelo && !estaAgachado && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            quiereSaltar = true;
        }

        // 7. ACTUALIZAR ANIMATOR
        ActualizarAnimaciones();
    }

    void ActualizarColisiones()
    {
        // Detección de suelo
        Vector2 posicionPies = new Vector2(col.bounds.center.x, col.bounds.center.y - offsetPies);
        Collider2D[] cosasSuelo = Physics2D.OverlapBoxAll(posicionPies, tamañoCajaSuelo, 0f, capaSuelo);
        enSuelo = false;
        foreach (Collider2D cosa in cosasSuelo)
        {
            if (cosa != col && !cosa.isTrigger) { enSuelo = true; break; }
        }

        // Detección de pared (calcula si la pared está a la derecha o izquierda según hacia dónde miramos)
        float direccionMirada = mirandoDerecha ? 1f : -1f;
        Vector2 posicionPared = new Vector2(col.bounds.center.x + (offsetPared * direccionMirada), col.bounds.center.y);
        Collider2D[] cosasPared = Physics2D.OverlapBoxAll(posicionPared, tamañoCajaPared, 0f, capaSuelo);
        enPared = false;
        foreach (Collider2D cosa in cosasPared)
        {
            if (cosa != col && !cosa.isTrigger) { enPared = true; break; }
        }
    }

    void ActualizarAnimaciones()
    {
        if (anim != null)
        {
            anim.SetBool("Corriendo", Mathf.Abs(rb.linearVelocity.x) > 0.1f && enSuelo);
            anim.SetBool("EnSuelo", enSuelo);
            anim.SetBool("Agachado", estaAgachado);
            anim.SetFloat("VelocidadY", rb.linearVelocity.y);
            anim.SetFloat("VelocidadX", Mathf.Abs(rb.linearVelocity.x));

            // Opcional: Si tienes una animación de deslizar pared, puedes añadirla aquí
            // anim.SetBool("Deslizando", deslizandoEnPared);
        }
    }

    void FixedUpdate()
    {
        float velActual = estaAgachado ? velocidad * velocidadAgachado : velocidad;

        // Movimiento base
        rb.linearVelocity = new Vector2(movimientoX * velActual, rb.linearVelocity.y);

        // APLICAR DESLIZAMIENTO DE PARED
        if (deslizandoEnPared)
        {
            // Si la velocidad de caída es mayor que nuestra velocidad de deslizamiento límite, la frenamos
            if (rb.linearVelocity.y < -velocidadDeslizamientoPared)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -velocidadDeslizamientoPared);
            }
        }

        if (quiereSaltar)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            quiereSaltar = false;
        }

        // La caída rápida no debería funcionar si te estás deslizando por la pared
        if (quiereCaidaRapida && rb.linearVelocity.y <= 0 && !deslizandoEnPared)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fuerzaCaidaRapida);
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escalaLocal = transform.localScale;
        escalaLocal.x *= -1;
        transform.localScale = escalaLocal;
    }

    private void OnDrawGizmos()
    {
        if (col == null) col = GetComponent<Collider2D>();
        if (col != null)
        {
            // Dibujar caja de suelo (Verde)
            Gizmos.color = Color.green;
            Vector2 posicionPies = new Vector2(col.bounds.center.x, col.bounds.center.y - offsetPies);
            Gizmos.DrawWireCube(posicionPies, tamañoCajaSuelo);

            // Dibujar caja de pared (Azul)
            Gizmos.color = Color.blue;
            float direccionMirada = mirandoDerecha ? 1f : -1f;
            Vector2 posicionPared = new Vector2(col.bounds.center.x + (offsetPared * direccionMirada), col.bounds.center.y);
            Gizmos.DrawWireCube(posicionPared, tamañoCajaPared);
        }
    }
}