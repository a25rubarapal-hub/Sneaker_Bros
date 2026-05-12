using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Vector3 pos = col.transform.position;
            GuardadoJava.Guardar(pos.x, pos.y);
            Debug.Log("Guardado en MySQL vía Java: " + pos);
        }
    }
}
