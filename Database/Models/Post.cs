// Database/Models/Post.cs

namespace Database.Models;

public class Post
{
    public required string Id { get; set; }
    public required string Slug { get; set; }
    public List<string> Tags { get; set; } = [];
    public required string Author { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }
    public Photo? CoverPhoto { get; set; }
    public List<Photo> Photos { get; set; } = [];
    public required string ArticleContent { get; set; } // as markdown
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public DateTime? PublishedAt { get; set; } // null value indicates the post has not yet been published
    public int ViewCount { get; set; } = 0;
    public required Location Location { get; set; }
    public int ReadTimeMinutes { get; set; } = 0;
}
