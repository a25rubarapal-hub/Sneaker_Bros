using UnityEngine;

public class Game_manager : MonoBehaviour
{
    public static Game_manager Instance;

    public int totalJumps = 0;
    public int totalCoins = 0;
    public int totalScore = 0;
    public string playerName = "";
    public bool bossKilled = false;

    private Mondongo mondongo;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        mondongo = FindFirstObjectByType<Mondongo>();
    }

    public void SaveGameData()
    {
        if (mondongo != null)
        {
            mondongo.GuardarPartida(playerName, totalScore, Mondongo.Instance.enemigosEliminados, totalJumps, bossKilled);
        }
        else
        {
            Debug.LogError("Mondongo no encontrado para guardar datos.");
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}