using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelOpciones;

    void Start()
    {
        panelMenu.SetActive(true);
        panelOpciones.SetActive(false);
    }

    public void AbrirOpciones()
    {
        panelMenu.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void VolverMenu()
    {
        panelMenu.SetActive(true);
        panelOpciones.SetActive(false);
    }
}