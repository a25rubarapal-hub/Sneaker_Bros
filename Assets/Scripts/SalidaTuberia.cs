using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SalidaTuberia : MonoBehaviour
{
    [Header("Arrastra aquí a tu Personaje")]
    public GameObject jugador;

    private Collider2D miCollider;

    void Awake()
    {
        miCollider = GetComponent<Collider2D>();

        // Empieza siendo "fantasma" para que no moleste
        miCollider.isTrigger = true;
    }

    public void ActivarPlataforma()
    {
        // Se vuelve sólido justo al teletransportarse
        miCollider.isTrigger = false;
    }

    void OnCollisionExit2D(Collision2D otro)
    {
        // ¡Sin Tags! Solo comprueba si el que se bajó del bloque es tu jugador
        if (otro.gameObject == jugador)
        {
            miCollider.isTrigger = true;
        }
    }
}