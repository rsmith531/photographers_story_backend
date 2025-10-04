// Database/IDatabaseService.cs

using Database.Models;

namespace Database.Interfaces;

/// <summary>
/// Defines the contract for all database-related operations,
/// abstracting away the underlying data store technology.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Retrieves all published posts from the database.
    /// </summary>
    /// <returns>A list of published posts.</returns>
    Task<List<Post>> GetPublishedPostsAsync();

    /// <summary>
    /// Gets a single post by its slug.
    /// </summary>
    /// <param name="slug">The URL-friendly slug of the post.</param>
    /// <returns>The post, or null if not found.</returns>
    Task<Post?> GetBySlugAsync(string slug);

    /// <summary>
    /// Creates a new post in the database and returns its generated slug.
    /// </summary>
    /// <param name="newPost">The post to create.</param>
    /// <returns>The generated slug of the newly created post.</returns>
    Task<string> CreatePostAsync(PostDTO newPost);

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="id">The ID of the post to update.</param>
    /// <param name="updatedPost">The updated post object.</param>
    Task UpdatePostAsync(string id, Post updatedPost);

    /// <summary>
    /// Increments the view count for a specific post.
    /// </summary>
    /// <param name="id">The ID of the post.</param>
    Task IncrementViewCountAsync(string id);

    /// <summary>
    /// Gets posts by a specific tag.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>A list of posts with the given tag.</returns>
    Task<List<Post>> GetPostsByTagAsync(string tag);

    // TODO: add publish and unpublish methods
}
