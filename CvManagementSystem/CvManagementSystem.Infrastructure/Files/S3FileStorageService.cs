using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.Sevices;

namespace CvManagementSystem.Infrastructure.Files;

public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3StorageSettings _settings;

    public S3FileStorageService(IAmazonS3 s3Client, IOptions<S3StorageSettings> settings)
    {
        _s3Client = s3Client;
        _settings = settings.Value;
    }
    
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var key = $"users/{Guid.NewGuid()}/{fileName}";
        
        var request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.Private
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return key;
    }
    
    public async Task DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = fileKey
        };

        await _s3Client.DeleteObjectAsync(request, cancellationToken);
    }
    
    public async Task<string> GetPresignedUrlAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _settings.BucketName,
            Key = fileKey,
            Expires = DateTime.UtcNow.Add(TimeSpan.FromSeconds(_settings.PresignedUrlExpirationMinutes)),
            Verb = HttpVerb.GET,
            Protocol = Protocol.HTTP
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }
}