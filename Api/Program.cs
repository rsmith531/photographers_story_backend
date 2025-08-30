// article about OpenAPI admin panels
// https://timdeschryver.dev/blog/what-about-my-api-documentation-now-that-swashbuckle-is-no-longer-a-dependency-in-aspnet-9

using Scalar.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-9.0
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// create a cache for the generated OpenAPI document
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// This middleware MUST be configured before any other middleware that depends on these headers
app.UseForwardedHeaders();

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
    options.Path = "/api/swagger";
});

// Render the OpenAPI document using NSwag's version of Redoc
// Available at https://localhost:{port}/api/redoc
app.UseReDoc(options =>
{
    options.DocumentPath = "/openapi/v1.json";
    options.Path = "/api/redoc";
});

// Render the OpenAPI document using Scalar
// Available at https://localhost:{port}/api/scalar
app.MapScalarApiReference("api/scalar");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
