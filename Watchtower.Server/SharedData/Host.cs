namespace Watchtower.Data;
using MongoDB.Driver;
using MongoDB.Bson;

public class WatchtowerHost: IWatchtowerHost {
    /* Representation of a host. */
    public ObjectId Id { get; set; }
    public string hostname { get; set; }
    public List<WatchtowerResource> Resources { get; set; }
    public IMongoDatabase Database;

    public void BindDatabase(IMongoDatabase database) {
        Database = database;
    }

    public void FetchResources()
    {
        var collection = Database.GetCollection<WatchtowerResource>("Resources");
        var filter = new BsonDocument("host", this.Id);
        Resources = collection.Find(filter).ToList();
    }

    /**
     * -1 – never checked
     * 0 - OK
     * 1 – At least 1 resource's response time is above threshold
     * 2 – At least 1 resource answers with wrong status
     */
    public int CurrentStatus() {
        return 0;
    }
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

    public int CurrentStatus();
    public int DailyUptime();
    public int WeeklyUptime();
}
