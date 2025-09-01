// Database/Models/Post.cs
using System.Text.RegularExpressions;
using MongoDB.Bson;

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
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; } = null;
    public DateTime? PublishedAt { get; set; } = null; // null value indicates the post has not yet been published
    public uint ViewCount { get; } = 0;
    public required Location Location { get; set; }
    public required uint ReadTimeMinutes { get; set; }
    
    public static Post Create(PostDTO newPost)
    {
        // Compute the slug
        var slug = newPost.Title.Trim().ToLower();
        slug = Regex.Replace(slug, @"\s+", "-"); // Replace whitespace with a dash
        slug = Regex.Replace(slug, @"[^a-z0-9-]", ""); // Remove invalid chars
        slug = Regex.Replace(slug, @"-+", "-"); // Ensure single dashes

        // Calculate the estimated reading time
        var wordCount = newPost.ArticleContent?.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Length ?? 0;
        // TODO: move into a helper function
        // TODO: handle markdown and images in
        var readTime = (int)Math.Round((double)wordCount / 265);


        return new Post
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Slug = slug,
            Tags = newPost.Tags,
            Author = newPost.Author,
            Title = newPost.Title,
            Summary = newPost.Summary,
            CoverPhoto = newPost.CoverPhoto,
            Photos = newPost.Photos,
            ArticleContent = newPost.ArticleContent,
            PublishedAt = newPost.PublishedAt ? DateTime.UtcNow : null,
            Location = newPost.Location,
            ReadTimeMinutes = (uint)readTime
        };
    }
}

public class PostDTO
{
    public List<string> Tags { get; set; } = [];
    public required string Author { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }
    public Photo? CoverPhoto { get; set; }
    public List<Photo> Photos { get; set; } = [];
    public required string ArticleContent { get; set; }
    public required bool PublishedAt { get; set; }
    public required Location Location { get; set; }
}
