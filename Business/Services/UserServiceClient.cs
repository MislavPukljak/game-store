using System.Text;
using Business.DTO.Authorization;
using Business.Interfaces;
using Business.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Business.Services;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient> _logger;
    private readonly string _baseUserAddress;

    public UserServiceClient(ILogger<UserServiceClient> logger, IHttpClientFactory httpClientFactory, IOptions<UserServiceClientOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _baseUserAddress = options.Value.BaseAddress;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUserAddress}api/users");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<UserDto>>(content);

            return result;
        }
        else
        {
            _logger.LogError("Failed to get users");

            throw new HttpRequestException("Failed to get users");
        }
    }

    public async Task<AuthServiceResponseDto> PostUserAsync(UserChangeDto userChangeDto)
    {
        var content = new StringContent(JsonConvert.SerializeObject(userChangeDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_baseUserAddress}api/users", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthServiceResponseDto>(responseContent);
            return result;
        }
        else
        {
            _logger.LogError("Failed to post user");

            throw new HttpRequestException("Failed to post user");
        }
    }

    public async Task<AuthServiceResponseDto> PutUserAsync(string originalEmail, UserChangeDto userChangeDto)
    {
        var content = new StringContent(JsonConvert.SerializeObject(userChangeDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_baseUserAddress}api/users?originalEmail={originalEmail}", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthServiceResponseDto>(responseContent);
            return result;
        }

        _logger.LogError("Failed to put user");

        throw new HttpRequestException("Failed to put user");
    }

    public async Task<AuthServiceResponseDto> DeleteUserAsync(string email)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{_baseUserAddress}api/users"),
            Content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json"),
        };

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthServiceResponseDto>(responseContent);
            return result;
        }

        _logger.LogError("Failed to delete user");

        throw new HttpRequestException("Failed to delete user");
    }

    public async Task<UserDto> PostAuthAsync(AuthDto authDto)
    {
        var content = new StringContent(JsonConvert.SerializeObject(authDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_baseUserAddress}api/auth", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserDto>(responseContent);

            return result;
        }
        else
        {
            throw new HttpRequestException("Failed to post user");
        }
    }
}
