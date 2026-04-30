using System.Collections;
using UnityEngine;

public class PozoTeleport : MonoBehaviour
{
    public Transform destino;

    [Header("Sonido")]
    public AudioClip sonidoTeleport;
    [Range(0f, 1f)] public float volumen = 1f;

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

        if (sonidoTeleport != null)
        {
            AudioSource audioTemp = gameObject.AddComponent<AudioSource>();
            audioTemp.clip = sonidoTeleport;
            audioTemp.volume = volumen;
            audioTemp.spatialBlend = 0f;
            audioTemp.Play();
            Destroy(audioTemp, sonidoTeleport.length);
        }

        yield return new WaitForSeconds(0.2f);

        player.transform.position = destino.position;

        yield return new WaitForSeconds(0.4f);

        player.CanMove = true;
        player.SetTeleporting(false);
    }
}