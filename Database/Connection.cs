// Database/Connection.cs

namespace Database.Connection;

/// <summary>
/// Settings for the MongoDB database connection, to be loaded from configuration.
/// </summary>
public class Mongo
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}
