// Database/Mongo/Service.cs

using MongoDB.Driver;
using Database.Interfaces;

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
    public async Task CreatePostAsync(Database.Models.Post newPost)
    {
        var mongoPost = Models.Post.FromCore(newPost);
        await _postsCollection.InsertOneAsync(mongoPost);
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
