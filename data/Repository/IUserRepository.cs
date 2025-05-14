using data.RemoteData.RemoteDatabase.DAO;

namespace data.Repository;

public interface IUserRepository
{
    List<UserDAO> GetAllStudents();
    bool RemoveStudentById(int id);
    UserDAO? FindStudentById(int id);
    UserDAO UpdateStudent(UserDAO user);
    List<UserDAO> GetAllStudentsByGroupId(int groupId);
    UserDAO AddStudent(UserDAO newUser);
}