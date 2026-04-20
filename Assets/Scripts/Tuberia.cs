using UnityEngine;

public class Tuberia : MonoBehaviour
{
    [Header("¿A dónde lleva este tubo?")]
    public Tuberia tuberiaConectada;

    [Header("Posiciones (Objetos Vacíos)")]
    public Transform puntoBoca;  // Donde te paras al entrar / por donde sales
    public Transform puntoFondo; // Hasta donde se esconde el personaje
}