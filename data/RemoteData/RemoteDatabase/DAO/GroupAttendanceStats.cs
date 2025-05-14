namespace data.RemoteData.RemoteDatabase.DAO;

public class GroupAttendanceStats
{
    public int UserCount { get; set; }
    public int TotalLessons { get; set; }
    public double AttendancePercentage { get; set; }
    public List<UserAtt> UserAttendanceDetails { get; set; } = new List<UserAtt>();
}