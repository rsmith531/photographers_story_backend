// Database/Mongo/Service.cs

using MongoDB.Driver;
using Database.Interfaces;

namespace Database.Mongo.Services;

public class Posts : IDatabaseService
{
    private readonly IMongoCollection<Models.Post> _postsCollection;

    public Posts(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _postsCollection = database.GetCollection<Models.Post>("posts");
    }

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
        var mongoPost = await _postsCollection.Find(filter).FirstOrDefaultAsync();
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
        var update = Builders<Models.Post>.Update.Inc(p => p.ViewCount, 1);
        await _postsCollection.UpdateOneAsync(filter, update);
    }

    /// <inheritdoc />
    public async Task<List<Database.Models.Post>> GetPostsByTagAsync(string tag)
    {
        var filter = Builders<Models.Post>.Filter.AnyEq(p => p.Tags, tag);
        var mongoPosts = await _postsCollection.Find(filter).ToListAsync();
        return [.. mongoPosts.Select(p => p.ToCore())];
    }
}
