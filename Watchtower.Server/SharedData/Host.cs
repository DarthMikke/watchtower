namespace Watchtower.Data;
using MongoDB.Driver;
using MongoDB.Bson;

public class WatchtowerHost {
    /* Representation of a host. */
    public ObjectId Id { get; set; }
    public string hostname { get; set; }

    public List<WatchtowerResource> Resources(IMongoClient client)
    {
        var db = client.GetDatabase("Main");
        var collection = db.GetCollection<WatchtowerResource>("Resources");
        var filter = new BsonDocument("host", this.Id);
        return collection.Find(filter).ToList();
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
