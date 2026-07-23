namespace UserService.Api.Settings;

public class FileUploadingSettings
{
    public string[] AllowedExtensions { get; set; } = null!;
    public uint MaxFileSize { get; set; }
}