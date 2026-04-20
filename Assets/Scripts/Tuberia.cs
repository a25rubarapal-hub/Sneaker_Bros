using UnityEngine;

public class DetectorTubo : MonoBehaviour
{
    public Movimiento scriptPersonaje;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si detectamos que el objeto tiene la etiqueta "Tuberia"
        if (col.CompareTag("Tuberia"))
        {
            scriptPersonaje.sobreTubo = true;
            scriptPersonaje.entradaTubo = col.transform;

            // Extraemos el script Tuberia para saber a dónde nos lleva
            Tuberia datosTubo = col.GetComponent<Tuberia>();
            if (datosTubo != null)
            {
                scriptPersonaje.salidaTubo = datosTubo.puntoDeSalida;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Tuberia"))
        {
            scriptPersonaje.sobreTubo = false;
            scriptPersonaje.entradaTubo = null;
            scriptPersonaje.salidaTubo = null;
        }
    }
}