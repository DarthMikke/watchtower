namespace Watchtower.Crawler;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Watchtower.Data;

public class Crawler
{
    public IMongoClient client;
    private IMongoDatabase database;
    public IEnumerable<WatchtowerHost>? hosts;
    public IEnumerable<WatchtowerHost>? Hosts { get { return Realm.Hosts; } }

    public IMongoCollection<WatchtowerRequest> requests;

    private HttpClient http;

    private WatchtowerRealm Realm;
    public int HostCount { get { return Realm.Hosts.Count(); } }

    public Crawler(string connectionString)
    {
        // Initialize MongoDB connection
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        try {
            client = new MongoClient(settings);
        } catch {
            Console.WriteLine("An error occured during connecting to the database. Check your credentials or network connection.");
            throw;
        }

        database = client.GetDatabase("Main");

        Realm = new WatchtowerRealm(connectionString, "Main");
        Realm.Update();
        Console.WriteLine("Connected successfully.");

        requests = database.GetCollection<WatchtowerRequest>("Requests");

        // Initialize HTTP client
        var httpHandler = new HttpClientHandler();
        httpHandler.AllowAutoRedirect = false;
        http = new HttpClient(httpHandler);
        http.DefaultRequestHeaders.Add("User-Agent", "Watchtower");
    }

    public bool canCrawl(WatchtowerResource resource)
    {
        var last = DateTime.Now.AddHours(-0.9);
        var newRequests = database.GetCollection<WatchtowerRequest>("Requests")
            .Find(x => x.ResourceId == resource.Id && x.Timestamp >= last)
            .ToList();

        return newRequests.Count < 1;
    }

    public void LoadHosts()
    {
        try {
            hosts = database.GetCollection<WatchtowerHost>("Hosts")
                .Find(x => true)
                .ToList();
        } catch {
            throw;
        }
    }

    public async Task<WatchtowerRequest?> Poll(WatchtowerHost host, WatchtowerResource resource)
    {
        DateTime start = DateTime.Now;
        HttpResponseMessage response = await http.GetAsync($"http://{host.hostname}{resource.path}");
        var status = (int)response.StatusCode;
        var duration = (int)((DateTime.Now.Ticks - start.Ticks)/TimeSpan.TicksPerMillisecond);

        return new WatchtowerRequest {
            Id = new ObjectId(),
            ResourceId = resource.Id,
            Timestamp = start,
            ResponseTime = duration,
            Status = status
        };
    }

    public void uploadResponse(WatchtowerRequest response)
    {
        requests.InsertOne(response);
    }
}
