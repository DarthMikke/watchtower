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
    
    [BsonElement("ignore")]
    public bool? _ignore { get; set; }

    public bool Ignore { get { return _ignore ?? false; } set { _ignore = value; } }
    
    private IMongoDatabase Database;
    public void BindDatabase(IMongoDatabase database) {
        Database = database;
    }

    public void FetchRequests() {
        Console.WriteLine("Fetching requests for resource " + path);
        var collection = Database.GetCollection<WatchtowerRequest>("Requests");
        var filter = new BsonDocument("resource", this.Id);
        Requests = collection.Find(filter).ToList();
        Requests.Sort(delegate(WatchtowerRequest a, WatchtowerRequest b)
        {
            return -a.Timestamp.CompareTo(b.Timestamp);
        }
        );
    }

    /**
     * -1 – never checked
     * 0 - OK
     * 1 – At least 1 resource's response time is above threshold
     * 2 – At least 1 resource answers with wrong status
     */
    public int CurrentStatus {
        get {
            if (Requests.Count == 0) {
                return -1;
            }
            if (Requests.First().Status == expectedStatus && Requests.First().ResponseTime <= expectedResponseTime) {
                return 0;
            }
            if (Requests.First().ResponseTime > expectedStatus) {
                return 1;
            }
            if (Requests.First().Status != expectedStatus) {
                return 2;
            }
            return 0;
        }
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
