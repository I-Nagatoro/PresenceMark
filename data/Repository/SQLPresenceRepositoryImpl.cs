using data.RemoteData.RemoteDataBase;
using data.RemoteData.RemoteDatabase.DAO;
using DocumentFormat.OpenXml.Office2019.Excel.RichData;

namespace data.Repository;

public class SQLPresenceRepositoryImpl : IPresenceRepository
{
    private readonly RemoteDatabaseContext _db;

    public SQLPresenceRepositoryImpl(RemoteDatabaseContext db)
    {
        _db = db;
    }
    public DateOnly? GetLastDateByGroupId(int groupId)
    {
        var lastDate = _db.presences.Where(p=>p.GroupId == groupId)
            .OrderByDescending(p=>p.Date)
            .Select(p=>p.Date).FirstOrDefault();
        return lastDate == default ? (DateOnly?)null : lastDate;
    }

    public void SavePresence(List<PresenceDAO> presences)
    {
        foreach (var presence in presences)
        {
            _db.presences.Add(presence);
        }
        _db.SaveChanges();
    }

    public List<PresenceDAO> GetPresenceByDateNGroup(DateOnly startDate, DateOnly endDate, int groupId)
    {
        return _db.presences.Where(p=>p.Date>=startDate && p.Date<=endDate &&
                                      _db.users.Any(u=>u.GroupId==groupId && u.UserId==p.UserId)).ToList();
    }

    public List<PresenceDAO> GetPresenceForAbsent(DateOnly date, int groupId)
    {
        return _db.presences.Where(p => p.GroupId == groupId && p.Date == date).ToList();
    }

    public void UpdateAtt(int userId, int groupId, int firstLessonNum, int lastLessonNum, DateOnly date, bool IsAttendance)
    {
        var presences = _db.presences
            .Where(p => p.UserId == userId
                        && p.GroupId == groupId
                        && p.LessonNumber >= firstLessonNum
                        && p.LessonNumber <= lastLessonNum
                        && p.Date == date)
            .ToList();
        foreach (var presence in presences)
        {
            presence.IsAttendance = IsAttendance;
        }
        _db.SaveChanges();
    }

    public List<PresenceDAO> GetPresenceByGroup(int groupId)
    {
        return _db.presences.Where(p => p.GroupId == groupId)
            .OrderBy(p => p.Date)
            .ThenBy(p => p.UserId).ToList();
    }

    public GroupAttendanceStats GetGeneralPresenceForGroup(int groupId)
    {
        var presences = _db.presences.Where(p => p.GroupId == groupId)
                .OrderBy(p => p.LessonNumber).ToList();
            var dates = _db.presences;
            var distDates = dates.Select(p => p.Date).Distinct().ToList();
            int lesId = 0;
            int lesNum = 1;
            double att = 0;
            int days = -1;
            int countAllLes = 0;
            DateOnly date = DateOnly.MinValue;
            List<int> usersId = new List<int>();
            foreach (var presence in presences)
            {
                if (!usersId.Contains(presence.UserId))
                {
                    usersId.Add(presence.UserId);
                }
                if (presence.Date != date)
                {
                    date = presence.Date;
                    lesId++;
                    lesNum = presence.LessonNumber;
                    days++;
                }
                if (presence.LessonNumber != lesNum && date == presence.Date)
                {
                    lesNum = presence.LessonNumber;
                    countAllLes++;
                    lesId++;
                }
                if (presence.IsAttendance)
                {
                    att++;
                }
            }

            List<UserAtt> a = new List<UserAtt>();
            List<int> ids = new List<int>();
            double ok = 0;
            double skip = 0;
            int userId = 0;
            foreach (var user in usersId)
            {
                var users = _db.presences.Where(p => p.UserId == user);
                foreach (var usera in users)
                {
                    userId = usera.UserId;
                    if (!ids.Contains(usera.UserId))
                    {
                        skip = 0;
                        ok = 0;
                        ids.Add(userId);
                        a.Add(new UserAtt { UserId = userId, Attended = ok, Missed = skip });
                        userId = usera.UserId;
                        if (usera.IsAttendance)
                        {
                            a.First(a => a.UserId == usera.UserId).Attended = ok += 1;
                        }
                        else
                        {
                            a.First(a => a.UserId == usera.UserId).Missed = skip += 1;
                        }
                    }
                    else
                    {
                        if (usera.IsAttendance)
                        {
                            a.First(a => a.UserId == usera.UserId).Attended = ok += 1;
                        }
                        else
                        {
                            a.First(a => a.UserId == usera.UserId).Missed = skip += 1;
                        }
                    }
                }
            }
            var statistics = new GroupAttendanceStats
            {
                UserCount = usersId.Count,
                TotalLessons = lesId,
                AttendancePercentage = att / usersId.Count / lesNum / distDates.Count() * 100
            };

            foreach (var user in a)
            {
                statistics.UserAttendanceDetails.Add(new UserAtt
                {
                    UserId = user.UserId,
                    Attended = user.Attended,
                    Missed = user.Missed,
                    AttPersent = user.Attended / (user.Missed + user.Attended) * 100
                });
            }

            return statistics;
    }

    public List<PresenceDAO> GetPresenceByUserId(int groupId, DateOnly? date, int? student)
    {
        var query = _db.presences.Where(p => p.GroupId == groupId);
        if (date.HasValue) query = query.Where(p => p.Date == date.Value);
        if(student.HasValue) query = query.Where(p=> p.UserId == student.Value);
        return query.ToList();
    }

    public void DeletePresenceByGroup(int group)
    {
        var attendances = _db.presences.Where(p => p.GroupId == group).ToList();
        _db.presences.RemoveRange(attendances);
        _db.SaveChanges();
    }

    public void ClearAllPresence()
    {
        var allAtt=_db.presences.ToList();
        _db.presences.RemoveRange(allAtt);
        _db.SaveChanges();
    }

    public void AddPresence(List<PresenceDAO> presenceList)
    {
        _db.presences.AddRange(presenceList);
        _db.SaveChanges();
    }

    public void UpdatePresence(List<PresenceDAO> attendanceList)
    {
        foreach (var presence in attendanceList)
        {
            var existing = _db.presences.FirstOrDefault(p => 
                    p.Date == presence.Date && p.UserId == presence.UserId && p.LessonNumber == presence.LessonNumber);
            if (existing != null) existing.IsAttendance = presence.IsAttendance;
        }
        _db.SaveChanges();
    }
}