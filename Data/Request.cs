namespace Watchtower.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class WatchtowerRequest
{
    public ObjectId Id { get; set; }
    [BsonElement("resource")]
    public ObjectId ResourceId { get; set; }
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }
    [BsonElement("status")]
    public int Status { get; set; }
    [BsonElement("responseTime")]
    public int ResponseTime { get; set; }
}