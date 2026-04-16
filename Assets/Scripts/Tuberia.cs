using UnityEngine;

public class Tuberia : MonoBehaviour
{
    [Header("Arrastra aquí a tu Personaje")]
    public GameObject jugador;

    [Header("Arrastra aquí la Salida correspondiente")]
    public Transform puntoDeSalida;

    [Header("¿Requiere pulsar Abajo?")]
    public bool requiereBoton = true;

    private bool jugadorEnZona = false;

    void Update()
    {
        // Si el jugador está tocando la tubería y pulsa Abajo
        if (jugadorEnZona && requiereBoton && Input.GetAxisRaw("Vertical") < 0)
        {
            Teletransportar();
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        // ¡Ya no usamos Tags! Comprobamos si el objeto que chocó es exactamente tu jugador
        if (otro.gameObject == jugador)
        {
            jugadorEnZona = true;

            // Si no requiere botón, se teletransporta automáticamente
            if (!requiereBoton)
            {
                Teletransportar();
            }
        }
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.gameObject == jugador)
        {
            jugadorEnZona = false;
        }
    }

    void Teletransportar()
    {
        if (puntoDeSalida != null && jugador != null)
        {
            // Movemos al jugador a la salida
            jugador.transform.position = puntoDeSalida.position;

            // Le avisamos a la salida que se vuelva sólida
            SalidaTuberia scriptSalida = puntoDeSalida.GetComponent<SalidaTuberia>();
            if (scriptSalida != null)
            {
                scriptSalida.ActivarPlataforma();
            }

            jugadorEnZona = false;
        }
        else
        {
            Debug.LogWarning("¡Falta arrastrar al Jugador o la Salida en el Inspector!");
        }
    }
}