using ClosedXML.Excel;
using data.RemoteData.RemoteDatabase.DAO;
using data.Repository;
using domain.Models;

namespace domain.UseCase;

public class PresenceUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPresenceRepository _presenceRepository;

    public PresenceUseCase(IGroupRepository groupRepository, IUserRepository userRepository,
        IPresenceRepository presenceRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _presenceRepository = presenceRepository;
    }
    public void GeneratePresenceOnDay(int groupId, int firstLessonNum, int lastLessonNum)
    {
        var users = _userRepository.GetAllStudentsByGroupId(groupId);
        DateOnly startDate = _presenceRepository.GetLastDateByGroupId(groupId)?.AddDays(1)??DateOnly.FromDateTime(DateTime.Today);
        List<PresenceDAO> presences = new List<PresenceDAO>();
        for (int lessonNumber = firstLessonNum; lessonNumber <= lastLessonNum; lessonNumber++)
        {
            foreach (var user in users)
            {
                presences.Add(new PresenceDAO
                {
                    UserId = user.UserId,
                    GroupId = groupId,
                    Date = startDate,
                    LessonNumber = lessonNumber,
                    IsAttendance = true
                });
            }
        }
        _presenceRepository.SavePresence(presences);
    }

    public void GeneratePresenceOnWeek(int groupId, int firstLessonNum, int lastLessonNum)
    {
        for (int i = 1; i <= lastLessonNum; i++)
        {
            GeneratePresenceOnDay(groupId, firstLessonNum, lastLessonNum);
        }
    }

    public List<PresenceDAO> GetPresenceByDateNGroup(DateOnly startDate, DateOnly endDate, int groupId)
    {
        return _presenceRepository.GetPresenceByDateNGroup(startDate, endDate, groupId);
    }

    public bool MarkStudentAbsentForLessons(int userId, int groupId, int firstLessonNum, int lastLessonNum, DateOnly date)
    {
        List<PresenceDAO> presences = _presenceRepository.GetPresenceForAbsent(date, groupId);
        if (presences.Where(p => p.UserId == userId).Count() > 0)
        {
            foreach (var presence in presences.Where(p => p.UserId == userId && p.LessonNumber >= firstLessonNum && p.LessonNumber <= lastLessonNum))
            {
                presence.IsAttendance = false;
            }
            _presenceRepository.UpdateAtt(userId, groupId, firstLessonNum, lastLessonNum, date, false);
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<PresenceDAO> GetAllPresenceByGroup(int groupId)
    {
        return _presenceRepository.GetPresenceByGroup(groupId);
    }

    public GroupAttendanceStats GetGeneralPresence(int groupId)
    {
        return _presenceRepository.GetGeneralPresenceForGroup(groupId);
    }

    public void ExportAttendanceXML()
    {
        var attendanceByGroup = GetAllAttendanceByGroups();
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string reportsFolderPath = Path.Combine(projectDirectory, "Reports");
            string filePath = Path.Combine(reportsFolderPath, "AttendanceReport.xlsx");
            if (!Directory.Exists(reportsFolderPath))
            {
                Directory.CreateDirectory(reportsFolderPath);
            }
            using (var workbook = new XLWorkbook())
            {
                foreach (var group in attendanceByGroup)
                {
                    var worksheet = workbook.Worksheets.Add($"{group.Key}");
                    worksheet.Cell(1, 1).Value = "ФИО";
                    worksheet.Cell(1, 2).Value = "Группа";
                    worksheet.Cell(1, 3).Value = "Дата";
                    worksheet.Cell(1, 4).Value = "Занятие";
                    worksheet.Cell(1, 5).Value = "Статус";

                    int row = 2;
                    int lesNum = 1;
                    foreach (var record in group.Value.OrderBy(r => r.Date).ThenBy(r => r.LessonNumber).ThenBy(r => r.UserId))
                    {
                        if (lesNum != record.LessonNumber)
                        {
                            row++;
                        }
                        worksheet.Cell(row, 1).Value = record.UserName;
                        worksheet.Cell(row, 2).Value = record.GroupName;
                        worksheet.Cell(row, 3).Value = record.Date.ToString("dd.MM.yyyy");
                        worksheet.Cell(row, 4).Value = record.LessonNumber;
                        worksheet.Cell(row, 5).Value = record.IsAttedance ? "Присутствует" : "Отсутствует";
                        row++;



                        lesNum = record.LessonNumber;
                    }

                    worksheet.Columns().AdjustToContents();
                }

                workbook.SaveAs(filePath);
            }
    }
    
    public Dictionary<string, List<AttRec>> GetAllAttendanceByGroups()
    {
        var attendanceByGroup = new Dictionary<string, List<AttRec>>();
        var allGroups = _groupRepository.GetAllGroups();

        foreach (var group in allGroups)
        {
            var groupAttendance = _presenceRepository.GetPresenceByGroup(group.Id);
            var attendanceRecords = new List<AttRec>();

            foreach (var record in groupAttendance)
            {
                var names = _userRepository.GetAllStudents().Where(u => u.UserId == record.UserId);
                foreach (var name in names)
                {
                    attendanceRecords.Add(new AttRec
                    {
                        UserName = name.FIO,
                        UserId = name.UserId,
                        Date = record.Date,
                        IsAttedance = record.IsAttendance,
                        LessonNumber = record.LessonNumber,
                        GroupName = group.Name
                    });
                }
            }

            attendanceByGroup.Add(group.Name, attendanceRecords);
        }

        return attendanceByGroup;
    }

    public List<PresenceDAO> GetPresenceByUserId(int groupId, DateOnly? date, int? student)
    {
        return _presenceRepository.GetPresenceByUserId(groupId, date, student);
    }

    public void DeletePresenceByGroup(int group)
    {
        _presenceRepository.DeletePresenceByGroup(group);
    }

    public void ClearAllPresence()
    {
        _presenceRepository.ClearAllPresence();
    }

    public void AddPresence(List<PresenceAPI> presenceList)
    {
        var convertPresence = presenceList.Select(p=> new PresenceDAO
        {
            Date = p.Date,
            LessonNumber = p.LessonNumber,
            UserId = p.Student,
            IsAttendance = p.Status,
            GroupId = _userRepository.FindStudentById(p.Student)?.GroupId ?? throw new Exception("Студент не найден"),
        }).ToList();
        _presenceRepository.AddPresence(convertPresence);
    }

    public void UpdatePresence(List<UpdateAttAPI> attendanceList)
    {
        var convertPresence = attendanceList.Select(p=>new PresenceDAO
        {
            Date = p.Date,
            LessonNumber = p.LessonNumber,
            UserId = p.Student,
            IsAttendance = p.Status
        }).ToList();
        _presenceRepository.UpdatePresence(convertPresence);
    }
}