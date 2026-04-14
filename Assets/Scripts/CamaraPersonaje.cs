using UnityEngine;

public class PlayerCameraLink : MonoBehaviour
{
    [Header("Asignación de Cámara")]
    [Tooltip("Si el código no encuentra la cámara, arrástrala aquí manualmente desde la jerarquía.")]
    public Transform camaraPrincipal;

    [Header("Configuración de la Cámara")]
    public float cameraZ = -10f;
    public float alturaFijaY = 0f;

    [Header("Límites (Estilo Mario)")]
    public float limiteDerecho = 2f;
    public float limiteIzquierdo = 2f;
    public bool modoMarioClasico = true;

    void Start()
    {
        // 1. Si la variable está vacía, intentamos que el código la busque solo
        if (camaraPrincipal == null)
        {
            if (Camera.main != null)
            {
                camaraPrincipal = Camera.main.transform;
            }
            else
            {
                // Si falla, te avisamos en la consola
                Debug.LogWarning("No se detectó la cámara automáticamente. Por favor, arrastra tu cámara al hueco 'Camara Principal' en el script de tu Jugador.");
            }
        }
    }

    void LateUpdate()
    {
        // 2. Medida de seguridad: Si no hay cámara, no hacemos nada para evitar errores
        if (camaraPrincipal == null) return;

        // 3. Lógica de seguimiento estilo Mario
        Vector3 camPos = camaraPrincipal.position;
        float distanciaX = transform.position.x - camPos.x;

        if (distanciaX > limiteDerecho)
        {
            camPos.x = transform.position.x - limiteDerecho;
        }
        else if (!modoMarioClasico && distanciaX < -limiteIzquierdo)
        {
            camPos.x = transform.position.x + limiteIzquierdo;
        }

        camPos.y = alturaFijaY;
        camPos.z = cameraZ;

        // 4. Aplicamos el movimiento
        camaraPrincipal.position = camPos;
    }
}