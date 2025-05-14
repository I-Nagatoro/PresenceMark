using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PresenceHTTPClient.Interfaces;
using PresenceHTTPClient.Models;

namespace PresenceHTTPClient.APIClients;

public class UserAPIClient : BaseAPIClient, IUserAPIClient
{
    private const string BasePath = "api/Admin";

    public UserAPIClient(IHttpClientFactory httpClientFactory, ILogger<UserAPIClient> logger) 
        : base(httpClientFactory, logger)
    {
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            var responce = await _httpClient.DeleteAsync($"{BasePath}/user?userGuid={userId}");
            return responce.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка, {ex.Message}");
            return false;
        }
    }

    public async Task UpdateUser(int userId, string newFio, int newGroupId)
    {
        var userUpdateData = new
        {
            UserId = userId,
            FIO = newFio,
            GroupId = newGroupId
        };

        var response = await _httpClient.PutAsJsonAsync($"/api/users/{userId}", userUpdateData);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Ошибка при обновлении пользователя {userId}: {response.StatusCode} - {content}");
        }
    }
    
    public async Task<bool> DeleteUsersByGroupIdAsync(int groupId)
    {
        try
        {
            var responce = await _httpClient.DeleteAsync($"{BasePath}/usersbygroupid/{groupId}");
            return responce.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при удалении пользователей из группы {groupId}");
            return false;
        }
    }
    

    public async Task<bool> CreateUser(string fio, int groupId)
    {
        try
        {
            var request = new CreateUserRequest { Fio = fio, GroupId = groupId };
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/usercreate", request, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка создания пользователей");
            return false;
        }
    }
}