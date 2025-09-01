// Tests/PostsController.cs

using Api.Controllers;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Api;

public class PostsControllerTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly PostsController _controller;

    public PostsControllerTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _controller = new PostsController(_mockDatabaseService.Object);
    }

    [Fact]
    public async Task GetPublishedPosts_Returns_Ok_When_Posts_Exist()
    {
        // Arrange
        var posts = new List<Post> { new Builders.PostBuilder().Build(), new Builders.PostBuilder().Build() };
        _mockDatabaseService.Setup(s => s.GetPublishedPostsAsync()).ReturnsAsync(posts);

        // Act
        var result = await _controller.GetPublishedPosts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPosts = Assert.IsType<List<Post>>(okResult.Value);
        Assert.Equal(posts.Count, returnedPosts.Count);
    }

    [Fact]
    public async Task GetPublishedPosts_Returns_NotFound_When_No_Posts_Exist()
    {
        // Arrange
        var posts = new List<Post>();
        _mockDatabaseService.Setup(s => s.GetPublishedPostsAsync()).ReturnsAsync(posts);

        // Act
        var result = await _controller.GetPublishedPosts();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetPostBySlug_Returns_Ok_When_Post_Exists()
    {
        // Arrange
        var slug = "test-post";
        var post = new Builders.PostBuilder().WithSlug(slug).Build();
        _mockDatabaseService.Setup(s => s.GetBySlugAsync(slug)).ReturnsAsync(post);

        // Act
        var result = await _controller.GetPostBySlug(slug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<Post>(okResult.Value);
        Assert.Equal(slug, returnedPost.Slug);
    }

    [Fact]
    public async Task GetPostBySlug_Returns_NotFound_When_Post_Does_Not_Exist()
    {
        // Arrange
        var slug = "non-existent-post";
        _mockDatabaseService.Setup(s => s.GetBySlugAsync(slug)).ReturnsAsync((Post?)null);

        // Act
        var result = await _controller.GetPostBySlug(slug);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreatePost_Returns_CreatedAt_When_Post_Created()
    {
        // Arrange
        var post = new Builders.PostBuilder().Build();
        _mockDatabaseService.Setup(s => s.CreatePostAsync(post)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreatePost(post);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task UpdatePost_Returns_NoContent_When_Post_Updated()
    {
        // Arrange
        var post = new Builders.PostBuilder().Build();
        _mockDatabaseService.Setup(s => s.UpdatePostAsync(post.Id, post)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdatePost(post.Id, post);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task IncrementViewCount_Returns_NoContent_When_Count_Incremented()
    {
        // Arrange
        var post = new Builders.PostBuilder().Build();
        _mockDatabaseService.Setup(s => s.IncrementViewCountAsync(post.Id)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.IncrementViewCount(post.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetPostsByTag_Returns_Ok_When_Posts_Exist()
    {
        // Arrange
        var tag = "test-tag";
        var posts = new List<Post> { new Builders.PostBuilder().WithTags([tag]).Build() };
        _mockDatabaseService.Setup(s => s.GetPostsByTagAsync(tag)).ReturnsAsync(posts);

        // Act
        var result = await _controller.GetPostsByTag(tag);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPosts = Assert.IsType<List<Post>>(okResult.Value);
        Assert.Equal(posts.Count, returnedPosts.Count);
    }

    [Fact]
    public async Task GetPostsByTag_Returns_NotFound_When_No_Posts_Exist()
    {
        // Arrange
        var tag = "non-existent-tag";
        var posts = new List<Post>();
        _mockDatabaseService.Setup(s => s.GetPostsByTagAsync(tag)).ReturnsAsync(posts);

        // Act
        var result = await _controller.GetPostsByTag(tag);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}