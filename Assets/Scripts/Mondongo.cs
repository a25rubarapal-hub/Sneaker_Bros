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
        // Replace <username>, <password>, and <cluster-url> with your actual MongoDB Atlas credentials
        string connectionString = "mongodb+srv://a25rubgonlie_db_user:FzGlr4kakXEMD5qZ@cluster0.8atznye.mongodb.net/?appName=Cluster0";

        try
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("SneakerBros");  // Your database name
            usersCollection = database.GetCollection<BsonDocument>("a25rubgonlie_db_user");  // Example collection
            Debug.Log("Conexión exitosa");
        }
        catch (System.Exception e)
        {
            Debug.LogError("MongoDB Connection Error: " + e.Message);
        }
    }
}
