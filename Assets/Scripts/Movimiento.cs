using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerPlatformerFinal : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 5f;
    public float fuerzaSalto = 5f;
    public float fuerzaCaidaRapida = 10f;
    public float velocidadAgachado = 0.5f;

    [Header("Deslizamiento en Pared (Wall Slide)")]
    public float velocidadDeslizamientoPared = 1.5f;
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
    private bool enParedDerecha;   // hay pared a la derecha
    private bool enParedIzquierda; // hay pared a la izquierda
    private bool deslizandoEnPared;
    private bool quiereSaltar;
    private bool estaAgachado;
    private bool presionaAbajo;
    private bool quiereCaidaRapida;
    private bool mirandoDerecha = true;

    private float runCooldown = 0f;
    private const float RUN_COOLDOWN_TIME = 0.05f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        ActualizarColisiones();

        movimientoX = Input.GetAxisRaw("Horizontal");
        presionaAbajo = Input.GetKey(KeyCode.S);

        if (presionaAbajo)
        {
            if (enSuelo) { estaAgachado = true; quiereCaidaRapida = false; }
            else { estaAgachado = false; quiereCaidaRapida = true; }
        }
        else
        {
            estaAgachado = false;
            quiereCaidaRapida = false;
        }

        // FIX TILEMAP: detectamos pared en ambos lados y comprobamos si el input
        // empuja HACIA esa pared concreta. Sin input → nunca hay wall slide.
        bool empujandoDerecha = movimientoX > 0 && enParedDerecha;
        bool empujandoIzquierda = movimientoX < 0 && enParedIzquierda;
        deslizandoEnPared = !enSuelo && (empujandoDerecha || empujandoIzquierda) && rb.linearVelocity.y < 0;

        if (movimientoX > 0 && !mirandoDerecha) Girar();
        else if (movimientoX < 0 && mirandoDerecha) Girar();

        if (Input.GetKeyDown(KeyCode.W) && enSuelo && !estaAgachado && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            quiereSaltar = true;

        if (runCooldown > 0f) runCooldown -= Time.deltaTime;

        ActualizarAnimaciones();
    }

    void ActualizarColisiones()
    {
        // --- Suelo ---
        Vector2 posicionPies = new Vector2(col.bounds.center.x, col.bounds.center.y - offsetPies);
        enSuelo = false;
        foreach (Collider2D c in Physics2D.OverlapBoxAll(posicionPies, tamañoCajaSuelo, 0f, capaSuelo))
            if (c != col && !c.isTrigger) { enSuelo = true; break; }

   
    }

    void ActualizarAnimaciones()
    {
        if (anim == null) return;

        bool hayInputCorrer = enSuelo && !estaAgachado && Mathf.Abs(movimientoX) > 0.01f;
        if (hayInputCorrer) runCooldown = RUN_COOLDOWN_TIME;
        bool corriendo = runCooldown > 0f && enSuelo && !estaAgachado;

        bool animAgachado = estaAgachado || (!enSuelo && presionaAbajo);

        anim.SetBool("Corriendo", corriendo);
        anim.SetBool("EnSuelo", enSuelo);
        anim.SetBool("Agachado", animAgachado);
        // Comentada hasta que crees el parámetro en el Animator
        // anim.SetBool("Deslizando", deslizandoEnPared);
        anim.SetFloat("VelocidadY", rb.linearVelocity.y);
        anim.SetFloat("VelocidadX", Mathf.Abs(rb.linearVelocity.x));
    }

    void FixedUpdate()
    {
        float velActual = estaAgachado ? velocidad * velocidadAgachado : velocidad;

        float velX = deslizandoEnPared ? 0f : movimientoX * velActual;
        rb.linearVelocity = new Vector2(velX, rb.linearVelocity.y);

        if (deslizandoEnPared && rb.linearVelocity.y < -velocidadDeslizamientoPared)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -velocidadDeslizamientoPared);

        if (quiereSaltar)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            quiereSaltar = false;
        }

        if (quiereCaidaRapida && rb.linearVelocity.y <= 0 && !deslizandoEnPared)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fuerzaCaidaRapida);
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnDrawGizmos()
    {
        if (col == null) col = GetComponent<Collider2D>();
        if (col == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector2(col.bounds.center.x, col.bounds.center.y - offsetPies),
            tamañoCajaSuelo);

       
    }
}