using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using UnityEngine;

public class Mondongo : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> usersCollection;

    void Start()
    {
        string connectionString = "mongodb+srv://a25rubgonlie_db_user:FzGlr4kakXEMD5qZ@cluster0.8atznye.mongodb.net/SneakerBros?retryWrites=true&w=majority";

        try
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("SneakerBros");
            usersCollection = database.GetCollection<BsonDocument>("users");

            long count = usersCollection.CountDocuments(Builders<BsonDocument>.Filter.Empty);
            Debug.Log("Conexión exitosa. Documentos en 'users': " + count);
        }
        catch (System.Exception e)
        {
            Debug.LogError("MongoDB Connection Error: " + e.Message);
        }
    }

    public void SaveGameData(int jumps, int coins, int score, string name, bool bossKilled)
    {
        try
        {
            var document = new BsonDocument
            {
                { "name", name },
                { "jumps", jumps },
                { "coins", coins },
                { "score", score },
                { "bossKilled", bossKilled }
            };
            usersCollection.InsertOne(document);
            Debug.Log("Datos guardados en MongoDB: " + document.ToJson());
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al guardar datos en MongoDB: " + e.Message);
        }
    }
}
}
