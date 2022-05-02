namespace Watchtower.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class WatchtowerResource {
    /* Representation of a resource. */
    public ObjectId Id { get; set; }
    [BsonElement("host")]
    public ObjectId HostId { get; set; }
    public string method { get; set; }
    public string path { get; set; }
    public int expectedStatus { get; set; }
    public int expectedResponseTime { get; set; }

    public List<WatchtowerRequest> Requests(IMongoClient client)
    {
        var db = client.GetDatabase("Main");
        var collection = db.GetCollection<WatchtowerRequest>("Requests");
        var filter = new BsonDocument("resource", this.Id);
        return collection.Find(filter).ToList();
    }
}
