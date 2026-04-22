using UnityEngine;

public class PlayerCameraLink : MonoBehaviour
{
    [Header("Asignación de Cámara")]
    public Transform camaraPrincipal;

    [Header("Configuración de la Cámara")]
    public float cameraZ = -10f;

    [Header("Límites (Estilo Mario)")]
    public float limiteDerecho = 2f;
    public float limiteIzquierdo = 2f;
    public bool modoMarioClasico = true;

    [Header("Seguimiento Vertical")]
    public float offsetVertical = 1f;

    private float alturaMinimaCamara;

    void Start()
    {
        if (camaraPrincipal == null)
        {
            if (Camera.main != null)
            {
                camaraPrincipal = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("No se detectó la cámara automáticamente.");
            }
        }

        // Guardamos la altura inicial como límite inferior
        if (camaraPrincipal != null)
        {
            alturaMinimaCamara = camaraPrincipal.position.y;
        }
    }

    void LateUpdate()
    {
        if (camaraPrincipal == null) return;

        Vector3 camPos = camaraPrincipal.position;

        // =========================
        // MOVIMIENTO HORIZONTAL
        // =========================
        float distanciaX = transform.position.x - camPos.x;

        if (distanciaX > limiteDerecho)
        {
            camPos.x = transform.position.x - limiteDerecho;
        }
        else if (!modoMarioClasico && distanciaX < -limiteIzquierdo)
        {
            camPos.x = transform.position.x + limiteIzquierdo;
        }

        // =========================
        // MOVIMIENTO VERTICAL (NUEVO)
        // =========================
        float objetivoY = transform.position.y + offsetVertical;

        // la cámara solo sube, nunca baja por debajo del inicio
        camPos.y = Mathf.Max(alturaMinimaCamara, objetivoY);

        // =========================
        // Z FIJO
        // =========================
        camPos.z = cameraZ;

        camaraPrincipal.position = camPos;
    }
}