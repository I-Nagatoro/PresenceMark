using data.RemoteData.RemoteDatabase.DAO;
using domain.UseCase;

namespace UI;

public class PresenceConsole
{
    private readonly PresenceUseCase _presenceUseCase;

    public PresenceConsole(PresenceUseCase presenceUseCase)
    {
        _presenceUseCase = presenceUseCase;
    }
    public void GeneratePresenceOnDay(DateTime date, int groupId, int firstLessonNum, int lastLessonNum)
    {
        _presenceUseCase.GeneratePresenceOnDay(groupId, firstLessonNum, lastLessonNum);
        Console.WriteLine("Посещаемость на день успешно сгенерирована.");
    }

    public void GeneratePresenceOnWeek(DateTime date, int groupId, int firstLessonNum, int lastLessonNum)
    {
        _presenceUseCase.GeneratePresenceOnWeek(groupId, firstLessonNum, lastLessonNum);
        Console.WriteLine("Посещаемость на неделю успешно сгенерирована.");
    }

    public void DisplayPresence(DateTime date, int groupId)
    {
        List<PresenceDAO> presences = _presenceUseCase.GetPresenceByDateNGroup(DateOnly.FromDateTime(date), DateOnly.FromDateTime(date), groupId);
        if (presences == null || !presences.Any())
        {
            Console.WriteLine("Посещаемость на выбранную дату отсутствует.");
            return;
        }
        var sortedPresences = presences.OrderBy(p=>p.LessonNumber).ThenBy(p=>p.UserId).ToList();
        Console.WriteLine($"\nПосещаемость на {date.ToShortDateString()} для группы с id {groupId}:");
        Console.WriteLine("---------------------------------------------------------------------------");
        int previousLesNum = -1;
        foreach (var presence in sortedPresences)
        {
            if (previousLesNum != presence.LessonNumber)
            {
                Console.WriteLine("---------------------------------------------");
                previousLesNum = presence.LessonNumber;
            }
            string status = presence.IsAttendance ? "Присутствует" : "Отсутствует";
            Console.WriteLine($"Пользователь ID: {presence.UserId}, Занятие {presence.LessonNumber}: {status}");
        }
        Console.WriteLine("---------------------------------------------------------------------------");
    }

    public void MarkStudentAbsent(DateTime date, int groupId, int userId, int firstLessonNum, int lastLessonNum)
    {
        bool check = _presenceUseCase.MarkStudentAbsentForLessons(userId, groupId, firstLessonNum, lastLessonNum, DateOnly.FromDateTime(date));
        if (check)
        {
            Console.WriteLine("Студент отмечен как отсутствующий");
        }
        else
        {
            Console.WriteLine($"Посещаемость для студента с ID: {userId} на дату {date.ToShortDateString()}" +
                              $" с {firstLessonNum} по {lastLessonNum} уроки не найдена.");
        }
    }

    public void ShowAllPresenceByGroup(int groupId)
    {
        var presences = _presenceUseCase.GetAllPresenceByGroup(groupId);
        if (presences == null || presences.Count == 0)
        {
            Console.WriteLine($"Посещаемость для группы с ID {groupId} отсутствует.");
            return;
        }
        var groupedPresences = presences.GroupBy(p => p.Date);
        foreach (var group in groupedPresences)
        {
            Console.WriteLine("===================================================");
            Console.WriteLine($"Дата: {group.Key.ToString("dd.MM.yyyy")}");
            Console.WriteLine("===================================================");
            var groupedByLesson = group.GroupBy(p => p.LessonNumber)
                .OrderBy(g => g.Key);
            foreach (var lessonGroup in groupedByLesson)
            {
                Console.WriteLine($"Занятие {lessonGroup.Key}:");
                var userIds = new HashSet<int>();
                foreach (var presence in lessonGroup.OrderBy(p => p.UserId))
                {
                    if (userIds.Add(presence.UserId))
                    {
                        string status = presence.IsAttendance ? "Присутствует" : "Отсутствует";
                        Console.WriteLine($"Студент ID: {presence.UserId}, Статус: {status}");
                    }
                }
                Console.WriteLine("---------------------------------------------------");
            }
        }
    }

    public void ShowGeneralPresence(int groupId)
    {
        var statistics = _presenceUseCase.GetGeneralPresence(groupId);
        Console.WriteLine($"Человек в группе: {statistics.UserCount}, " +
                          $"Количество проведённых занятий: {statistics.TotalLessons}, " +
                          $"Общий процент посещаемости группы: {statistics.AttendancePercentage}%");

        foreach (var user in statistics.UserAttendanceDetails)
        {
            Console.ForegroundColor = user.AttPersent < 40 ? ConsoleColor.Red : ConsoleColor.White;
            Console.WriteLine($"ID Пользователя: {user.UserId}, Посетил: {user.Attended}, " +
                              $"Пропустил: {user.Missed}, Процент посещаемости: {user.AttPersent}%");
        }
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void ExportAttendanceXML()
    {
        _presenceUseCase.ExportAttendanceXML();
        Console.WriteLine("Данные посещаемости успешно экспортированы в Excel.");
    }
}