using data.RemoteData.RemoteDatabase.DAO;

namespace PresenceHTTPClient.Models;

public class GroupWithStudentDAO
{
    public required int ID{get; set; }
    public required string Name{get; set; }
    public List<UserDAO> Users { get; set; } = new List<UserDAO>();
}