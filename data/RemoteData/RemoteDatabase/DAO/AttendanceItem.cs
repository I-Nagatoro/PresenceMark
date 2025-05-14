using Avalonia.Media;

namespace data.RemoteData.RemoteDatabase.DAO;

public class AttendanceItem
{
    public int LessonId { get; set; }
    public string FIO { get; set; }
    public string Status { get; set; }
    public IBrush StatusColor { get; set; }
    public bool IsPresent { get; set; }
    public DateOnly Date { get; set; }
}
