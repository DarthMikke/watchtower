namespace Watchtower.Crawler;
using MongoDB.Driver;

public class Crawler
{
    private IMongoClient client;
    private IMongoDatabase database;
    
    public Crawler(string connectionString) {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        try {
            client = new MongoClient(settings);
        } catch {
            Console.WriteLine("An error occured during connecting to the database. Check your crednetials or network connection.");
            throw new Exception();
        }

        database = client.GetDatabase("Main");

        Console.WriteLine("Connected successfully.");
    }

    
}
