using UnityEngine;

public class MovimientoNPC : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    public LayerMask capaSueloYEscenario; // Aquí seleccionaremos "Mapa" en el Inspector
    private bool moviendoDerecha = true;

    [Header("Configuración Visual")]
    public bool spriteInvertido = true;

    [Header("Interacción con el Jugador")]
    public string nombreDelBody = "body";

    private Rigidbody2D rb;
    private Collider2D miCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        miCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Movimiento constante
        float velocidadActual = moviendoDerecha ? velocidad : -velocidad;
        rb.linearVelocity = new Vector2(velocidadActual, rb.linearVelocity.y);

        // Control visual del sprite
        ActualizarEscala();
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        // 1. Detección del Jugador (buscando el objeto "body")
        if (colision.gameObject.name == nombreDelBody)
        {
            // Ignoramos la colisión para que el NPC lo atraviese
            Physics2D.IgnoreCollision(colision.collider, miCollider);
            return;
        }

        // 2. Detección de Paredes usando el LayerMask
        // Comprobamos si el objeto chocado está en la capa que definimos como mapa
        if (((1 << colision.gameObject.layer) & capaSueloYEscenario) != 0)
        {
            foreach (ContactPoint2D contacto in colision.contacts)
            {
                // Si chocamos de lado (la normal en X es fuerte)
                if (Mathf.Abs(contacto.normal.x) > 0.5f)
                {
                    moviendoDerecha = !moviendoDerecha;
                    break;
                }
            }
        }
    }

    void ActualizarEscala()
    {
        float direccionVisual = moviendoDerecha ? 1f : -1f;
        if (spriteInvertido) direccionVisual *= -1f;

        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * direccionVisual,
            transform.localScale.y,
            transform.localScale.z
        );
    }
}