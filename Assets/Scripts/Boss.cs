using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class Boss : MonoBehaviour
{
    public enum Estado { Idle, Walk, Attack, Hit, React, Dead }
    private Estado estadoActual = Estado.Idle;

    [Header("Parámetros de Animator")]
    [SerializeField] private string paramCaminar = "Caminar";
    [SerializeField] private string triggerAtacar = "Atacar";
    [SerializeField] private string triggerRecibir = "Recibir";
    [SerializeField] private string triggerReaccionar = "Reaccionar";
    [SerializeField] private string paramMuerto = "Muerto";

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 3f;
    [SerializeField] private string tagPared = "Pared";

    [Header("Duración de Estados")]
    [SerializeField] private float duracionAtaque = 0.8f;
    [SerializeField] private float duracionRecibir = 0.5f;
    [SerializeField] private float duracionReaccionar = 0.3f;
    [SerializeField] private float duracionCaminar = 3f;
    [SerializeField] private float duracionIdle = 1f;

    private Animator animator;
    private int hashCaminar;
    private int hashAtacar;
    private int hashRecibir;
    private int hashReaccionar;
    private int hashMuerto;
    private float timerEstado = 0f;
    private bool movingRight = true;
    private bool caminandoActualmente = false;

    void Start()
    {
        ObtenerComponentes();
        InicializarHashesAnimator();
        ValidarParametrosAnimator(); // <-- nuevo: valida que los parámetros existan
        Debug.Log("Boss: Inicializando en estado Walk");
        CambiarEstado(Estado.Walk);
    }

    void Update()
    {
        ActualizarTiempoEstado();
        ActualizarLogicaEstado();

        if (caminandoActualmente)
            Mover();
    }

    void ObtenerComponentes()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Boss: No se encontró componente Animator en este GameObject");
        else
            Debug.Log("Boss: Animator encontrado correctamente");
    }

    void InicializarHashesAnimator()
    {
        hashCaminar    = Animator.StringToHash(paramCaminar);
        hashAtacar     = Animator.StringToHash(triggerAtacar);
        hashRecibir    = Animator.StringToHash(triggerRecibir);
        hashReaccionar = Animator.StringToHash(triggerReaccionar);
        hashMuerto     = Animator.StringToHash(paramMuerto);
    }

    /// <summary>
    /// Recorre los parámetros del Animator y avisa si algún nombre no coincide.
    /// </summary>
    void ValidarParametrosAnimator()
    {
        if (animator == null) return;

        var nombres = new System.Collections.Generic.HashSet<int>();
        foreach (var p in animator.parameters)
            nombres.Add(p.nameHash);

        void Chequear(int hash, string nombre)
        {
            if (!nombres.Contains(hash))
                Debug.LogError($"Boss: El parámetro '{nombre}' NO existe en el Animator Controller. Revisa el nombre exacto.");
            else
                Debug.Log($"Boss: Parámetro '{nombre}' encontrado OK.");
        }

        Chequear(hashCaminar,    paramCaminar);
        Chequear(hashAtacar,     triggerAtacar);
        Chequear(hashRecibir,    triggerRecibir);
        Chequear(hashReaccionar, triggerReaccionar);
        Chequear(hashMuerto,     paramMuerto);
    }

    void Mover()
    {
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * velocidadMovimiento * Time.deltaTime);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagPared))
            CambiarDireccion();
    }

    void ActualizarTiempoEstado()
    {
        if (timerEstado > 0f)
            timerEstado -= Time.deltaTime;
    }

    void ActualizarLogicaEstado()
    {
        switch (estadoActual)
        {
            case Estado.Idle:
                // FIX: después del idle, vuelve a caminar
                if (timerEstado <= 0f)
                    CambiarEstado(Estado.Walk);
                break;

            case Estado.Walk:
                // FIX: cuando acaba el timer, hace una pausa en Idle antes de volver
                if (timerEstado <= 0f)
                    CambiarEstado(Estado.Idle);
                break;

            case Estado.Attack:
                if (timerEstado <= 0f)
                    CambiarEstado(Estado.Walk);
                break;

            case Estado.Hit:
                if (timerEstado <= 0f)
                    CambiarEstado(Estado.Walk);
                break;

            case Estado.React:
                if (timerEstado <= 0f)
                    CambiarEstado(Estado.Walk);
                break;

            case Estado.Dead:
                caminandoActualmente = false;
                break;
        }
    }

    void CambiarEstado(Estado nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;

        Debug.Log($"Boss: {estadoActual} → {nuevoEstado}");
        estadoActual = nuevoEstado;

        // Detener movimiento y animación de caminar por defecto
        caminandoActualmente = false;        
        if (animator != null)
            animator.SetBool("Caminar", false); // ← string directo

        switch (estadoActual)
        {
            case Estado.Idle:
                timerEstado = duracionIdle;
                break;

            case Estado.Walk:
                caminandoActualmente = true;
                if (animator != null)
                    animator.SetBool("Caminar", true); // ← string directo, sin hash
                timerEstado = Random.Range(2f, 4f);
                break;

            case Estado.Attack:
                if (animator != null)
                    animator.SetTrigger(hashAtacar);
                timerEstado = duracionAtaque;
                break;

            case Estado.Hit:
                if (animator != null)
                    animator.SetTrigger(hashRecibir);
                timerEstado = duracionRecibir;
                break;

            case Estado.React:
                if (animator != null)
                    animator.SetTrigger(hashReaccionar);
                timerEstado = duracionReaccionar;
                break;

            case Estado.Dead:
                if (animator != null)
                    animator.SetBool(hashMuerto, true);
                break;
        }
    }

    // ── API pública ──────────────────────────────────────────────

    public void Atacar()
    {
        if (estadoActual == Estado.Hit || estadoActual == Estado.Dead) return;
        CambiarEstado(Estado.Attack);
    }

    public void RecibirGolpe()
    {
        if (estadoActual == Estado.Dead) return;
        CambiarEstado(Estado.Hit);
    }

    public void Reaccionar()
    {
        if (estadoActual == Estado.Dead) return;
        CambiarEstado(Estado.React);
    }

    public void Morir()
    {
        CambiarEstado(Estado.Dead);
    }

    void CambiarDireccion()
    {
        movingRight = !movingRight;
    }

    public Estado ObtenerEstadoActual() => estadoActual;
}