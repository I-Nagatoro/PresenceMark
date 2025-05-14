using System.Text;
using domain.UseCase;

namespace UI;

public class GroupConsole
{
    private readonly GroupUseCase _groupUseCase;

    public GroupConsole(GroupUseCase groupUseCase)
    {
        _groupUseCase = groupUseCase;
    }
    public void DisplayAllGroups()
    {
        Console.WriteLine("Список групп:");
        StringBuilder groupOut = new StringBuilder();
        var groups = _groupUseCase.GetAllGroups();
        foreach (var group in groups)
        {
            groupOut.AppendLine($"{group.Id}\t{group.Name}");
        }
        Console.WriteLine(groupOut);
        Console.WriteLine("---------------------------------------------------------------\n");
    }

    public void AddGroup(string groupName)
    {
        _groupUseCase.AddGroup(groupName);
        Console.WriteLine($"\nГруппа {groupName} добавлена.\n");
    }

    public void UpdateGroup(int id, string groupName)
    {
        _groupUseCase.UpdateGroup(id, groupName);
        Console.WriteLine($"\nНазвание группы с ID {id} изменено на {groupName}.\n");
    }

    public void FindGroupById(int id)
    {
        var group = _groupUseCase.FindGroupById(id);
    }
}