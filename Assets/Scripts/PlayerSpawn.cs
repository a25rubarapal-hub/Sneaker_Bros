using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        Vector2 pos = GuardadoJava.Cargar();

        // Si no hay datos, no mover
        if (pos != Vector2.zero)
            transform.position = pos;
    }
}
