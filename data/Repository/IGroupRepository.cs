using data.RemoteData.RemoteDatabase.DAO;

namespace data.Repository;

public interface IGroupRepository
{
    List<GroupDAO> GetAllGroups();
    bool AddGroup(string groupName);
    bool UpdateGroup(int id, GroupDAO groupExists);
    GroupDAO FindGroupById(int id);
    void DeleteGroupById(int groupId);
}