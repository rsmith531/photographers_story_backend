// Tests/Database/Builders/Mongo.cs

using Database.Mongo.Models;
using MongoDB.Bson;

namespace Tests.Builders.Mongo;

public class PostBuilder
{
    private readonly Post _post;

    public PostBuilder()
    {
        _post = new Post
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "Default Test Post Title",
            Slug = "default-test-post-slug",
            ArticleContent = "This is the default post body.",
            Author = "Test Person",
            PublishedAt = null,
            Tags = ["photography", "test"],
            CreatedAt = DateTime.UtcNow,
            Location = new Location
            {
                Id = "1",
                Place = "Test City",
                Latitude = 41.1561268114726,
                Longitude = -81.41418385982185
            },
            Summary = "A post for testing purposes"
        };
    }

    public PostBuilder WithTitle(string title)
    {
        _post.Title = title;
        return this;
    }

    public PostBuilder WithSlug(string slug)
    {
        _post.Slug = slug;
        return this;
    }

    public PostBuilder IsPublished()
    {
        _post.PublishedAt = DateTime.UtcNow;
        return this;
    }

    public PostBuilder IsDraft()
    {
        _post.PublishedAt = null;
        return this;
    }

    public Post Build()
    {
        return _post;
    }
}