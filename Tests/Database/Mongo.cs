using Moq;
using MongoDB.Driver;
using Database.Mongo.Services;
using Database.Mongo.Models;
using Tests.Builders.Mongo;
using Tests.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Tests.Database;

// unit tests for the MongoDB Posts Service. They aren't supposed to test that
// data in the database is appropriately handled, since that would be testing
// the Mongo driver methods. Instead, we are testing that the correct methods
// are being called by replacing the underlying "database" with mocks so that it
// doesn't rely on an actual database running.

// It should test different things based on the method it is testing:

// assert return values when
// - When your service method transforms, filters, or wraps data before returning.
// - When your method has branching logic based on the mock’s return (e.g., return null if not found).

// verify method calls when 
// - When your method's purpose is to invoke a side effect (e.g., Insert, Update).
// - When you want to ensure the right arguments were passed to the dependency.
// - When your method doesn't return anything (void/Task) and just triggers actions.


public class MongoPostsServiceTests
{
    private readonly Mock<IMongoCollection<Post>> _mockCollection;
    private readonly Mock<IAsyncCursor<Post>> _mockCursor;
    private readonly Posts _service;

    public MongoPostsServiceTests()
    {
        // Set up the mocks
        _mockCollection = new Mock<IMongoCollection<Post>>();
        _mockCursor = new Mock<IAsyncCursor<Post>>();

        // Configure the cursor mock to return a list of items and then signal the end of the collection
        _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Instantiate the service using the mocked collection
        _service = new Posts(_mockCollection.Object);
    }

