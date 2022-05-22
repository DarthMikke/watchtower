namespace Watchtower.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class WatchtowerResource : IWatchtowerResource {
    /* Representation of a resource. */
    public ObjectId Id { get; set; }
    [BsonElement("host")]
    public ObjectId HostId { get; set; }
    public string method { get; set; }
    public string path { get; set; }
    public int expectedStatus { get; set; }
    public int expectedResponseTime { get; set; }

    public List<WatchtowerRequest> Requests { get; set; }
    
    private IMongoDatabase Database;
    public void BindDatabase(IMongoDatabase database) {
        Database = database;
    }

    public void FetchRequests() {
        Console.WriteLine("Fetching requests for resource " + path);
        var collection = Database.GetCollection<WatchtowerRequest>("Requests");
        var filter = new BsonDocument("resource", this.Id);
        Requests = collection.Find(filter).ToList();
    }
}

interface IWatchtowerResource {
    public ObjectId Id { get; set; }
    public ObjectId HostId { get; set; }
    public string method { get; set; }
    public string path { get; set; }
    public int expectedStatus { get; set; }
    public int expectedResponseTime { get; set; }
    public List<WatchtowerRequest> Requests { get; set; }
}
