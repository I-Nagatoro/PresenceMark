using System.Globalization;
using DocumentFormat.OpenXml.Drawing;
using domain.UseCase;

namespace UI;

public class MainConsole
{
    private readonly GroupConsole _groupConsole;
    private readonly UserConsole _userConsole;
    private readonly PresenceConsole _presenceConsole;

    public MainConsole(UserUseCase userUseCase, GroupUseCase groupUseCase, PresenceUseCase presenceUseCase)
    {
        _userConsole = new UserConsole(userUseCase, groupUseCase);
        _groupConsole = new GroupConsole(groupUseCase);
        _presenceConsole = new PresenceConsole(presenceUseCase);
    }

    public void Run()
    {
        while (true)
        {
            ShowStartMenu();
            string input = Console.ReadLine();
            Console.WriteLine();

            switch (input)
            {
                case "1":
                    _userConsole.DisplayAllStudents();
                    break;
                case "2":
                    HandleDeleteStudent();
                    break;
                case "3":
                    HandleStudentUpdate();
                    break;
                case "4":
                    HandleFindStudentById();
                    break;
                case "5":
                    _groupConsole.DisplayAllGroups();
                    break;
                case "6":
                    HandleCreateGroup();
                    break;
                case "7":
                    HandleUpdateGroup();
                    break;
                case "8":
                    HandleFindGroupById();
                    break;
                case "9":
                    HandleCreateDailyAtt();
                    break;
                case "10":
                    HandleCreateWeeklyAtt();
                    break;
                case "11":
                    HandleViewAtt();
                    break;
                case "12":
                    HandleRecAbs();
                    break;
                case "13":
                    HandleShowAttForGroup();
                    break;
                case "14":
                    HandleStatsAttForGroup();
                    break;
                case "15":
                    HandleGenerateXML();
                    break;
            }
        }
    }

    private void HandleGenerateXML()
    {
        _presenceConsole.ExportAttendanceXML();
        Console.WriteLine("Отчёт успешно экспортирован.");
    }

    private void HandleStatsAttForGroup()
    {
        int groupId = ValidationIntInput("Введите id группы: ");
        _presenceConsole.ShowGeneralPresence(groupId);
    }

    private void HandleShowAttForGroup()
    {
        int groupId = ValidationIntInput("Введите id группы: ");
        _presenceConsole.ShowAllPresenceByGroup(groupId);
    }

    private void HandleRecAbs()
    {
        DateTime date = ValidationDateInput("Введите дату отсутствия (дд.мм.гггг): ");
        int groupId = ValidationIntInput("Введите id группы: ");
        int userId = ValidationIntInput("Введите id студента: ");
        int firstLessonNum = ValidationIntInput("Введите номер начала занятий: ");
        int lastLessonNum = ValidationIntInput("Введите номер окончания занятий: ");

        _presenceConsole.MarkStudentAbsent(date, groupId, userId, firstLessonNum, lastLessonNum);
    }

    private void HandleViewAtt()
    {
        DateTime date = ValidationDateInput("Введите дату (дд.мм.гггг): ");
        int groupId = ValidationIntInput("Введите id группы: ");
        _presenceConsole.DisplayPresence(date, groupId);
    }

    private void HandleCreateWeeklyAtt()
    {
        int groupId = ValidationIntInput("Введите id группы: ");
        int firstLessonNum = ValidationIntInput("Введите номер начала занятий: ");
        int lastLessonNum = ValidationIntInput("Введите номер окончания занятий: ");
            
        _presenceConsole.GeneratePresenceOnWeek(DateTime.Today, groupId, firstLessonNum, lastLessonNum);
        Console.WriteLine("Записи за неделю созданы успешно.");
    }

    private int ValidationIntInput(string prompt)
    {
        int result;
        Console.Write(prompt);
        while (!int.TryParse(Console.ReadLine(), out result))
        {
            Console.WriteLine("Ошибка формата ввода. Повторите попытку.");
            Console.Write(prompt);
        }
        return result;
    }
    
    private DateTime ValidationDateInput(string prompt)
    {
        DateTime date;
        Console.Write(prompt);
        while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            Console.WriteLine("Некорректный формат даты. Используйте дд.мм.гггг");
            Console.Write(prompt);
        }
        return date;
    }

    private void HandleCreateDailyAtt()
    {
        int groupId = ValidationIntInput("Введите id группы: ");
        int firstLessonNum = ValidationIntInput("Введите номер начала занятий: ");
        int lastLessonNum = ValidationIntInput("Введите номер окончания занятий: ");
        _presenceConsole.GeneratePresenceOnDay(DateTime.Today, groupId, firstLessonNum, lastLessonNum);
    }

    private void HandleFindGroupById()
    {
        Console.WriteLine("Введите id группы для обновления:");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _groupConsole.FindGroupById(id);
        }
    }

    private void HandleUpdateGroup()
    {
        Console.WriteLine("Введите id группы для обновления:");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Console.Write("Введите новое название для группы: ");
            _groupConsole.UpdateGroup(id, Console.ReadLine());
        }
    }

    private void HandleCreateGroup()
    {
        Console.Write("Введите название для новой группы: ");
        _groupConsole.AddGroup(Console.ReadLine());
    }

    private void HandleFindStudentById()
    {
        Console.WriteLine("Введите id студента для поиска:");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _userConsole.FindStudentById(id);
        }
    }

    private void HandleStudentUpdate()
    {
        Console.WriteLine("Введите id студента для обновления:");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _userConsole.UpdateStudentById(id);
        }
    }

    private void HandleDeleteStudent()
    {
        Console.WriteLine("Введите id студента для удаления:");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _userConsole.RemoveStudentById(id);
        }
    }

    private void ShowStartMenu()
    {
        Console.WriteLine("\n -= Добро пожаловать! =-");
        
        Console.WriteLine("\n Управление студентами:");
        Console.WriteLine("1. Список всех студентов");
        Console.WriteLine("2. Удалить студента");
        Console.WriteLine("3. Обновить данные студента");
        Console.WriteLine("4. Найти студента\n");
        
        Console.WriteLine("Управление группами:");
        Console.WriteLine("5. Показать все группы");
        Console.WriteLine("6. Создать новую группу");
        Console.WriteLine("7. Переименовать группу");
        Console.WriteLine("8. Найти группу\n");
        
        Console.WriteLine("Управление посещаемостью:");
        Console.WriteLine("9. Создать записи за день");
        Console.WriteLine("10. Создать записи за неделю");
        Console.WriteLine("11. Просмотр посещаемости");
        Console.WriteLine("12. Зарегистрировать отсутствие студента");
        Console.WriteLine("13. История посещаемости группы");
        Console.WriteLine("14. Статистика посещаемости");
        Console.WriteLine("15. Экспорт в Excel\n");
        
        Console.Write("Выберите действие: ");
    }
}