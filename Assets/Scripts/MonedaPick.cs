using UnityEngine;

public class MonedaPick : MonoBehaviour
{

    private int puntuacion;
    public TextMesh puntuacionText;
    void Start()
    {
        puntuacion = 0;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScore score = collision.GetComponent<PlayerScore>();

            if (score != null)
            {
                score.SumarMoneda();
            }

            Destroy(gameObject);

        }
    }
}