namespace data.RemoteData.RemoteDatabase.DAO;

public class AttRec
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public DateOnly Date { get; set; }
    public bool IsAttedance { get; set; }
    public int LessonNumber { get; set; }
    public string GroupName { get; set; }
}