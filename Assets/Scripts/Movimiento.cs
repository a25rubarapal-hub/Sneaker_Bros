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

    [Header("Detección de Suelo")]
    public LayerMask capaSuelo;
    public Vector2 tamañoCajaSuelo = new Vector2(0.6f, 0.1f);
    public float offsetPies = 0.6f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    private float movimientoX;
    private bool enSuelo;
    private bool quiereSaltar;
    private bool estaAgachado;
    private bool quiereCaidaRapida;
    private bool mirandoDerecha = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. DETECCIÓN DE SUELO 
        Vector2 posicionPies = new Vector2(col.bounds.center.x, col.bounds.center.y - offsetPies);
        Collider2D[] cosasTocadas = Physics2D.OverlapBoxAll(posicionPies, tamañoCajaSuelo, 0f, capaSuelo);

        enSuelo = false;
        foreach (Collider2D cosa in cosasTocadas)
        {
            if (cosa != col)
            {
                enSuelo = true;
                break;
            }
        }

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

        // 4. VOLTEAR AL PERSONAJE 
        if (movimientoX > 0 && !mirandoDerecha) Girar();
        else if (movimientoX < 0 && mirandoDerecha) Girar();

        // 5. LEER SALTO
        if (Input.GetKeyDown(KeyCode.W) && enSuelo && !estaAgachado)
        {
            quiereSaltar = true;
        }

        // 6. ACTUALIZAR ANIMATOR (Solo una vez y al final)
        ActualizarAnimaciones();
    }

    void ActualizarAnimaciones()
    {
        if (anim != null)
        {
            // Corriendo: Solo si se mueve y ESTÁ en el suelo
            anim.SetBool("Corriendo", Mathf.Abs(rb.linearVelocity.x) > 0.1f && enSuelo);

            // EnSuelo: Fundamental para saltos/caídas
            anim.SetBool("EnSuelo", enSuelo);

            // Agachado: La señal de la S
            anim.SetBool("Agachado", estaAgachado);

            // VelocidadY: Para detectar si sube o cae
            anim.SetFloat("VelocidadY", rb.linearVelocity.y);
            anim.SetFloat("VelocidadX", rb.linearVelocity.x);
        }
    }

    void FixedUpdate()
    {
        float velActual = estaAgachado ? velocidad * velocidadAgachado : velocidad;
        rb.linearVelocity = new Vector2(movimientoX * velActual, rb.linearVelocity.y);

        if (quiereSaltar)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            quiereSaltar = false;
        }

        if (quiereCaidaRapida && rb.linearVelocity.y <= 0)
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
            Gizmos.color = Color.green;
            Vector2 posicionPies = new Vector2(col.bounds.center.x, col.bounds.center.y - offsetPies);
            Gizmos.DrawWireCube(posicionPies, tamañoCajaSuelo);
        }
    }
}