using System.Text;
using data.RemoteData.RemoteDatabase.DAO;
using domain.UseCase;

namespace UI;

public class UserConsole
{
    private readonly UserUseCase _userUseCase;
    private readonly GroupUseCase _groupUseCase;

    public UserConsole(UserUseCase userUseCase, GroupUseCase groupUseCase)
    {
        _userUseCase = userUseCase;
        _groupUseCase = groupUseCase;
    }
    
    public void DisplayAllStudents()
    {
        Console.WriteLine("\n Список студентов:");
        StringBuilder usersOut = new StringBuilder();

        var users = _userUseCase.GetAllStudents();
        if (users == null || !users.Any())
        {
            Console.WriteLine("Нет студентов для отображения");
            return;
        }

        var groups = _groupUseCase.GetAllGroups();
        foreach (var user in users)
        {
            var group = groups.FirstOrDefault(g => g.Id == user.GroupId);
            string groupName = group != null ? group.Name : "Группа не найдена";
            usersOut.AppendLine($"{user.UserId}\t{user.FIO}\t{groupName}");
        }
        Console.WriteLine(usersOut);
        Console.WriteLine("---------------------------------------------------------------\n");
    }

    public void RemoveStudentById(int id)
    {
        string response = _userUseCase.RemoveStudentById(id) ? "Студент удалён" : "Студент не найден";
        Console.WriteLine($"\n{response}\n");
    }

    public void UpdateStudentById(int id)
    {
        var user = _userUseCase.FindStudentById(id);
        Console.WriteLine($"Текущие данные: {user.FIO}");
        Console.Write("\nВведите новое ФИО: ");
        string newFIO = Console.ReadLine();
        Console.Write("\nВведите новый ID группы: ");
        int GroupId = int.Parse(Console.ReadLine());
        _userUseCase.UpdateStudent(id, newFIO, GroupId);
        Console.WriteLine("\nСтудент обновлен.\n");
    }

    public void FindStudentById(int id)
    {
        UserDAO user = _userUseCase.FindStudentById(id);
        if (user != null)
        {
            Console.WriteLine($"\nСтудент найден: ID: {user.UserId}, ФИО: {user.FIO}, Группа: {user.Group.Name}\n");
        }
        else
        {
            Console.WriteLine("\nСтудент не найден.\n");
        }
    }
}