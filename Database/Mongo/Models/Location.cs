// Database/Mongo/Models/Location.cs

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Database.Mongo.Models;

public class Location
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // friendly name for the location
    [BsonElement("place")]
    public required string Place { get; set; }

    [BsonElement("latitude")]
    public double Latitude { get; set; }

    [BsonElement("longitude")]
    public double Longitude { get; set; }

    /// <summary>
    /// Maps a core Location model to this MongoDB-specific model.
    /// </summary>
    public static Location FromCore(Database.Models.Location coreLocation)
    {
        return new Location
        {
            Id = coreLocation.Id,
            Place = coreLocation.Place,
            Latitude = coreLocation.Latitude,
            Longitude = coreLocation.Longitude
        };
    }

    /// <summary>
    /// Maps this MongoDB-specific model to a core Location model.
    /// </summary>
    public Database.Models.Location ToCore()
    {
        return new Database.Models.Location
        {
            Id = Id,
            Place = Place,
            Latitude = Latitude,
            Longitude = Longitude
        };
    }
}
