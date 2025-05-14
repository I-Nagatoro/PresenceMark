using data.Exceptions;
using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using Microsoft.EntityFrameworkCore;

namespace data.Repository;

public class SQLUserRepositoryImpl : IUserRepository
{
    private readonly RemoteDatabaseContext _db;

    public SQLUserRepositoryImpl(RemoteDatabaseContext db)
    {
        _db = db;
    }

    public List<UserDAO> GetAllStudents()
    {
        return _db.users.OrderBy(u=>u.UserId).AsNoTracking().ToList();
    }

    public bool RemoveStudentById(int id)
    {
        var user = _db.users.FirstOrDefault(u => u.UserId == id);
        if (user == null) throw new UserNotFoundException(id);
        _db.users.Remove(user);
        _db.SaveChanges();
        return true;
    }

    public UserDAO? FindStudentById(int id)
    {
        return _db.users.Include(u=>u.Group)
            .Where(u => u.UserId == id)
            .Select(u=>new UserDAO
                {
                    UserId = u.UserId,
                    FIO = u.FIO,
                    GroupId = u.GroupId,
                    Group = new GroupDAO
                    {
                        Id = u.Group.Id,
                        Name = u.Group.Name,
                    }
                }).AsNoTracking().FirstOrDefault();
    }

    public UserDAO UpdateStudent(UserDAO user)
    {
        var groupExists = _db.groups.Any(g => g.Id == user.GroupId);
        if (!groupExists) throw new GroupNotFoundException(user.GroupId);
        var userExists = _db.users.FirstOrDefault(u => u.UserId == user.UserId);
        if (userExists == null) throw new UserNotFoundException(user.UserId);
        userExists.FIO = user.FIO;
        userExists.GroupId = user.GroupId;
        _db.SaveChanges();
        return userExists;
    }

    public List<UserDAO> GetAllStudentsByGroupId(int groupId)
    {
        List<UserDAO> users = _db.users.Where(u => u.GroupId == groupId).ToList();
        return users;
    }

    public UserDAO AddStudent(UserDAO newUser)
    {
        _db.users.Add(newUser);
        _db.SaveChanges();
        return newUser;
    }
}