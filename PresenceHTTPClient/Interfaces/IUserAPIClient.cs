namespace PresenceHTTPClient.Interfaces;

public interface IUserAPIClient
{
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> DeleteUsersByGroupIdAsync(int groupId);
    Task<bool> CreateUser(string fio, int groupId);
    Task UpdateUser(int userId, string newFio, int newGroupId);
}