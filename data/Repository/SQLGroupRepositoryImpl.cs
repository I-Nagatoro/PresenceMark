using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using Microsoft.EntityFrameworkCore;

namespace data.Repository;

public class SQLGroupRepositoryImpl : IGroupRepository
{
    private readonly RemoteDatabaseContext _db;

    public SQLGroupRepositoryImpl(RemoteDatabaseContext db)
    {
        _db = db;
    }
    public List<GroupDAO> GetAllGroups()
    {
        return _db.groups.Include(g=>g.Users)
            .Select(g=>new GroupDAO
            {
                Id = g.Id,
                Name = g.Name,
                Users = g.Users.Select(u=>new UserDAO
                {
                    UserId = u.UserId,
                    FIO = u.FIO,
                    GroupId = g.Id
                }).ToList()
            }).ToList();
    }

    public bool AddGroup(string groupName)
    {
        var GroupDAO = new GroupDAO
        {
            Id = _db.groups.Max(g => g.Id) + 1,
            Name = groupName,
        };
        _db.groups.Add(GroupDAO);
        _db.SaveChanges();
        return true;
    }

    public bool UpdateGroup(int id, GroupDAO groupExists)
    {
        var group = _db.groups.Include(g => g.Users)
            .FirstOrDefault(g=>g.Id==id);
        if(group == null) return false;
        group.Name = groupExists.Name;
        group.Users = groupExists.Users.Select(u=>new UserDAO
        {
            UserId = u.UserId,
            FIO = u.FIO,
            GroupId = u.GroupId
        }).ToList();
        _db.SaveChanges();
        return true;
    }

    public GroupDAO FindGroupById(int id)
    {
        var group = _db.groups.Include(g => g.Users).FirstOrDefault(g=>g.Id==id);
        if(group == null) return null;
        return new GroupDAO
        {
            Id = group.Id,
            Name = group.Name,
            Users = group.Users.Select(u => new UserDAO
            {
                UserId = u.UserId,
                FIO = u.FIO
            }).ToList()
        };
    }

    public void DeleteGroupById(int groupId)
    {
        var group = _db.groups.Include(g => g.Users).FirstOrDefault(g => g.Id==groupId);
        _db.groups.Remove(group);
        _db.SaveChanges();
    }
}