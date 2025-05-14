using data.RemoteData.RemoteDatabase.DAO;
using Microsoft.Extensions.Logging;
using PresenceHTTPClient.Interfaces;
using PresenceHTTPClient.Models;

namespace PresenceHTTPClient.APIClients;

public class GroupAPIClient : BaseAPIClient, IGroupAPIClient
{
    private const string BasePath = "Group";
    
    public GroupAPIClient(IHttpClientFactory httpClientFactory, ILogger<GroupAPIClient> logger) 
        : base(httpClientFactory, logger)
    {
    }

    public async Task<List<GroupDAO>> GetGroupsAsync()
    {
        return await GetAsync<List<GroupDAO>>(BasePath) ?? new List<GroupDAO>();
    }

    public async Task<List<GroupWithStudentDAO>> GetGroupsWithUsersAsync()
    {
        return await GetAsync<List<GroupWithStudentDAO>>("Admin") ?? new List<GroupWithStudentDAO>();
    }

    public async Task RemoveAllUsersFromGroup(int groupId)
    {
        var response = await _httpClient.DeleteAsync($"/api/groups/{groupId}/users");

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Ошибка при удалении студентов из группы {groupId}: {response.StatusCode} - {content}");
        }
    }
}