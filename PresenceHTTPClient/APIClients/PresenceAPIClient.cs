using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PresenceHTTPClient.Interfaces;
using PresenceHTTPClient.Models;

namespace PresenceHTTPClient.APIClients;

public class PresenceAPIClient : BaseAPIClient, IPresenceAPIClient
{
    private const string BasePath = "api/Presence";

    public PresenceAPIClient(IHttpClientFactory httpClientFactory, ILogger<PresenceAPIClient> logger) 
        : base(httpClientFactory, logger)
    {
    }

    public async Task<PresenceResponse?> GetPresenceAsync(int groupId, string startDate, string endDate)
    {
        try
        {
            var url = $"{BasePath}?groupID={groupId}&start={startDate}&end={endDate}";
            _logger.LogInformation($"Отправка запроса на: {url}");
    
            return await _httpClient.GetFromJsonAsync<PresenceResponse>(url);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении посещаемости");
            throw;
        }
    }

    public async Task<bool> DeletePresenceRecords(string date, int lessonNumder, Guid userGuid)
    {
        return await DeleteAsync($"{BasePath}/records/?date={date}&lessonNumber={lessonNumder}&userGuid={userGuid}");
    }
}