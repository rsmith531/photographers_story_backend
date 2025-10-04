// Database/Models/Photo.cs

using MongoDB.Bson;
using Microsoft.AspNetCore.Http;

namespace Database.Models;

public class Photo
{
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public required string AltText { get; set; }
    public required string PublicUrl { get; set; }
    public required uint Width { get; set; }
    public required uint Height { get; set; }

    public static Photo Create(PhotoDTO newPhoto)
    {
        return new Photo
        {
            AltText = newPhoto.AltText,
            // TODO: do I do my image upload here or somewhere else?
            PublicUrl = "https://picsum.photos/800/600",
            Width = newPhoto.Width,
            Height = newPhoto.Height
        };
    }
}

public class PhotoDTO
{
    public required IFormFile Image { get; set; }
    public required string AltText { get; set; }
    public required uint Width { get; set; }
    public required uint Height { get; set; }
}