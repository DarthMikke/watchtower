namespace Watchtower.Crawler;
using MongoDB.Driver;
using MongoDB.Bson;
using Watchtower.Data;

public class Crawler
{
    public IMongoClient client;
    private IMongoDatabase database;
    public IEnumerable<WatchtowerHost>? hosts;
    
    public Crawler(string connectionString) {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        try {
            client = new MongoClient(settings);
        } catch {
            Console.WriteLine("An error occured during connecting to the database. Check your credentials or network connection.");
            throw;
        }

        database = client.GetDatabase("Main");

        Console.WriteLine("Connected successfully.");
    }

    

    public void LoadHosts() {
        try {
            hosts = database.GetCollection<WatchtowerHost>("Hosts")
                .Find(x => true)
                .ToList();
        } catch {
            throw;
        }
    }
}
