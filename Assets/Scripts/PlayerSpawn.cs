using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        GuardadoSQLite.Init();              // 1) Abrir la BD
        Vector2 pos = GuardadoSQLite.Cargar(); // 2) Leer posición
        if (pos != Vector2.zero)
            transform.position = pos;       // 3) Mover al jugador
    }
}
