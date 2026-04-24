using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class JefeMario : MonoBehaviour
{
    enum Estado { Patrulla, Reaccion, Persecucion, Ataque, Espera }
    private Estado estadoActual = Estado.Patrulla;

    [Header("Movimiento")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;
    public float distanciaAtaque = 1.5f;
    private bool isInvincible = false;
   

    public float invincibleTime = 1f;
    [Header("Tiempos")]
    public float tiempoReaccion = 0.5f;
    public float tiempoAtaque = 0.5f;
    public float tiempoEspera = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform jugador;
     public int maxHealth = 67;
      private int currentHealth;
    private bool mirandoDerecha = true;
    private bool jugadorEnRango = false;
    private float tiempoCambioDireccion;

    void Start()
    {
         currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        //rb.gravityScale = 1f;
        //rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        AsignarTiempoPatrulla();
    }

    void Update()
    {
        
        // Control de animación de caminar
        bool mov = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("Caminando", mov);

        if (estadoActual == Estado.Patrulla)
        {
            tiempoCambioDireccion -= Time.deltaTime;
            if (tiempoCambioDireccion <= 0)
            {
                Girar();
                AsignarTiempoPatrulla();
            }
        }

        if (estadoActual == Estado.Persecucion && jugador != null)
        {
            if (jugador.position.x > transform.position.x && !mirandoDerecha) Girar();
            if (jugador.position.x < transform.position.x && mirandoDerecha) Girar();

            float dist = Vector2.Distance(transform.position, jugador.position);
            if (dist <= distanciaAtaque)
                StartCoroutine(SecuenciaAtaque());
        }
        
    }

    void FixedUpdate()
    {
        
        float velY = rb.linearVelocity.y;

        switch (estadoActual)
        {
            case Estado.Patrulla:
                rb.linearVelocity = new Vector2((mirandoDerecha ? velocidadPatrulla : -velocidadPatrulla), velY);
                break;

            case Estado.Persecucion:
                rb.linearVelocity = new Vector2((mirandoDerecha ? velocidadPersecucion : -velocidadPersecucion), velY);
                break;

            case Estado.Reaccion:
            case Estado.Ataque:
            case Estado.Espera:
                rb.linearVelocity = new Vector2(0, velY);
                break;
        }
    }

    IEnumerator SecuenciaReaccion()
    {
        estadoActual = Estado.Reaccion;
        animator.SetTrigger("Reacionar");

        yield return new WaitForSeconds(tiempoReaccion);

        estadoActual = Estado.Persecucion;
    }

    IEnumerator SecuenciaAtaque()
    {
        estadoActual = Estado.Ataque;
        animator.SetTrigger("Atacar");

        yield return new WaitForSeconds(tiempoAtaque);

        estadoActual = Estado.Espera;
        yield return new WaitForSeconds(tiempoEspera);

        estadoActual = jugadorEnRango ? Estado.Persecucion : Estado.Patrulla;
        if (!jugadorEnRango) AsignarTiempoPatrulla();
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void AsignarTiempoPatrulla()
    {
        tiempoCambioDireccion = Random.Range(2f, 5f);
    }
     public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;

        
        if (currentHealth <= 0)
            Destroy(gameObject);

        StartCoroutine(Invincibility());
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (estadoActual == Estado.Patrulla && col.gameObject.CompareTag("Pared"))
        {
            Girar();
            AsignarTiempoPatrulla();
        }
         if (col.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = col.gameObject.GetComponent<PlayerHealth>();

            if (col.contacts[0].normal.y < -0.5f)
            {
                TakeDamage(1);
            }
            else
            {
                player.TakeDamage(1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            jugador = col.transform;
            jugadorEnRango = true;

            if (estadoActual == Estado.Patrulla)
               StartCoroutine(SecuenciaReaccion());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            jugadorEnRango = false;

            if (estadoActual == Estado.Persecucion)
            {
                estadoActual = Estado.Patrulla;
                AsignarTiempoPatrulla();
            }
        }
    }
    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }


}
