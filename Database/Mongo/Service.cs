// Database/Mongo/Service.cs

using MongoDB.Driver;
using Database.Mongo.Models;

namespace Database.Mongo.Services;

public class Posts
{
    private readonly IMongoCollection<Post> _postsCollection;

    public Posts(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _postsCollection = database.GetCollection<Post>("posts");
    }

    /// <summary>
    /// Retrieves all published posts from the database.
    /// </summary>
    /// <returns>A list of published posts.</returns>
    public async Task<List<Post>> GetPublishedPostsAsync()
    {
        var filter = Builders<Post>.Filter.Ne(p => p.PublishedAt, null);
        return await _postsCollection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Gets a single post by its slug.
    /// </summary>
    /// <param name="slug">The URL-friendly slug of the post.</param>
    /// <returns>The post, or null if not found.</returns>
    public async Task<Post> GetBySlugAsync(string slug)
    {
        var filter = Builders<Post>.Filter.Eq(p => p.Slug, slug);
        return await _postsCollection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Creates a new post in the database.
    /// </summary>
    /// <param name="newPost">The post to create.</param>
    public async Task CreatePostAsync(Post newPost)
    {
        await _postsCollection.InsertOneAsync(newPost);
    }

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="id">The ID of the post to update.</param>
    /// <param name="updatedPost">The updated post object.</param>
    public async Task UpdatePostAsync(string id, Post updatedPost)
    {
        await _postsCollection.ReplaceOneAsync(p => p.Id == id, updatedPost);
    }

    /// <summary>
    /// Deletes a post from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the post to delete.</param>
    public async Task DeletePostAsync(string id)
    {
        var filter = Builders<Post>.Filter.Eq(p => p.Id, id);
        await _postsCollection.DeleteOneAsync(filter);
    }

    /// <summary>
    /// Increments the view count for a specific post.
    /// </summary>
    /// <param name="id">The ID of the post.</param>
    public async Task IncrementViewCountAsync(string id)
    {
        var filter = Builders<Post>.Filter.Eq(p => p.Id, id);
        var update = Builders<Post>.Update.Inc(p => p.ViewCount, 1);
        await _postsCollection.UpdateOneAsync(filter, update);
    }

    /// <summary>
    /// Gets posts by a specific tag.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>A list of posts with the given tag.</returns>
    public async Task<List<Post>> GetPostsByTagAsync(string tag)
    {
        var filter = Builders<Post>.Filter.AnyEq(p => p.Tags, tag);
        return await _postsCollection.Find(filter).ToListAsync();
    }
}
