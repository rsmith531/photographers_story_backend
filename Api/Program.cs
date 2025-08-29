// article about OpenAPI admin panels
// https://timdeschryver.dev/blog/what-about-my-api-documentation-now-that-swashbuckle-is-no-longer-a-dependency-in-aspnet-9

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// create a cache for the generated OpenAPI document
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

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
// Available at https://localhost:{port}/swagger
app.UseSwaggerUi(options =>
{
    options.DocumentPath = "/openapi/v1.json";
});

// Render the OpenAPI document using NSwag's version of Redoc
// Available at https://localhost:{port}/api-docs
app.UseReDoc(options =>
{
    options.DocumentTitle = "Open API - ReDoc";
    options.SpecUrl("/openapi/v1.json");
});

// Render the OpenAPI document using Scalar
// Available at https://localhost:{port}/scalar/v1
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
