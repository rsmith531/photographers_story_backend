// Storage/IStorageService.cs

// minio API: https://github.com/minio/minio-dotnet/blob/master/Docs/API.md#aws-s3

namespace Storage.Interfaces;

/// <summary>
/// Defines the contract for all storage-related operations, abstracting away
/// the underlying technology.
/// </summary>
public interface IStorageService
{

    /// <summary>
    /// Get a URL that can be used to upload an image to storage.
    /// </summary>
    /// <returns>The URL the client should use to upload their images.</returns>
    Task<string> GetPresignedUploadUrl();

    /// <summary>
    /// Get a URL that can be used to retrieve an image from storage.
    /// </summary>
    /// <param name="objectName">The object name to retrieve the URL for.</param>
    /// <returns>The URL that a client can use to retrieve an image from storage</returns>
    Task<string> GetPresignedImageUrl(string objectName);

    /// <summary>
    /// Removes one or more image objects from storage.
    /// </summary>
    /// <param name="objectNames">The object names of the objects that should be deleted.</param>
    /// <returns>An array of object names that failed to delete.</returns>
    Task<string[]> DeleteImages(string[] objectNames);


}
