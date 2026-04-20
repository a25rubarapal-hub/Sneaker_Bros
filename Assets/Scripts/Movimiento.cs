using System.Collections;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float Acceleration = 15.0f;
    public float Speed = 1.0f;
    public float JumpForce = 185.0f;
    public float CrouchSpeedMultiplier = 0.4f;

    private Rigidbody2D Rigidbody2D;
    private Animator animator;

    private float Horizontal;
    private float TimeBetweenJumps = 0.1f;
    private float LastJump;
    public float Velocity;
    private bool isCrouching;

    // --- NUEVO SISTEMA DE TUBERÍAS (SIN TAGS) ---
    private bool enAnimacionTubo = false;
    private Tuberia tuboActual; // Guarda el tubo sobre el que estamos parados
    // --------------------------------------------

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. Si estamos dentro de un tubo, bloqueamos el registro de inputs
        if (enAnimacionTubo) return;

        bool enSuelo = Mathf.Abs(Rigidbody2D.linearVelocity.y) < 0.05f;
        isCrouching = Input.GetKey(KeyCode.S) && enSuelo;

        // 2. Detectar si queremos entrar por la tubería
        if (tuboActual != null && Input.GetKeyDown(KeyCode.S) && enSuelo && tuboActual.tuberiaConectada != null)
        {
            StartCoroutine(ViajeTuberia(tuboActual, tuboActual.tuberiaConectada));
            return; // Salimos del update
        }

        // 3. Tu lógica de movimiento original
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal > 0.0f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (Horizontal < 0.0f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Horizontal != 0.0f)
            Velocity = Mathf.Clamp(Velocity + Horizontal * Acceleration * Time.deltaTime, -1.0f, 1.0f);
        else
            Velocity -= Velocity * Acceleration * Time.deltaTime;

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && LastJump < Time.time - TimeBetweenJumps && enSuelo && !isCrouching)
        {
            Rigidbody2D.AddForce(Vector2.up * JumpForce);
            LastJump = Time.time;
        }

        animator.SetBool("EnSuelo", enSuelo);
        animator.SetBool("Agachado", isCrouching);
        animator.SetBool("Corriendo", Mathf.Abs(Velocity) > 0.1f && !isCrouching);
        animator.SetFloat("VelocidadX", Mathf.Abs(Velocity));
        animator.SetFloat("VelocidadY", Rigidbody2D.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        if (enAnimacionTubo) return; // Evitar aplicar físicas en el tubo

        float currentSpeed = isCrouching ? Speed * CrouchSpeedMultiplier : Speed;
        Rigidbody2D.linearVelocity = new Vector2((Mathf.Abs(Velocity) < 0.01f ? 0.0f : Velocity) * currentSpeed, Rigidbody2D.linearVelocity.y);
    }

    // --- DETECCIÓN DEL TUBO MEDIANTE COMPONENTES (SIN TAGS) ---
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Intentamos obtener el script "Tuberia" del objeto que acabamos de pisar
        Tuberia tuboDetectado = col.GetComponent<Tuberia>();

        // Si no es nulo, significa que este objeto es una tubería
        if (tuboDetectado != null)
        {
            tuboActual = tuboDetectado;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Tuberia tuboDetectado = col.GetComponent<Tuberia>();

        // Si salimos de una tubería y es la misma en la que estábamos parados, la soltamos
        if (tuboDetectado != null && tuboActual == tuboDetectado)
        {
            tuboActual = null;
        }
    }

    // --- LA ANIMACIÓN ESTILO MARIO ---
    private IEnumerator ViajeTuberia(Tuberia entrada, Tuberia salida)
    {
        enAnimacionTubo = true;
        Rigidbody2D.simulated = false; // Apagamos gravedad y colisiones
        Velocity = 0f;

        animator.SetBool("Corriendo", false);
        animator.SetBool("Agachado", true);
        animator.SetFloat("VelocidadX", 0);

        // 1. Centrar en la boca del tubo de entrada
        transform.position = new Vector3(entrada.puntoBoca.position.x, transform.position.y, transform.position.z);

        float tiempo = 0;
        float duracion = 1f; // Segundos que tarda en bajar

        // 2. Bajar lentamente hasta el punto escondido
        Vector3 posInicial = transform.position;
        Vector3 posOculta = new Vector3(posInicial.x, entrada.puntoFondo.position.y, posInicial.z);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            transform.position = Vector3.Lerp(posInicial, posOculta, tiempo / duracion);
            yield return null; // Espera al siguiente frame
        }

        // 3. Teletransporte invisible al fondo de la otra tubería
        transform.position = new Vector3(salida.puntoBoca.position.x, salida.puntoFondo.position.y, transform.position.z);

        // 4. Subir lentamente
        tiempo = 0;
        posInicial = transform.position;
        Vector3 posFuera = new Vector3(posInicial.x, salida.puntoBoca.position.y, posInicial.z);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            transform.position = Vector3.Lerp(posInicial, posFuera, tiempo / duracion);
            yield return null;
        }

        // 5. Devolver el control al jugador
        transform.position = posFuera;
        animator.SetBool("Agachado", false);
        Rigidbody2D.simulated = true;
        enAnimacionTubo = false;
        tuboActual = salida; // Te deja listo en el tubo de salida por si quieres volver a bajar
    }
}