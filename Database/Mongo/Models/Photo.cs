// Database/Mongo/Models/Photo.cs

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Database.Mongo.Models;

public class Photo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonElement("alt_text")]
    public required string AltText { get; set; }

    [BsonElement("public_url")]
    public required string PublicUrl { get; set; }

    [BsonElement("width")]
    public int Width { get; set; }

    [BsonElement("height")]
    public int Height { get; set; }
}