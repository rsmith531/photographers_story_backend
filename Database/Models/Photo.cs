// Database/Models/Photo.cs

namespace Database.Models;

public class Photo
{
    public required string Id { get; set; }
    public required string AltText { get; set; }
    public required string PublicUrl { get; set; }
    public required uint Width { get; set; }
    public required uint Height { get; set; }
}