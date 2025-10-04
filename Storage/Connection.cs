// Storage/Connection.cs

namespace Storage.Connection;

/// <summary>
/// Settings for the MinIO S3 connection, to be loaded from configuration.
/// </summary>
public class MinioConnection
{
    public required string Endpoint { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
}
