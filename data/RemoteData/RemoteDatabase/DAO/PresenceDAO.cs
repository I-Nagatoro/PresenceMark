using System;
using System.Collections.Generic;

namespace data.RemoteData.RemoteDatabase.DAO;

public class PresenceDAO
{
    public int PresenceId { get; set; }
    public int UserId { get; set; }
    public DateOnly Date { get; set; }
    public int LessonNumber { get; set; }
    public bool IsAttendance { get; set; }
    public int GroupId { get; set; }

    public string DateString => Date.ToString("dd.MM.yyyy");
    public string Status => IsAttendance ? "Присутствовал" : "Отсутствовал";
}