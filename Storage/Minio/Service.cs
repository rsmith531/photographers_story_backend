// Storage/Minio/Service.cs

using Minio;
using Storage.Interfaces;

namespace Storage.Minio.Services;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minioClient;

    public MinioStorageService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }
}