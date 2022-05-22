namespace Watchtower.Data;
using MongoDB.Driver;
using MongoDB.Bson;

public class WatchtowerHost: IWatchtowerHost {
    /* Representation of a host. */
    public ObjectId Id { get; set; }
    public string hostname { get; set; }
    public List<WatchtowerResource> Resources { get; set; }
    private IMongoDatabase Database { get; set; }

    public void BindDatabase(IMongoDatabase database) {
        Database = database;
    }

    public void FetchResources()
    {
        Console.WriteLine("Fetching resources for host " + hostname);
        Resources = new List<WatchtowerResource>();

        var collection = Database.GetCollection<WatchtowerResource>("Resources");
        var filter = new BsonDocument("host", this.Id);
        var resources = collection.Find(filter).ToList();
        foreach (var resource in resources)
        {
            resource.BindDatabase(Database);
            Resources.Add(resource);
        }
    }

    /**
     * -1 – never checked
     * 0 - OK
     * 1 – At least 1 resource's response time is above threshold
     * 2 – At least 1 resource answers with wrong status
     */
    public int CurrentStatus { get {
        return 0;
    } }
    public int DailyUptime() {
        return 0;
    }
    public int WeeklyUptime() {
        return 0;
    }

}

interface IWatchtowerHost {
    public ObjectId Id { get; set; }
    public string hostname { get; set; }
    public List<WatchtowerResource> Resources { get; set; }

    public int CurrentStatus { get; }
    public int DailyUptime();
    public int WeeklyUptime();
}
