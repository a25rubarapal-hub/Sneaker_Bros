using System.Collections;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float Acceleration = 15.0f;
    public float Speed = 1.0f;
    public float JumpForce = 185.0f;
    public float CrouchSpeedMultiplier = 0.4f;

    private Rigidbody2D rb2d;
    private Animator animator;

    private float Horizontal;
    private float TimeBetweenJumps = 0.1f;
    private float LastJump;
    public float Velocity;
    private bool isCrouching;

    // --- VARIABLES OPTIMIZADAS PARA TUBERÍAS ---
    private bool puedeMoverse = true;
    [HideInInspector] public bool sobreTubo = false;
    [HideInInspector] public Transform entradaTubo;
    [HideInInspector] public Transform salidaTubo;
    // -------------------------------------------

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Si estamos en la animación de la tubería, ignoramos el resto del Update
        if (!puedeMoverse) return;

        Horizontal = Input.GetAxisRaw("Horizontal");
        bool enSuelo = Mathf.Abs(rb2d.linearVelocity.y) < 0.05f;
        isCrouching = Input.GetKey(KeyCode.S) && enSuelo;

        // Detectar si queremos entrar al tubo
        if (sobreTubo && isCrouching && entradaTubo != null && salidaTubo != null)
        {
            StartCoroutine(RutinaTuberia());
            return; // Salimos del update para no aplicar más movimiento
        }

        // Físicas y rotación normales
        if (Horizontal > 0.0f) transform.localScale = new Vector3(1, 1, 1);
        else if (Horizontal < 0.0f) transform.localScale = new Vector3(-1, 1, 1);

        if (Horizontal != 0.0f)
            Velocity = Mathf.Clamp(Velocity + Horizontal * Acceleration * Time.deltaTime, -1.0f, 1.0f);
        else
            Velocity -= Velocity * Acceleration * Time.deltaTime;

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && LastJump < Time.time - TimeBetweenJumps && enSuelo && !isCrouching)
        {
            rb2d.AddForce(Vector2.up * JumpForce);
            LastJump = Time.time;
        }

        // Animaciones
        animator.SetBool("EnSuelo", enSuelo);
        animator.SetBool("Agachado", isCrouching);
        animator.SetBool("Corriendo", Mathf.Abs(Velocity) > 0.1f && !isCrouching);
        animator.SetFloat("VelocidadX", Mathf.Abs(Velocity));
        animator.SetFloat("VelocidadY", rb2d.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        if (!puedeMoverse) return;

        float currentSpeed = isCrouching ? Speed * CrouchSpeedMultiplier : Speed;
        rb2d.linearVelocity = new Vector2((Mathf.Abs(Velocity) < 0.01f ? 0.0f : Velocity) * currentSpeed, rb2d.linearVelocity.y);
    }

    // --- LA MEJOR MANERA: LA CORRUTINA ---
    private IEnumerator RutinaTuberia()
    {
        // 1. Preparamos al personaje
        puedeMoverse = false;
        rb2d.simulated = false; // Apagamos las colisiones y la gravedad temporalmente
        Velocity = 0;           // Frenamos en seco

        animator.SetBool("Corriendo", false);
        animator.SetBool("Agachado", true); // Forzamos la animación de agachado
        animator.SetFloat("VelocidadX", 0);

        // Centramos al personaje en el eje X del tubo
        transform.position = new Vector3(entradaTubo.position.x, transform.position.y, transform.position.z);

        float duracionAnimacion = 1.0f; // Segundos que tarda en bajar/subir
        float tiempo = 0;

        // 2. Animación de bajada (Usamos Lerp para un movimiento matemáticamente perfecto)
        Vector3 posicionInicial = transform.position;
        Vector3 posicionOculta = posicionInicial + Vector3.down * 1.5f; // Baja 1.5 unidades

        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.deltaTime;
            transform.position = Vector3.Lerp(posicionInicial, posicionOculta, tiempo / duracionAnimacion);
            yield return null; // Espera al siguiente frame
        }

        // 3. Teletransporte silencioso al tubo de destino (empieza desde abajo para poder subir)
        tiempo = 0;
        posicionInicial = salidaTubo.position + Vector3.down * 1.5f;
        Vector3 posicionFinal = salidaTubo.position;

        transform.position = posicionInicial;

        // 4. Animación de subida
        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.deltaTime;
            transform.position = Vector3.Lerp(posicionInicial, posicionFinal, tiempo / duracionAnimacion);
            yield return null;
        }

        // 5. Devolvemos el control al jugador
        transform.position = posicionFinal; // Aseguramos que termine exactamente en el punto
        animator.SetBool("Agachado", false);
        rb2d.simulated = true; // Encendemos gravedad y colisiones
        puedeMoverse = true;
    }
}