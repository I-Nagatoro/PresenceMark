using data.RemoteData.RemoteDatabase.DAO;
using domain.Models;
using domain.UseCase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PresenceAPI.Controllers;

[ApiController]
[Route("api/admin/groups")]
public class GroupsController : ControllerBase
{
    private readonly GroupUseCase _groupUseCase;
    private readonly APIUseCase _apiUseCase;
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(GroupUseCase groupUseCase, APIUseCase apiUseCase, ILogger<GroupsController> logger)
    {
        _groupUseCase = groupUseCase;
        _apiUseCase = apiUseCase;
        _logger = logger;
    }

    [HttpPost("create")]
    public IActionResult CreateGroup(GroupAPI groupDto)
    {
        try
        {
            _logger.LogInformation("Попытка создания новой группы");
            
            if (groupDto == null)
            {
                _logger.LogWarning("CreateGroup: Данные группы не предоставлены");
                return BadRequest(new { message = "Данные группы не могут быть пустыми" });
            }

            if (string.IsNullOrWhiteSpace(groupDto.Name))
            {
                _logger.LogWarning("CreateGroup: Не указано название группы");
                return BadRequest(new { message = "Название группы не может быть пустым" });
            }

            _apiUseCase.AddGroup(groupDto);
            _logger.LogInformation($"Группа {groupDto.Name} успешно создана");
            return Ok(new { message = "Группа успешно создана" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании группы");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("remove/{groupId}")]
    public IActionResult RemoveGroup(int groupId)
    {
        try
        {
            _logger.LogInformation($"Попытка удаления группы с ID: {groupId}");
            
            if (groupId <= 0)
            {
                _logger.LogWarning("RemoveGroup: Неверный ID группы");
                return BadRequest(new { message = "ID группы должен быть положительным числом" });
            }

            _groupUseCase.DeleteGroupById(groupId);
            _logger.LogInformation($"Группа с ID {groupId} успешно удалена");
            return Ok(new { message = "Группа успешно удалена" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при удалении группы с ID: {groupId}");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("add/students/{groupId}")]
    public IActionResult AddStudents(int groupId, [FromBody] StudentsDTO studentsDto)
    {
        try
        {
            _logger.LogInformation($"Попытка добавления студентов в группу с ID: {groupId}");
            
            if (groupId <= 0)
            {
                _logger.LogWarning("AddStudents: Неверный ID группы");
                return BadRequest(new { message = "ID группы должен быть положительным числом" });
            }

            if (studentsDto == null || studentsDto.Students == null || !studentsDto.Students.Any())
            {
                _logger.LogWarning("AddStudents: Не предоставлены данные студентов");
                return BadRequest(new { message = "Список студентов не может быть пустым" });
            }

            _apiUseCase.AddStudentsToExistingGroup(groupId, studentsDto.Students);
            _logger.LogInformation($"В группу {groupId} успешно добавлено {studentsDto.Students.Count} студентов");
            return Ok(new { message = "Студенты добавлены в группу" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при добавлении студентов в группу с ID: {groupId}");
            return BadRequest(new { message = ex.Message });
        }
    }
}

[ApiController]
[Route("api/admin/presence")]
public class PresenceController : ControllerBase
{
    private readonly PresenceUseCase _presenceUseCase;
    private readonly ILogger<PresenceController> _logger;

    public PresenceController(PresenceUseCase presenceUseCase, ILogger<PresenceController> logger)
    {
        _presenceUseCase = presenceUseCase;
        _logger = logger;
    }

    [HttpGet("get/{groupId}")]
    public IActionResult GetGroupPresence(int groupId, [FromQuery] DateOnly? date, [FromQuery] int? student)
    {
        try
        {
            _logger.LogInformation($"Запрос посещаемости для группы {groupId}, дата: {date}, студент: {student}");
            
            if (groupId <= 0)
            {
                _logger.LogWarning("GetGroupPresence: Неверный ID группы");
                return BadRequest(new { message = "ID группы должен быть положительным числом" });
            }

            var result = _presenceUseCase.GetPresenceByUserId(groupId, date, student);
            _logger.LogInformation($"Успешно получены данные посещаемости для группы {groupId}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при получении посещаемости для группы {groupId}");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("clear/{groupId}")]
    public IActionResult ClearPresence([FromQuery] int? group)
    {
        try
        {
            _logger.LogInformation($"Попытка очистки посещаемости для группы {group}");
            
            if (!group.HasValue || group <= 0)
            {
                _logger.LogWarning("ClearPresence: Неверный ID группы");
                return BadRequest(new { message = "ID группы должен быть положительным числом" });
            }

            _presenceUseCase.DeletePresenceByGroup(group.Value);
            _logger.LogInformation($"Посещаемость для группы {group} успешно очищена");
            return Ok(new { message = "Посещаемость группы очищена" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при очистке посещаемости для группы {group}");
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpDelete("clear/all")]
    public IActionResult ClearAllPresence()
    {
        try
        {
            _logger.LogInformation("Попытка очистки всей посещаемости");
            
            _presenceUseCase.ClearAllPresence();
            _logger.LogInformation("Вся посещаемость успешно очищена");
            return Ok(new { message = "Все записи посещаемости успешно удалены." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при очистке всей посещаемости");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("newRecord")]
    public IActionResult RecordPresence([FromBody] List<domain.Models.PresenceAPI> presenceList)
    {
        try
        {
            _logger.LogInformation("Попытка записи новой посещаемости");
            
            if (presenceList == null || !presenceList.Any())
            {
                _logger.LogWarning("RecordPresence: Не предоставлены данные посещаемости");
                return BadRequest(new { message = "Список посещаемости не может быть пустым" });
            }

            _presenceUseCase.AddPresence(presenceList);
            _logger.LogInformation($"Успешно записана посещаемость для {presenceList.Count} записей");
            return Ok(new { message = "Посещаемость записана" });
        }
        catch (Exception ex)
        { 
            _logger.LogError(ex, "Ошибка при записи посещаемости");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    public IActionResult ModifyPresence([FromBody] List<UpdateAttAPI> attendanceList)
    {
        try
        {
            _logger.LogInformation("Попытка обновления посещаемости");
            
            if (attendanceList == null || !attendanceList.Any())
            {
                _logger.LogWarning("ModifyPresence: Не предоставлены данные для обновления");
                return BadRequest(new { message = "Список обновлений не может быть пустым" });
            }

            _presenceUseCase.UpdatePresence(attendanceList);
            _logger.LogInformation($"Успешно обновлена посещаемость для {attendanceList.Count} записей");
            return Ok(new { message = "Посещаемость обновлена" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении посещаемости");
            return BadRequest(new { message = ex.Message });
        }
    }
}