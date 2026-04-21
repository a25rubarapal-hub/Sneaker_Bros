using System.Collections;
using UnityEngine;

public class PozoTeleport : MonoBehaviour
{
    public Transform destino;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Movimiento player = collision.GetComponent<Movimiento>();

        if (player != null && player.IsTeleporting == false && Input.GetKey(KeyCode.S))
        {
            StartCoroutine(Teleport(player));
        }
    }

    private IEnumerator Teleport(Movimiento player)
    {
        player.SetTeleporting(true);
        player.CanMove = false;

        yield return new WaitForSeconds(0.2f);

        player.transform.position = destino.position;

        yield return new WaitForSeconds(1.0f);

        player.CanMove = true;
        player.SetTeleporting(false);
    }
}