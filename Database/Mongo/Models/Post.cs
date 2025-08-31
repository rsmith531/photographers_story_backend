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
    public List<string> Tags { get; set; } = [];

    [BsonElement("author")]
    public required string Author { get; set; }

    [BsonElement("title")]
    public required string Title { get; set; }

    [BsonElement("summary")]
    public required string Summary { get; set; }

    [BsonElement("cover_photo")]
    public Photo? CoverPhoto { get; set; }

    [BsonElement("photos")]
    public List<Photo> Photos { get; set; } = [];

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
    public uint ViewCount { get; set; } = 0;

    [BsonElement("location")]
    public required Location Location { get; set; }

    [BsonElement("read_time_minutes")]
    public uint ReadTimeMinutes { get; set; } = 0;

    /// <summary>
    /// Maps a core Post model to this MongoDB-specific model.
    /// </summary>
    public static Post FromCore(Database.Models.Post corePost)
    {
        return new Post
        {
            Id = corePost.Id,
            Slug = corePost.Slug,
            Tags = corePost.Tags,
            Author = corePost.Author,
            Title = corePost.Title,
            Summary = corePost.Summary,
            CoverPhoto = corePost.CoverPhoto != null ? Photo.FromCore(corePost.CoverPhoto) : null,
            Photos = [.. corePost.Photos.Select(Photo.FromCore)],
            ArticleContent = corePost.ArticleContent,
            CreatedAt = corePost.CreatedAt,
            EditedAt = corePost.EditedAt,
            PublishedAt = corePost.PublishedAt,
            ViewCount = corePost.ViewCount,
            Location = Location.FromCore(corePost.Location),
            ReadTimeMinutes = corePost.ReadTimeMinutes
        };
    }

    /// <summary>
    /// Maps this MongoDB-specific model to a core Post model.
    /// </summary>
    public Database.Models.Post ToCore()
    {
        return new Database.Models.Post
        {
            Id = Id,
            Slug = Slug,
            Tags = Tags,
            Author = Author,
            Title = Title,
            Summary = Summary,
            CoverPhoto = CoverPhoto?.ToCore(),
            Photos = [.. Photos.Select(p => p.ToCore())],
            ArticleContent = ArticleContent,
            CreatedAt = CreatedAt,
            EditedAt = EditedAt,
            PublishedAt = PublishedAt,
            ViewCount = ViewCount,
            Location = Location.ToCore(),
            ReadTimeMinutes = ReadTimeMinutes
        };
    }
}
