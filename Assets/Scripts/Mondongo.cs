using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;
using System.Collections;

public class Mondongo : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> partidasCollection;

    public static Mondongo Instance;
    public int enemigosEliminados = 0;

    void Awake()
    {
<<<<<<< Updated upstream
        // Replace <username>, <password>, and <cluster-url> with your actual MongoDB Atlas credentials
=======
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

>>>>>>> Stashed changes
        string connectionString = "mongodb+srv://a25rubgonlie_db_user:FzGlr4kakXEMD5qZ@cluster0.8atznye.mongodb.net/?appName=Cluster0";

        try
        {
            client = new MongoClient(connectionString);
<<<<<<< Updated upstream
            database = client.GetDatabase("SneakerBros");  // Your database name
            usersCollection = database.GetCollection<BsonDocument>("a25rubgonlie_db_user");  // Example collection
            Debug.Log("Conexión exitosa");
=======
            database = client.GetDatabase("SneakerBros");
            partidasCollection = database.GetCollection<BsonDocument>("partidas");
            Debug.Log("✅ Conexión exitosa a MongoDB");
>>>>>>> Stashed changes
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ MongoDB Connection Error: " + e.Message);
        }
    }
<<<<<<< Updated upstream
}
=======

    public void GuardarPartida(string nombre, int puntuacion, int enemigos, int saltos, bool boss)
    {
        StartCoroutine(GuardarPartidaCoroutine(nombre, puntuacion, enemigos, saltos, boss));
    }

    IEnumerator GuardarPartidaCoroutine(string nombre, int puntuacion, int enemigos, int saltos, bool boss)
    {
        var documento = new BsonDocument
        {
            { "nombre", nombre },
            { "puntuacion", puntuacion },
            { "enemigos_eliminados", enemigos },
            { "veces_que_salto", saltos },
            { "mato_boss", boss },
            { "fecha", System.DateTime.UtcNow }
        };

        var tarea = partidasCollection.InsertOneAsync(documento);

        yield return new WaitUntil(() => tarea.IsCompleted);

        if (tarea.IsFaulted)
            Debug.LogError("❌ Error al guardar: " + tarea.Exception);
        else
            Debug.Log("✅ Partida guardada: " + nombre + " | " + puntuacion + " pts | Enemigos: " + enemigos + " | Saltos: " + saltos + " | Boss: " + boss);
    }
}
>>>>>>> Stashed changes