    [Fact]
    public async Task GetPublishedPostsAsync_Returns_OnlyPublishedPosts()
    {
        // Arrange
        var publishedPost = new Builders.Mongo.PostBuilder().IsPublished().Build();
        var draftPost = new Builders.Mongo.PostBuilder().IsDraft().Build();
        var mockPosts = new List<Post> { publishedPost, draftPost };

        // Configure the cursor mock to return our mock data
        _mockCursor.Setup(_ => _.Current).Returns([publishedPost]);

        // capture the filter used to test it
        FilterDefinition<Post>? capturedFilter = null;

        // Configure the collection mock to return our mock cursor when FindAsync is called
        // We use It.IsAny<T> to make the test less brittle by not depending on the exact filter expression.
        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<FindOptions<Post, Post>>(),
            It.IsAny<CancellationToken>()
        )).Callback((FilterDefinition<Post> filter, FindOptions<Post, Post> _, CancellationToken _) =>
            {
                capturedFilter = filter;
            }).ReturnsAsync(_mockCursor.Object);

        // Act
        var result = await _service.GetPublishedPostsAsync();

        // Assert

        // check the result
        Assert.NotNull(result);
        Assert.Single(result); // We should only get one post (the published one)
        Assert.Equal(publishedPost.Slug, result[0].Slug);
        Assert.NotNull(result[0].PublishedAt);

        // check the filter
        Assert.NotNull(capturedFilter);
        var serializer = BsonSerializer.SerializerRegistry.GetSerializer<Post>();
        var renderedFilter = capturedFilter.Render(new RenderArgs<Post>(serializer, BsonSerializer.SerializerRegistry));

        // Check that the filter contains the correct conditions
        Assert.True(renderedFilter.Contains("published_at"));
        Assert.Equal(BsonNull.Value, renderedFilter["published_at"].AsBsonDocument["$ne"]);

    }

    [Fact]
    public async Task GetBySlugAsync_Returns_CorrectPost()
    {
        // Arrange
        var expectedSlug = "published-post-slug";
        var mockPost = new Builders.Mongo.PostBuilder().IsPublished().WithSlug(expectedSlug).Build();
        _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        _mockCursor.Setup(_ => _.Current).Returns([mockPost]);

        FilterDefinition<Post>? capturedFilter = null;
        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<FindOptions<Post, Post>>(),
            It.IsAny<CancellationToken>()
        )).Callback((FilterDefinition<Post> filter, FindOptions<Post, Post> _, CancellationToken _) =>
    {
        capturedFilter = filter;
    }).ReturnsAsync(_mockCursor.Object);

        // Act
        var result = await _service.GetBySlugAsync(expectedSlug);

        // Assert

        // test the result
        Assert.NotNull(result);
        Assert.Equal(expectedSlug, result.Slug);
        Assert.NotNull(result.PublishedAt); // A post fetched by slug should also be published

        // test the filter
        Assert.NotNull(capturedFilter);
        var serializer = BsonSerializer.SerializerRegistry.GetSerializer<Post>();
        var renderedFilter = capturedFilter.Render(new RenderArgs<Post>(serializer, BsonSerializer.SerializerRegistry));

        // Check that the filter contains the correct conditions
        Assert.Equal(expectedSlug, renderedFilter["slug"].AsString);
        Assert.True(renderedFilter.Contains("published_at"));
        Assert.Equal(BsonNull.Value, renderedFilter["published_at"].AsBsonDocument["$ne"]);
    }

    [Fact]
    public async Task GetBySlugAsync_Returns_NullForNonExistentPost()
    {
        // Arrange
        var slug = "draft-post";
        var draftPost = new Builders.Mongo.PostBuilder().IsDraft().WithSlug(slug).Build();
        // We configure the mock cursor to return no data
        _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockCursor.Setup(_ => _.Current).Returns([]);

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<FindOptions<Post, Post>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(_mockCursor.Object);

        // Act
        var result = await _service.GetBySlugAsync("non-existent-slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySlugAsync_Returns_NullForNonPublishedPost()
    {
        // Arrange
        var expectedSlug = "unpublished-post-slug";

        // We now configure the mock to return the post that would be found
        // by the database query before the service's logic filters it.
        _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        _mockCursor.Setup(_ => _.Current).Returns([]);

        // This setup will cause the Find operation to "find" the mockPost
        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<FindOptions<Post, Post>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(_mockCursor.Object);

        // Act
        var result = await _service.GetBySlugAsync(expectedSlug);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePostAsync_Calls_InsertOneAsync()
    {
        // Arrange
        var mockPost = new Builders.PostBuilder().IsPublished().Build();
        var mongoPost = Post.FromCore(mockPost);

        // Configure the mock to do nothing when InsertOneAsync is called
        _mockCollection.Setup(c => c.InsertOneAsync(
            It.IsAny<Post>(),
            It.IsAny<InsertOneOptions>(),
            It.IsAny<CancellationToken>()
        )).Returns(Task.CompletedTask).Verifiable();

        // Act
        await _service.CreatePostAsync(mockPost);

        // Assert
        // Verify that the InsertOneAsync method was called exactly once with a post object
        _mockCollection.Verify(c => c.InsertOneAsync(
            It.Is<Post>(p => p.Slug == mongoPost.Slug),
            It.IsAny<InsertOneOptions>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task UpdatePostAsync_Calls_ReplaceOneAsync()
    {
        // Arrange
        var mockId = ObjectId.GenerateNewId().ToString();
        var mockPost = new Builders.PostBuilder().IsPublished().Build();
        var updatedMongoPost = Post.FromCore(mockPost);

        _mockCollection.Setup(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<Post>(),
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, new BsonDocument())).Verifiable();

        // Act
        await _service.UpdatePostAsync(mockId, mockPost);

        // Assert
        // Verify that ReplaceOneAsync was called once with the correct filter and updated object
        _mockCollection.Verify(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.Is<Post>(p => p.Id == mockId && p.Title == mockPost.Title),
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task IncrementViewCountAsync_Calls_UpdateOneAsync()
    {
        // Arrange
        var postId = ObjectId.GenerateNewId().ToString();
        FilterDefinition<Post>? capturedFilter = null;
        UpdateDefinition<Post>? capturedUpdate = null;

        _mockCollection.Setup(c => c.UpdateOneAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<UpdateDefinition<Post>>(),
            It.IsAny<UpdateOptions>(),
            It.IsAny<CancellationToken>()
        )).Callback((
            FilterDefinition<Post> filter,
            UpdateDefinition<Post> update,
            UpdateOptions _,
            CancellationToken __
        ) =>
        {
            capturedFilter = filter;
            capturedUpdate = update;
        }).ReturnsAsync(new UpdateResult.Acknowledged(1, 1, null));

        // Act
        await _service.IncrementViewCountAsync(postId);

        // Assert
        Assert.NotNull(capturedFilter);
        Assert.NotNull(capturedUpdate);


        var serializer = BsonSerializer.SerializerRegistry.GetSerializer<Post>();
        var renderedFilter = capturedFilter.Render(new RenderArgs<Post>(serializer, BsonSerializer.SerializerRegistry));
        Assert.Equal(postId, renderedFilter["_id"].ToString());

        var renderedUpdate = capturedUpdate.Render(new RenderArgs<Post>(serializer, BsonSerializer.SerializerRegistry));

        var incDocument = renderedUpdate["$inc"].AsBsonDocument;
        Assert.True(incDocument.Contains("view_count"));
        Assert.Equal(1, incDocument["view_count"].AsInt32);
    }

    [Fact]
    public async Task GetPostsByTagAsync_Returns_CorrectPosts()
    {
        // Arrange
        var tag = "photography";
        var postWithTag = new Builders.Mongo.PostBuilder().IsPublished().Build();
        var anotherPostWithTag = new Builders.Mongo.PostBuilder().IsPublished().WithSlug("another-post").Build();
        var postWithoutTag = new Builders.Mongo.PostBuilder().IsPublished().WithSlug("no-tag-post").Build(); // This one should be filtered out
        var unpublishedPostWithTag = new Builders.Mongo.PostBuilder().IsDraft().WithSlug("unpublished-post").Build(); // This one should be filtered out
        postWithTag.Tags.Add(tag);
        anotherPostWithTag.Tags.Add(tag);
        unpublishedPostWithTag.Tags.Add(tag);

        var mockPosts = new List<Post> { postWithTag, anotherPostWithTag, postWithoutTag, unpublishedPostWithTag };

        _mockCursor.Setup(_ => _.Current).Returns([postWithTag, anotherPostWithTag]);

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<FindOptions<Post, Post>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(_mockCursor.Object);

        // Act
        var result = await _service.GetPostsByTagAsync(tag);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, p => Assert.Contains(tag, p.Tags));
        Assert.All(result, p => Assert.NotNull(p.PublishedAt));
    }

    [Fact]
    public async Task GetPostsByTagAsync_Returns_Empty_When_NoMatchingTag()
    {
        // Arrange
        var tag = "nonexistent-tag";
        var mockPosts = new List<Post>(); // No matches

        _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        _mockCursor.Setup(_ => _.Current).Returns(mockPosts);

        FilterDefinition<Post>? capturedFilter = null;

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<FindOptions<Post, Post>>(),
            It.IsAny<CancellationToken>()
        )).Callback((FilterDefinition<Post> filter, FindOptions<Post, Post> _, CancellationToken _) =>
        {
            capturedFilter = filter;
        }).ReturnsAsync(_mockCursor.Object);

        // Act
        var result = await _service.GetPostsByTagAsync(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);


        Assert.NotNull(capturedFilter);
        var serializer = BsonSerializer.SerializerRegistry.GetSerializer<Post>();
        var renderedFilter = capturedFilter.Render(new RenderArgs<Post>(serializer, BsonSerializer.SerializerRegistry));

        // Check that the filter contains the correct conditions
        Assert.Equal(tag, renderedFilter["tags"].AsString); // Mongo driver is smart enough for elemMatch on simple equality
        Assert.True(renderedFilter.Contains("published_at"));
        Assert.Equal(BsonNull.Value, renderedFilter["published_at"].AsBsonDocument["$ne"]);

    }
}