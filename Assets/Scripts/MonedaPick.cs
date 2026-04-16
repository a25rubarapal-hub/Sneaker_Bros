using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonedaPick : MonoBehaviour
{
    private int puntuacion;
    public TextMeshProUGUI puntuacionText;
    void Start()
    {
        puntuacion = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

       if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            puntuacion++;
            puntuacionText.text = puntuacion.ToString();
        }
       
    }
    
}
