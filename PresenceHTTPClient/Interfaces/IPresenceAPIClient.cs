using PresenceHTTPClient.Models;

namespace PresenceHTTPClient.Interfaces;

public interface IPresenceAPIClient
{
    Task<PresenceResponse?> GetPresenceAsync(int groupId, string startDate, string endDate);
    Task<bool> DeletePresenceRecords(string date, int lessonNumder, Guid userGuid);
}