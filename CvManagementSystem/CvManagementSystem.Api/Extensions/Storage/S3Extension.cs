using Amazon.S3;
using Microsoft.Extensions.Options;
using CvManagementSystem.Infrastructure.Files;

namespace UserService.Api.Extensions.Storage;

public static class S3Extension
{
    public static void AddS3Storage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAmazonS3>(storageProvider =>
        {
            var s3StorageSettings = storageProvider.GetRequiredService<IOptions<S3StorageSettings>>().Value;
            var amazonS3Config = new AmazonS3Config
            {
                ServiceURL = s3StorageSettings.ServiceUrl,
                ForcePathStyle = true,
                UseHttp = true,
            };
            
            return new AmazonS3Client(s3StorageSettings.AccessKey, s3StorageSettings.SecretKey, amazonS3Config);
        });
    }
}