using data.RemoteData.RemoteDatabase.DAO;
using data.Repository;

namespace domain.UseCase;

public class UserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public UserUseCase(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }
    
    private void ValidateFIO(string fio)
    {
        if (string.IsNullOrWhiteSpace(fio))
        {
            throw new ArgumentException("ФИО не может быть пустым.");
        }
    }
    
    private GroupDAO ValidateGroup(int groupId)
    {
        var group = _groupRepository.GetAllGroups()
            .FirstOrDefault(g => g.Id == groupId);

        if (group == null)
        {
            throw new Exception("Группа не найдена.");
        }

        return group;
    }
    
    public List<UserDAO> GetAllStudents()
    {
        return _userRepository.GetAllStudents();
    }

    public bool RemoveStudentById(int id)
    {
        return _userRepository.RemoveStudentById(id);
    }

    public UserDAO? FindStudentById(int id)
    {
        return _userRepository.FindStudentById(id);
    }

    public UserDAO UpdateStudent(int id, string newFio, int groupId)
    {
        ValidateFIO(newFio);
        ValidateGroup(groupId);
        
        UserDAO user = new UserDAO
        {
            UserId = id,
            FIO = newFio,
            GroupId = groupId
        };
        var result = _userRepository.UpdateStudent(user);
        if(result == null) throw new Exception("Обновление студента не удалось.");
        return new UserDAO
        {
            UserId = result.UserId,
            FIO = result.FIO,
            GroupId = result.GroupId
        };
    }
}