// Database/Models/Post.cs
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; } = null;
    public DateTime? PublishedAt { get; set; } = null; // null value indicates the post has not yet been published
    public uint ViewCount { get; set; } = 0;
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
        var wordCount = newPost.ArticleContent.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Length;
        // TODO: move into a helper function
        // TODO: handle markdown
        var readTime = (int)Math.Round((double)wordCount / 265);

        var articlePhotos = new List<Photo>();

        // This block implements the index-based matching logic you requested.
        if (newPost.Photos is not null && newPost.PhotosMetadata.Count != 0)
        {
            if (newPost.Photos.Count != newPost.PhotosMetadata.Count)
            {
                // throw an error or perhaps make a validation rule for this in the API
            }
            else
            {
                for (var i = 0; i < newPost.Photos.Count; i++)
                {
                    var metadata = newPost.PhotosMetadata[i];

                    articlePhotos.Add(Photo.Create(new PhotoDTO
                    {
                        Image = newPost.Photos[i],
                        AltText = metadata.AltText,
                        Width = metadata.Width,
                        Height = metadata.Height
                    }));
                }
            }
        }

        return new Post
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Slug = slug,
            Tags = newPost.Tags,
            Author = newPost.Author,
            Title = newPost.Title,
            Summary = newPost.Summary,
            CoverPhoto = newPost.CoverPhoto != null ? Photo.Create(newPost.CoverPhoto) : null,
            Photos = articlePhotos,
            ArticleContent = newPost.ArticleContent,
            PublishedAt = newPost.IsPublished ? DateTime.UtcNow : null,
            Location = Location.Create(newPost.Location),
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
    public PhotoDTO? CoverPhoto { get; set; }
    public IFormFileCollection? Photos { get; set; }
    public List<PhotoDTO> PhotosMetadata { get; set; } = [];
    public required string ArticleContent { get; set; }
    public required bool IsPublished { get; set; }
    public required LocationDTO Location { get; set; }
}
