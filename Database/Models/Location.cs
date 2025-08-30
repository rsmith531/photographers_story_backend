// Database/Models/Location.cs


namespace Database.Models;

public class Location
{
    public string? Id { get; set; }
    public required string Place { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
