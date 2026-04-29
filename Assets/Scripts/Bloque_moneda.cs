using UnityEngine;

public class BloqueMoneda : MonoBehaviour
{
    public GameObject monedaPrefab;
    public Transform puntoSpawn;
    private bool usado = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (usado) return;

        if (!collision.gameObject.CompareTag("Player")) return;

        // comprobar que el golpe viene desde abajo
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            usado = true;

            if (monedaPrefab != null && puntoSpawn != null)
            {
                Instantiate(monedaPrefab, puntoSpawn.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Falta monedaPrefab o puntoSpawn asignado");
            }
        }
    }
}