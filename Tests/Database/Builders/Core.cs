// Tests/Database/Builders/Mongo.cs

using Database.Models;

namespace Tests.Builders;

public class PostBuilder
{
    private readonly PostDTO _post;

    public PostBuilder()
    {
        _post = new PostDTO
        {
            Title = "Default Test Post Title",
            ArticleContent = "This is the default post body.",
            Author = "Test Person",
            IsPublished = false,
            Tags = ["photography", "test"],
            Location = new LocationDTO
            {
                Place = "Test City",
                Latitude = 41.1561268114726,
                Longitude = -81.41418385982185
            },
            Summary = "A post for testing purposes",
        };
    }

    public PostBuilder WithTitle(string title)
    {
        _post.Title = title;
        return this;
    }

    public PostBuilder WithTags(List<string> tags)
    {
        _post.Tags = tags;
        return this;
    }

    public PostBuilder IsPublished()
    {
        _post.IsPublished = true;
        return this;
    }

    public PostBuilder IsDraft()
    {
        _post.IsPublished = false;
        return this;
    }

    public PostDTO Build()
    {
        return _post;
    }
}