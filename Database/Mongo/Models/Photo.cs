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
    public uint Width { get; set; }

    [BsonElement("height")]
    public uint Height { get; set; }

    /// <summary>
    /// Maps a core Photo model to this MongoDB-specific model.
    /// </summary>
    public static Photo FromCore(Database.Models.Photo corePhoto)
    {
        return new Photo
        {
            Id = corePhoto.Id,
            AltText = corePhoto.AltText,
            PublicUrl = corePhoto.PublicUrl,
            Width = corePhoto.Width,
            Height = corePhoto.Height
        };
    }

    /// <summary>
    /// Maps this MongoDB-specific model to a core Photo model.
    /// </summary>
    public Database.Models.Photo ToCore()
    {
        return new Database.Models.Photo
        {
            Id = Id,
            AltText = AltText,
            PublicUrl = PublicUrl,
            Width = Width,
            Height = Height
        };
    }
}