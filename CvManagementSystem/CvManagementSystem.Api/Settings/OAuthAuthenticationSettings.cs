namespace UserService.Api.Settings;

public class OAuthAuthenticationSettings
{
    public OAuthInstance Google { get; set; } = null!;
    public OAuthInstance Facebook { get; set; } = null!;
}