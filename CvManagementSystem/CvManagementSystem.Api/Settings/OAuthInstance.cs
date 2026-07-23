namespace UserService.Api.Settings;

public class OAuthInstance
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string RedirectUrl { get; set; } = null!;
    public string CallbackPath { get; set; } = null!;
}