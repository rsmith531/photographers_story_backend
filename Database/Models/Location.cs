// Database/Models/Location.cs

using FluentValidation;
using MongoDB.Bson;

namespace Database.Models;

public class Location
{
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public required string Place { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }

    public static Location Create(LocationDTO newLocation)
    {
        return new Location
        {
            Place = newLocation.Place,
            Latitude = newLocation.Latitude,
            Longitude = newLocation.Longitude
        };
    }
}

public class LocationDTO
{
    public required string Place { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
}

public class LocationDTOValidator : AbstractValidator<LocationDTO>
{
    public LocationDTOValidator()
    {
        RuleFor(location => location.Place).NotNull();
        RuleFor(location => location.Latitude).NotNull().InclusiveBetween(-90, 90);
        RuleFor(location => location.Longitude).NotNull().InclusiveBetween(-180, 180);
    }
}
