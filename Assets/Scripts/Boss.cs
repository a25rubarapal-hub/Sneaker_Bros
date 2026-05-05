using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Boss : MonoBehaviour
{
    private enum Estado { Idle, Walk, Attack, Hit }
    private Estado estadoActual = Estado.Idle;

    [Header("Parámetros de Animator")]
    [SerializeField] private string paramCaminando = "Caminando";
    [SerializeField] private string triggerAtacar = "Atacar";
    [SerializeField] private string triggerGolpe = "Golpe";

    [Header("Duración de Estados")]
    [SerializeField] private float duracionAtaque = 0.8f;
    [SerializeField] private float duracionGolpe = 0.3f;
    [SerializeField] private float duracionCaminar = 3f;

    private Animator animator;
    private int hashCaminando;
    private int hashAtacar;
    private int hashGolpe;
    private float timerEstado = 0f;

    void Start()
    {
        ObtenerAnimator();
        InicializarHashesAnimator();
        CambiarEstado(Estado.Idle);
    }

    void Update()
    {
        ActualizarTiempoEstado();
        ActualizarLogicaEstado();
    }

    void ObtenerAnimator()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Boss: No se encontró componente Animator en este GameObject");
        }
    }

    void InicializarHashesAnimator()
    {
        hashCaminando = Animator.StringToHash(paramCaminando);
        hashAtacar = Animator.StringToHash(triggerAtacar);
        hashGolpe = Animator.StringToHash(triggerGolpe);
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
                break;

            case Estado.Walk:
                if (timerEstado <= 0f)
                    CambiarEstado(Estado.Idle);
                break;

            case Estado.Attack:
                if (timerEstado <= 0f)
                    CambiarEstado(Estado.Idle);
                break;

            case Estado.Hit:
                if (timerEstado <= 0f)
                    CambiarEstado(Estado.Idle);
                break;
        }
    }

    void CambiarEstado(Estado nuevoEstado)
    {
        if (estadoActual == nuevoEstado)
            return;

        estadoActual = nuevoEstado;

        switch (estadoActual)
        {
            case Estado.Idle:
                ResetearTriggers();
                break;

            case Estado.Walk:
                ActivarParametroBool(hashCaminando, true);
                timerEstado = duracionCaminar;
                break;

            case Estado.Attack:
                ActivarParametroBool(hashCaminando, false);
                ActivarTrigger(hashAtacar);
                timerEstado = duracionAtaque;
                break;

            case Estado.Hit:
                ActivarParametroBool(hashCaminando, false);
                ActivarTrigger(hashGolpe);
                timerEstado = duracionGolpe;
                break;
        }
    }

    void ActivarTrigger(int hashTrigger)
    {
        if (animator == null) return;
        animator.SetTrigger(hashTrigger);
    }

    void ActivarParametroBool(int hashParametro, bool valor)
    {
        if (animator == null) return;
        animator.SetBool(hashParametro, valor);
    }

    void ResetearTriggers()
    {
        if (animator == null) return;
        animator.ResetTrigger(hashAtacar);
        animator.ResetTrigger(hashGolpe);
        animator.SetBool(hashCaminando, false);
    }

    public void Atacar()
    {
        if (estadoActual == Estado.Hit) return;
        CambiarEstado(Estado.Attack);
    }

    public void Caminar()
    {
        CambiarEstado(Estado.Walk);
    }

    public void RecibirGolpe()
    {
        CambiarEstado(Estado.Hit);
    }

    public Estado ObtenerEstadoActual()
    {
        return estadoActual;
    }
}
