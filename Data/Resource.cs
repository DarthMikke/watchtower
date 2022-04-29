namespace Watchtower.Data;
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
}
