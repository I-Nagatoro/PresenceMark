using data.RemoteData.RemoteDatabase.DAO;
using data.Repository;

namespace domain.UseCase;

public class GroupUseCase
{
    private readonly IGroupRepository _groupRepository;

    public GroupUseCase(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    private void ValidateGroup(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new ArgumentException("Имя группы не может быть пустым.");
        }
    }
    
    public List<GroupDAO> GetAllGroups()
    {
        return _groupRepository.GetAllGroups();
    }

    public void AddGroup(string groupName)
    {
        ValidateGroup(groupName);
        _groupRepository.AddGroup(groupName);
    }

    public void UpdateGroup(int id, string groupName)
    {
        ValidateGroup(groupName);
        var groupExists = _groupRepository.GetAllGroups().FirstOrDefault(g => g.Id == id);
        groupExists.Name = groupName;
        _groupRepository.UpdateGroup(id, groupExists);
    }

    public string FindGroupById(int id)
    {
        string group = _groupRepository.FindGroupById(id).Name;
        return group;
    }

    public void DeleteGroupById(int groupId)
    {
        _groupRepository.DeleteGroupById(groupId);
    }
}