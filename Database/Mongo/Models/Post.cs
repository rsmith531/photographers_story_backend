// Database/Mongo/Models/Post.cs

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Database.Mongo.Models;

public class Post
{
    // string in C# but stored as an ObjectId in Mongo
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    // URL-friendly
    [BsonElement("slug")]
    public required string Slug { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new List<string>();

    [BsonElement("author")]
    public required string Author { get; set; }

    [BsonElement("title")]
    public required string Title { get; set; }

    [BsonElement("summary")]
    public required string Summary { get; set; }

    [BsonElement("cover_photo")]
    public Photo? CoverPhoto { get; set; }

    [BsonElement("photos")]
    public List<Photo> Photos { get; set; } = new List<Photo>();

    // as markdown
    [BsonElement("article_content")]
    public required string ArticleContent { get; set; }

    [BsonElement("created_at")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("edited_at")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? EditedAt { get; set; }

    // null value indicates the post has not yet been published
    [BsonElement("published_at")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? PublishedAt { get; set; }

    [BsonElement("view_count")]
    public int ViewCount { get; set; } = 0;

    [BsonElement("location")]
    public required Location Location { get; set; }

    [BsonElement("read_time_minutes")]
    public int ReadTimeMinutes { get; set; } = 0;
}
