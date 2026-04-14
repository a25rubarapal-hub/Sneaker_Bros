using UnityEngine;

public class MovimientoEnemigo : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    public float velocidad = 2f;
    private bool moviendoDerecha = true; // Empieza movi�ndose a la derecha

    private Rigidbody2D rb;

    void Start()
    {
        // Obtenemos el componente Rigidbody2D al iniciar
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movemos al enemigo aplicando velocidad constante en el eje X
        if (moviendoDerecha)
        {
            rb.linearVelocity = new Vector2(velocidad, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(-velocidad, rb.linearVelocity.y);
        }
    }

    // Se ejecuta autom�ticamente cuando el enemigo choca con algo
    private void OnCollisionEnter2D(Collision2D colision)
    {
        // Revisamos los puntos de contacto del choque
        foreach (ContactPoint2D contacto in colision.contacts)
        {
            // Comprobamos si el golpe fue por un lado (paredes) analizando la "normal" del choque.
            // Si el valor absoluto en X es mayor que 0 (aprox 0.5 o m�s), significa que es una pared vertical.
            if (Mathf.Abs(contacto.normal.x) > 0.5f)
            {
                Girar();
                break; // Salimos del bucle para no girar dos veces en el mismo choque
            }
        }
    }

    // Funci�n encargada de voltear al enemigo
    private void Girar()
    {
        // Cambiamos la direcci�n l�gica
        moviendoDerecha = !moviendoDerecha;

        // Volteamos visualmente el sprite multiplicando su escala en X por -1
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}