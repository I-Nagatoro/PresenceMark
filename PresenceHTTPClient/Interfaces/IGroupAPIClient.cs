using data.RemoteData.RemoteDatabase.DAO;
using PresenceHTTPClient.Models;

namespace PresenceHTTPClient.Interfaces;

public interface IGroupAPIClient
{
    Task<List<GroupDAO>> GetGroupsAsync();
    Task<List<GroupWithStudentDAO>> GetGroupsWithUsersAsync();
    Task RemoveAllUsersFromGroup(int groupId);
}