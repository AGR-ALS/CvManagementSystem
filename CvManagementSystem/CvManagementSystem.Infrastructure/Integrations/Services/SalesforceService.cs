using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CvManagementSystem.Infrastructure.Integrations.Contracts;
using CvManagementSystem.Infrastructure.Integrations.Settings;
using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.Integrations;
using UserService.Application.Abstractions.Integrations.Models;
using UserService.Application.Abstractions.Integrations.Services;
using UserService.Application.Abstractions.Repositories;

namespace CvManagementSystem.Infrastructure.Integrations.Services;

public class SalesforceService : ISalesforceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISalesforceRecordsRepository _salesforceRecordsRepository;
    private readonly SalesforceSettings _settings;
    
    public SalesforceService(IHttpClientFactory httpClientFactory, IOptions<SalesforceSettings> settings, ISalesforceRecordsRepository salesforceRecordsRepository)
    {
        _httpClientFactory = httpClientFactory;
        _salesforceRecordsRepository = salesforceRecordsRepository;
        _settings = settings.Value;
    }
    
    public async Task CreateCustomerAsync(SalesforceContact contact, SalesforceAccount account, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        Console.WriteLine("1");
        var token = await GetAccessTokenAsync(httpClient);
        Console.WriteLine("2");
        var accountId = await CreateAccountAsync(token, account, httpClient);
        Console.WriteLine("3");
        await CreateContactAsync(token, accountId, contact, httpClient);
        Console.WriteLine("4");
    }

    public async Task CreateCreationRecordAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _salesforceRecordsRepository.CreateCreationRecordAsync(userId, cancellationToken);
    }

    public async Task<bool> GetCreationRecordExistenceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _salesforceRecordsRepository.GetCreationRecordExistenceAsync(userId, cancellationToken);
    }

    private async Task<SalesforceTokenResponse> GetAccessTokenAsync(HttpClient httpClient)
    {
        var requestData = new Dictionary<string, string>
        {
            {
                "grant_type",
                _settings.GrantType
            },
            {
                "client_id",
                _settings.ClientId
            },
            {
                "client_secret",
                _settings.ClientSecret
            }
        };
        
        Console.WriteLine("OLOLOLOLO2L");
        Console.WriteLine(_settings.LoginUrl);
        var response = await httpClient.PostAsync(
            $"{_settings.LoginUrl}/services/oauth2/token",
            new FormUrlEncodedContent(requestData)
        );
        Console.WriteLine("OLOLOLOLO2L3");
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("OLOLOLOLOL");
        Console.WriteLine(responseBody);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error occured while getting oauth token: {responseBody}");
        }
        
        var token =
            JsonSerializer.Deserialize<SalesforceTokenResponse>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                });


        return token?? throw new Exception("Token deserialization returned null");
    }



    private async Task<string> CreateAccountAsync(SalesforceTokenResponse token, SalesforceAccount account, HttpClient httpClient)
    {

        var accountData = new
        {
            Name = account.Name,
            Phone = account.PhoneNumber,
            Website = account.Website,
        };
        
        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                $"{token.InstanceUrl}/services/data/v{_settings.ApiVersion}/sobjects/Account"
            );
        
        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token.AccessToken
            );
        
        request.Content =
            new StringContent(
                JsonSerializer.Serialize(accountData),
                Encoding.UTF8,
                "application/json"
            );

        var response = await httpClient.SendAsync(request);
        
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error occured while creating salesforce account: {responseBody}");
        }
        
        var result =
            JsonSerializer.Deserialize<SalesforceCreateResponse>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                });


        return result?.Id ?? throw new Exception("Account creation deserialization returned null");
    }
    
    private async Task CreateContactAsync(SalesforceTokenResponse token, string accountId, SalesforceContact contact, HttpClient httpClient)
    {
        var contactData = new
        {
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
            Title = contact.Title,
            Phone = contact.PhoneNumber,
            AccountId = accountId
        };

        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                $"{token.InstanceUrl}/services/data/v{_settings.ApiVersion}/sobjects/Contact"
            );
        
        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token.AccessToken
            );
        
        request.Content =
            new StringContent(
                JsonSerializer.Serialize(contactData),
                Encoding.UTF8,
                "application/json"
            );
        
        var response =
            await httpClient.SendAsync(request);
        
        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error occured while creating salesforce contact: {responseBody}");
        }
    }
}