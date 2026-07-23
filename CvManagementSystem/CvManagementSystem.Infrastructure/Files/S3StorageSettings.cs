namespace CvManagementSystem.Infrastructure.Files;

public class S3StorageSettings
{
    public string ServiceUrl { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string BucketName { get; set; } = null!;
    public uint PresignedUrlExpirationMinutes { get; set; }
}