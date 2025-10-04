// Api/Program.cs

// article about OpenAPI admin panels
// https://timdeschryver.dev/blog/what-about-my-api-documentation-now-that-swashbuckle-is-no-longer-a-dependency-in-aspnet-9

using Scalar.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net; // IP address parser
using Database.Mongo.Services;
using Database.Connection;
using Database.Interfaces;
using Storage.Interfaces;
using Storage.Minio.Services;
using Minio;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-9.0
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Add the Docker host's gateway IP to the list of known proxies
    options.KnownProxies.Add(IPAddress.Parse("172.17.0.1"));
});

// create a cache for the generated OpenAPI document
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Conditionally configure the database service based on the environment
if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    // get mongo connection config from appsettings.json
    var mongoSettings = builder.Configuration.GetSection("MongoDbDatabase").Get<Mongo>();

    if (mongoSettings == null)
        throw new ArgumentNullException(nameof(mongoSettings));

    // register a mongo client to the builder
    builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoSettings.ConnectionString));

    builder.Services.AddScoped(sp =>
      sp.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DatabaseName));

    // register the database from the mongo client
    builder.Services.AddSingleton<IDatabaseService, Posts>();

    // TODO: figure out how to get the type safe settings
    // var minioSettings = GetRequiredService<IOptions<MinioConnection>>().Value;

    builder.Services.AddMinio((configureClient) => configureClient
        .WithEndpoint(builder.Configuration["Minio:Endpoint"])
        .WithCredentials(builder.Configuration["Minio:AccessKey"], builder.Configuration["Minio:SecretKey"])
        .WithSSL(false)
        .Build()
    );
    builder.Services.AddScoped<IStorageService, MinioStorageService>();
}

var app = builder.Build();

// This middleware MUST be configured before any other middleware that depends on these headers
app.UseForwardedHeaders();

app.UsePathBase("/api");

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    // cache the generated OpenAPI document in prod
    app.MapOpenApi().CacheOutput();
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Render the OpenAPI document using NSwag's Swagger UI
// Available at https://localhost:{port}/api/swagger
app.UseSwaggerUi(options =>
{
    options.DocumentPath = "/openapi/v1.json";
    options.Path = "/swagger";
});

// Render the OpenAPI document using NSwag's version of Redoc
// Available at https://localhost:{port}/api/redoc
app.UseReDoc(options =>
{
    options.DocumentPath = "/openapi/v1.json";
    options.Path = "/redoc";
});

// Render the OpenAPI document using Scalar
// Available at https://localhost:{port}/api/scalar
app.MapScalarApiReference("/scalar");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
