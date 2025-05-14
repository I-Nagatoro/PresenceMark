using data.RemoteData.RemoteDatabase.DAO;
namespace data.Repository;

public interface IPresenceRepository
{
    DateOnly? GetLastDateByGroupId(int groupId);
    void SavePresence(List<PresenceDAO> presences);
    List<PresenceDAO> GetPresenceByDateNGroup(DateOnly startDate, DateOnly endDate, int groupId);
    List<PresenceDAO> GetPresenceForAbsent(DateOnly date, int groupId);
    void UpdateAtt(int userId, int groupId, int firstLessonNum, int lastLessonNum, DateOnly date, bool IsAttendance);
    List<PresenceDAO> GetPresenceByGroup(int groupId);
    GroupAttendanceStats GetGeneralPresenceForGroup(int groupId);
    List<PresenceDAO> GetPresenceByUserId(int groupId, DateOnly? date, int? student);
    void DeletePresenceByGroup(int group);
    void ClearAllPresence();
    void AddPresence(List<PresenceDAO> presenceList);
    void UpdatePresence(List<PresenceDAO> attendanceList);
}