using UnityEngine;
using TMPro;
using System.Collections; // Necesario para las corrutinas del rebote

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

    public bool IsTeleporting { get; private set; }
    public bool CanMove = true;

    [Header("Sonido")]
    public AudioClip sonidoSalto;
    [Range(0f, 1f)] public float volumenSalto = 1f;

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void Update()
    {
        // Aseguramos usar 'velocity'
        bool enSuelo = Mathf.Abs(Rigidbody2D.linearVelocity.y) < 0.05f;

        isCrouching = Input.GetKey(KeyCode.S) && enSuelo;

        if (!CanMove)
        {
            animator.SetBool("EnSuelo", enSuelo);
            animator.SetBool("Agachado", false);
            animator.SetBool("Corriendo", false);
            return;
        }

        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal > 0.0f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (Horizontal < 0.0f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Horizontal != 0.0f)
            Velocity = Mathf.Clamp(Velocity + Horizontal * Acceleration * Time.deltaTime, -1.0f, 1.0f);
        else
            Velocity -= Velocity * Acceleration * Time.deltaTime;

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) &&
            LastJump < Time.time - TimeBetweenJumps &&
            enSuelo && !isCrouching)
        {
            Rigidbody2D.AddForce(Vector2.up * JumpForce);
            LastJump = Time.time;

            if (Game_manager.Instance != null)
                Game_manager.Instance.totalJumps++;

            if (sonidoSalto != null)
            {
                audioSource.clip = sonidoSalto;
                audioSource.volume = volumenSalto;
                audioSource.Play();
            }
        }

        animator.SetBool("EnSuelo", enSuelo);
        animator.SetBool("Agachado", isCrouching);
        animator.SetBool("Corriendo", Mathf.Abs(Velocity) > 0.1f && !isCrouching);
        animator.SetFloat("VelocidadX", Mathf.Abs(Velocity));
        animator.SetFloat("VelocidadY", Rigidbody2D.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;

        float currentSpeed = isCrouching ? Speed * CrouchSpeedMultiplier : Speed;

        Rigidbody2D.linearVelocity = new Vector2(
            (Mathf.Abs(Velocity) < 0.01f ? 0.0f : Velocity) * currentSpeed,
            Rigidbody2D.linearVelocity.y
        );
    }

    public void SetTeleporting(bool value)
    {
        IsTeleporting = value;
    }

    // ==========================================
    // M�TODO PARA EL REBOTE DEL HEADSENSOR
    // ==========================================
    public void AplicarRebote(Vector2 fuerzaRebote, float tiempoDeBloqueo)
    {
        if (Rigidbody2D != null)
        {
            // Frenamos en el aire para un rebote limpio
            Rigidbody2D.linearVelocity = Vector2.zero;
            Velocity = 0f; // Reiniciamos tu variable de aceleraci�n para que no te deslices

            // Aplicamos el salto/empuje
            Rigidbody2D.AddForce(fuerzaRebote, ForceMode2D.Impulse);

            // Quitamos el control temporalmente usando tu variable CanMove
            StartCoroutine(BloquearMovimientoRutina(tiempoDeBloqueo));
        }
    }

    private IEnumerator BloquearMovimientoRutina(float tiempo)
    {
        CanMove = false; // Te quita el control
        yield return new WaitForSeconds(tiempo); // Espera el tiempo de bloqueo (0.2s)
        CanMove = true;  // Te devuelve el control
    }
}