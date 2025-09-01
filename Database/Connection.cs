// Database/Connection.cs

namespace Database.Connection;

/// <summary>
/// Settings for the MongoDB database connection, to be loaded from configuration.
/// </summary>
public class Mongo
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}
