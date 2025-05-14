using data.RemoteData.RemoteDatabase.DAO;
using domain.Models;
using domain.UseCase;
using Microsoft.AspNetCore.Mvc;

namespace PresenceAPI.Controllers;

[ApiController]
[Route("api/admin/groups")]
public class GroupsController : ControllerBase
{
    private readonly GroupUseCase _groupUseCase;
    private readonly APIUseCase _apiUseCase;

    public GroupsController(GroupUseCase groupUseCase, APIUseCase apiUseCase)
    {
        _groupUseCase = groupUseCase;
        _apiUseCase = apiUseCase;
    }

    [HttpPost("create")]
    public IActionResult CreateGroup(GroupAPI groupDto)
    {
        try
        {
            _apiUseCase.AddGroup(groupDto);
            return Ok(new { message = "Группа успешно создана" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("remove/{groupId}")]
    public IActionResult RemoveGroup(int groupId)
    {
        try
        {
            _groupUseCase.DeleteGroupById(groupId);
            return Ok(new { message = "Группа успешно удалена" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("add/students/{groupId}")]
    public IActionResult AddStudents(int groupId, [FromBody] StudentsDTO studentsDto)
    {
        try
        {
            _apiUseCase.AddStudentsToExistingGroup(groupId, studentsDto.Students);
            return Ok(new { message = "Студенты добавлены в группу" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

[ApiController]
[Route("api/admin/presence")]
public class PresenceController : ControllerBase
{
    private readonly PresenceUseCase _presenceUseCase;

    public PresenceController(PresenceUseCase presenceUseCase)
    {
        _presenceUseCase = presenceUseCase;
    }

    [HttpGet("get/{groupId}")]
    public IActionResult GetGroupPresence(int groupId, [FromQuery] DateOnly? date, [FromQuery] int? student)
    {
        try
        {
            var result = _presenceUseCase.GetPresenceByUserId(groupId, date, student);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("clear/{groupId}")]
    public IActionResult ClearPresence([FromQuery] int? group)
    {
        try
        {
                _presenceUseCase.DeletePresenceByGroup(group.Value);
                return Ok(new { message = "Посещаемость группы очищена" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpDelete("clear/all")]
    public IActionResult ClearAllPresence()
    {
        try
        {
            _presenceUseCase.ClearAllPresence();
            return Ok(new { message = "Все записи посещаемости успешно удалены." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("newRecord")]
    public IActionResult RecordPresence([FromBody] List<domain.Models.PresenceAPI> presenceList)
    {
        try
        {
            _presenceUseCase.AddPresence(presenceList);
            return Ok(new { message = "Посещаемость записана" });
        }
        catch (Exception ex)
        { 
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    public IActionResult ModifyPresence([FromBody] List<UpdateAttAPI> attendanceList)
    {
        try
        {
            _presenceUseCase.UpdatePresence(attendanceList);
            return Ok(new { message = "Посещаемость обновлена" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}