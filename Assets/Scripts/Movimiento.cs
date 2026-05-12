using UnityEngine;
using System.Collections;

public class Movimiento : MonoBehaviour
{
    public float Acceleration = 15.0f;
    public float Speed = 1.0f;
    public float JumpForce = 185.0f;
    public float CrouchSpeedMultiplier = 0.4f;

    private Rigidbody2D Rigidbody2D;
    private Animator animator;
    private AudioSource audioSource;

    private float Horizontal;
    private float TimeBetweenJumps = 0.1f;
    private float LastJump;
    public float Velocity;

    private bool isCrouching;
    public bool CanMove = true;

    // --- VARIABLES DE TELEPORT ---
    public bool IsTeleporting { get; private set; }

    [Header("Sonido")]
    public AudioClip sonidoSalto;
    [Range(0f, 1f)] public float volumenSalto = 1f;

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (IsTeleporting) return;

        bool enSuelo = Mathf.Abs(Rigidbody2D.linearVelocity.y) < 0.05f;
        isCrouching = Input.GetKey(KeyCode.S) && enSuelo;

        if (!CanMove)
        {
            ActualizarAnimaciones(enSuelo, false, false);
            return;
        }

        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal != 0.0f)
        {
            transform.localScale = new Vector3(Horizontal > 0 ? 1 : -1, 1, 1);
            Velocity = Mathf.Clamp(Velocity + Horizontal * Acceleration * Time.deltaTime, -1.0f, 1.0f);
        }
        else
        {
            Velocity -= Velocity * Acceleration * Time.deltaTime;
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) &&
            LastJump < Time.time - TimeBetweenJumps && enSuelo && !isCrouching)
        {
            Rigidbody2D.AddForce(Vector2.up * JumpForce);
            LastJump = Time.time;
            if (sonidoSalto) audioSource.PlayOneShot(sonidoSalto, volumenSalto);
        }

        ActualizarAnimaciones(enSuelo, isCrouching, Mathf.Abs(Velocity) > 0.1f);
    }

    private void ActualizarAnimaciones(bool suelo, bool agachado, bool corriendo)
    {
        animator.SetBool("EnSuelo", suelo);
        animator.SetBool("Agachado", agachado);
        animator.SetBool("Corriendo", corriendo);
        animator.SetFloat("VelocidadX", Mathf.Abs(Velocity));
        animator.SetFloat("VelocidadY", Rigidbody2D.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        if (!CanMove || IsTeleporting) return;
        float currentSpeed = isCrouching ? Speed * CrouchSpeedMultiplier : Speed;
        Rigidbody2D.linearVelocity = new Vector2(Velocity * currentSpeed, Rigidbody2D.linearVelocity.y);
    }

    // --- MÉTODOS DE TELEPORT ---
    public void SetTeleporting(bool state)
    {
        IsTeleporting = state;
        if (state)
        {
            Velocity = 0f;
            Rigidbody2D.linearVelocity = Vector2.zero;
        }
    }

    // --- MÉTODOS DEL BOSS ---
    public void AplicarRebote(Vector2 fuerzaRebote, float tiempoDeBloqueo)
    {
        Rigidbody2D.linearVelocity = Vector2.zero;
        Velocity = 0f;
        Rigidbody2D.AddForce(fuerzaRebote, ForceMode2D.Impulse);
        StartCoroutine(BloquearMovimientoRutina(tiempoDeBloqueo));
    }

    private IEnumerator BloquearMovimientoRutina(float tiempo)
    {
        CanMove = false;
        yield return new WaitForSeconds(tiempo);
        CanMove = true;
    }
}