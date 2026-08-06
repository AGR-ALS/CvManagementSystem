namespace CvManagementSystem.Infrastructure.Integrations.Settings;

public class DropBoxSettings
{
    public string DropboxUploadUrl { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string Folder { get; set; } = null!;
    public string RefreshTokenUrl {get; set; } = null!;
    public string ClientId {get; set; } = null!;
    public string ClientSecret {get; set; } = null!;
    public string GrantType {get; set; } = null!;
}