// Database/Models/Location.cs


namespace Database.Models;

public class Location
{
    public required string Id { get; set; }
    public required string Place { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
}
