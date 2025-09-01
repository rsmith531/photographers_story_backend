// Database/Mongo/Service.cs

using MongoDB.Driver;
using Database.Interfaces;
using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace Database.Mongo.Services;

public class Posts(IMongoCollection<Models.Post> postsCollection) : IDatabaseService
{

    private readonly IMongoCollection<Models.Post> _postsCollection = postsCollection;

    public Posts(string connectionString, string databaseName) : this(
        new MongoClient(connectionString).GetDatabase(databaseName).GetCollection<Models.Post>("posts")
    )
    { }

    /// <inheritdoc />
    public async Task<List<Database.Models.Post>> GetPublishedPostsAsync()
    {
        var filter = Builders<Models.Post>.Filter.Ne(p => p.PublishedAt, null);
        var mongoPosts = await _postsCollection.Find(filter).ToListAsync();
        return [.. mongoPosts.Select(p => p.ToCore())];
    }

    /// <inheritdoc />
    public async Task<Database.Models.Post?> GetBySlugAsync(string slug)
    {
        var filter = Builders<Models.Post>.Filter.Eq(p => p.Slug, slug);
        var filter2 = Builders<Models.Post>.Filter.Ne(p => p.PublishedAt, null);
        var combinedFilter = Builders<Models.Post>.Filter.And(filter, filter2);

        var mongoPost = await _postsCollection.Find(combinedFilter).FirstOrDefaultAsync();
        return mongoPost?.ToCore();
    }

    /// <inheritdoc />
    public async Task<string> CreatePostAsync(Database.Models.PostDTO newPost)
    {
        // Generate a URL-friendly slug from the title.
        // This converts the title to lowercase, replaces spaces and other whitespace
        // with a single dash, and removes any characters that are not
        // alphanumeric or a dash.
        var slug = newPost.Title.Trim().ToLower();
        slug = Regex.Replace(slug, @"\s+", "-"); // Replace whitespace with a dash
        slug = Regex.Replace(slug, @"[^a-z0-9-]", ""); // Remove invalid chars
        slug = Regex.Replace(slug, @"-+", "-"); // Ensure single dashes

        // Calculate the estimated reading time based on the article's word count.
        var wordCount = newPost.ArticleContent?.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Length ?? 0;

        // TODO: move into a helper function
        // TODO: handle markdown and images in calculation
        // Divide word count by the average reading speed and round to the nearest whole number.
        var readTime = (int)Math.Round((double)wordCount / 265); // Average reading speed in words per minute.

        var post = new Database.Models.Post()
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

        var mongoPost = Models.Post.FromCore(post);
        await _postsCollection.InsertOneAsync(mongoPost);

        return slug;
    }

    /// <inheritdoc />
    public async Task UpdatePostAsync(string id, Database.Models.Post updatedPost)
    {
        var mongoPost = Models.Post.FromCore(updatedPost);
        await _postsCollection.ReplaceOneAsync(p => p.Id == id, mongoPost);
    }

    /// <inheritdoc />
    public async Task IncrementViewCountAsync(string id)
    {
        var filter = Builders<Models.Post>.Filter.Eq(p => p.Id, id);
        var update = Builders<Models.Post>.Update.Inc("ViewCount", 1);
        await _postsCollection.UpdateOneAsync(filter, update);
    }

    /// <inheritdoc />
    public async Task<List<Database.Models.Post>> GetPostsByTagAsync(string tag)
    {
        var filter = Builders<Models.Post>.Filter.AnyEq(p => p.Tags, tag);
        var filter2 = Builders<Models.Post>.Filter.Ne(p => p.PublishedAt, null);
        var combinedFilter = Builders<Models.Post>.Filter.And(filter, filter2);
        var mongoPosts = await _postsCollection.Find(combinedFilter).ToListAsync();
        return [.. mongoPosts.Select(p => p.ToCore())];
    }
}
