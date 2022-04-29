namespace Watchtower.Data;
using MongoDB.Driver;
using MongoDB.Bson;

public class WatchtowerHost {
    /* Representation of a host. */
    public ObjectId Id { get; set; }
    public string hostname { get; set; }

    public List<Resource> Resources(MongoClient client) {
        var db = client.GetDatabase("Main");
        var collection = db.GetCollection<Resource>("Resources");
        var filter = new BsonDocument("host", this.Id);
        return collection.Find(filter).ToList();
    }
}
