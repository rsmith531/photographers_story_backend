// Api/Controllers/PostsController.cs

using Microsoft.AspNetCore.Mvc;
using Database.Interfaces;
using Database.Models;

namespace Api.Controllers;

[Route("posts")]
[ApiController]
public class PostsController(IDatabaseService databaseService) : ControllerBase
{
    private readonly IDatabaseService _databaseService = databaseService;

    /// <summary>
    /// Retrieves all published posts from the database.
    /// </summary>
    /// <returns>A list of published posts.</returns>
    [HttpGet("/published")]
    [EndpointSummary("Get all published posts")]
    [ProducesResponseType(typeof(IEnumerable<Post>), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/json")]
    public async Task<IActionResult> GetPublishedPosts()
    {
        var posts = await _databaseService.GetPublishedPostsAsync();

        if (posts.Count == 0)
        {
            return NotFound();
        }

        return Ok(posts);
    }

    /// <summary>
    /// Gets a single post by its URL-friendly slug.
    /// </summary>
    /// <param name="slug">The slug of the post.</param>
    /// <returns>The post, or a 404 Not Found if not found.</returns>
    [HttpGet("{slug}")]
    [EndpointSummary("Get a post by its slug")]
    [ProducesResponseType(typeof(Post), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/json")]
    public async Task<IActionResult> GetPostBySlug(string slug)
    {
        var post = await _databaseService.GetBySlugAsync(slug);

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    /// <summary>
    /// Creates a new post in the database.
    /// </summary>
    /// <param name="newPost">The post object to create.</param>
    /// <returns>A 201 Created response.</returns>
    [HttpPost]
    [EndpointSummary("Create a new post")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<IActionResult> CreatePost([FromBody] Post newPost)
    {
        await _databaseService.CreatePostAsync(newPost);
        return CreatedAtAction(nameof(GetPostBySlug), new { slug = newPost.Slug }, newPost);
    }

    // TODO: make all the params optional and just overwrite the ones provided
    // TODO: handle not found post ids
    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="id">The ID of the post to update.</param>
    /// <param name="updatedPost">The updated post object.</param>
    /// <returns>A 204 No Content response if successful.</returns>
    [HttpPut("{id}")]
    [EndpointSummary("Update a post")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/json")]
    public async Task<IActionResult> UpdatePost(string id, [FromBody] Post updatedPost)
    {
        await _databaseService.UpdatePostAsync(id, updatedPost);
        return NoContent();
    }

    /// <summary>
    /// Increments the view count for a specific post.
    /// </summary>
    /// <param name="id">The ID of the post.</param>
    /// <returns>A 204 No Content response.</returns>
    [HttpPut("{id}/views")]
    [EndpointSummary("Increment a post's view count")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> IncrementViewCount(string id)
    {
        await _databaseService.IncrementViewCountAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Retrieves posts that have a specific tag.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>A list of posts with the given tag.</returns>
    [HttpGet("tagged/{tag}")]
    [EndpointSummary("Get posts by tag")]
    [ProducesResponseType(typeof(IEnumerable<Post>), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, "application/json")]
    public async Task<IActionResult> GetPostsByTag(string tag)
    {
        var posts = await _databaseService.GetPostsByTagAsync(tag);

        if (posts.Count == 0)
        {
            return NotFound();
        }

        return Ok(posts);
    }
}
