using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CvManagementSystem.Infrastructure.Integrations.Contracts;
using CvManagementSystem.Infrastructure.Integrations.Settings;
using CvManagementSystem.Infrastructure.Integrations.Utility;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.Integrations.Models;
using UserService.Application.Abstractions.Integrations.Services;
using UserService.Application.Exceptions;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace CvManagementSystem.Infrastructure.Integrations.Services;

public class SupportTicketService : ISupportTicketService
{
    private readonly IUsersService _usersService;
    private readonly IPositionsService _positionsService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DropBoxSettings _dropBoxSettings;
    private readonly SupportTicketSettings _supportTicketSettings;

    public SupportTicketService(
        IUsersService usersService,
        IPositionsService positionsService,
        IOptions<DropBoxSettings> dropBoxSettings,
        IOptions<SupportTicketSettings> supportTickerSettings,
        IHttpClientFactory httpClientFactory
    )
    {
        _usersService = usersService;
        _positionsService = positionsService;
        _httpClientFactory = httpClientFactory;
        _dropBoxSettings = dropBoxSettings.Value;
        _supportTicketSettings = supportTickerSettings.Value;
    }

    public async Task CreateSupportTicket(SupportTicket supportTicket, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetUserByIdAsync(supportTicket.ReportedById, cancellationToken);
        Position? position = null;
        if (supportTicket.PositionId != null)
        {
            position = await _positionsService.GetPositionByIdAsync(supportTicket.PositionId.Value, cancellationToken);
        }

        var ticketData = new
        {
            ReportedBy = new
            {
                Name = user.ProfileData.FirstName + " " + user.ProfileData.LastName,
                Email = user.Email,
                Role = user.Role.Name,
            },
            Position = position?.Title,
            Priority = PriorityMapper.MapPriority(supportTicket.Priority),
            Link = supportTicket.PageLink,
            Summary = supportTicket.Summary,
            AdminEmails = _supportTicketSettings.AdminEmails,
        };

        var jsonContent = JsonSerializer.Serialize(ticketData, new JsonSerializerOptions { WriteIndented = true });

        await UploadToDropboxAsync(jsonContent, cancellationToken);
    }

    private async Task UploadToDropboxAsync(string jsonContent, CancellationToken cancellationToken)
    {
        var dropboxAccessToken = await GetAccessTokenAsync(cancellationToken);

        var fileName = $"/{_dropBoxSettings.Folder}/ticket_{Guid.NewGuid()}.json";
        var apiArg = JsonSerializer.Serialize(new { path = fileName, mode = "add", autorename = true, mute = false });

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, _dropBoxSettings.DropboxUploadUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", dropboxAccessToken);
        request.Headers.Add("Dropbox-API-Arg", apiArg);
        var jsonBytes = Encoding.UTF8.GetBytes(jsonContent);
        request.Content = new ByteArrayContent(jsonBytes);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to upload ticket to Dropbox: {error}");
        }
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, _dropBoxSettings.RefreshTokenUrl);

        request.Content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", _dropBoxSettings.GrantType),
            new KeyValuePair<string, string>("refresh_token", _dropBoxSettings.RefreshToken),
            new KeyValuePair<string, string>("client_id", _dropBoxSettings.ClientId),
            new KeyValuePair<string, string>("client_secret", _dropBoxSettings.ClientSecret)
        ]);

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);

        var accessToken = JsonSerializer.Deserialize<DropBoxAccessTokenResponse>(
            jsonResponse,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                
            })?.AccessToken;
        
        return accessToken ?? throw new JsonException("Access token deserialization returned null");
    }
}