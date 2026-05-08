 using UnityEngine;

public class Game_manager : MonoBehaviour
{
    public static Game_manager Instance;

    // Datos a recopilar
    public int totalJumps = 0;
    public int totalCoins = 0;
    public int totalScore = 0;
    public string playerName = "";
    public bool bossKilled = false;

    // Referencia a Mondongo para guardar en DB
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

    // Función para guardar datos en MongoDB
    public void SaveGameData()
    {
        if (mondongo != null)
        {
            mondongo.SaveGameData(totalJumps, totalCoins, totalScore, playerName, bossKilled);
        }
        else
        {
            Debug.LogError("Mondongo no encontrado para guardar datos.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
