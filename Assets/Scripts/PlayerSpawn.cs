using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        Vector2 pos = GuardadoJava.Cargar();
        transform.position = pos;
        Debug.Log("📥 Cargando");
    }
}
