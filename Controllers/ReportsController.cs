using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;
using TeacherWorkplace.Filters;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Controllers
{
    [AuthorizeFilter]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReportsController(ApplicationDbContext context) => _context = context;

        public IActionResult Index() => View();

        public async Task<IActionResult> Performance(int? groupId, int? subjectId)
        {
            var groups   = await _context.Groups.OrderBy(g => g.Name).ToListAsync();
            var subjects = await _context.Subjects.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();

            ViewBag.Groups   = new SelectList(groups,   "Id", "Name",    groupId);
            ViewBag.Subjects = new SelectList(subjects, "Id", "Name",    subjectId);
            ViewBag.SelectedGroupId   = groupId;
            ViewBag.SelectedSubjectId = subjectId;

            if (groupId.HasValue)
            {
                var query = _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Grades).ThenInclude(g => g.Subject)
                    .Where(s => s.GroupId == groupId.Value);

                if (subjectId.HasValue)
                    query = query.Where(s => s.Grades.Any(g => g.SubjectId == subjectId.Value));

                var students = await query.ToListAsync();

                var studentStats = students.Select(s => new
                {
                    Student      = s,
                    AverageGrade = s.Grades.Any() ? Math.Round(s.Grades.Average(g => g.Score), 2) : 0.0,
                    TotalGrades  = s.Grades.Count,
                    ExcellentCount = s.Grades.Count(g => g.Score == 5),
                    GoodCount      = s.Grades.Count(g => g.Score == 4),
                    SatisfCount    = s.Grades.Count(g => g.Score == 3),
                    BadCount       = s.Grades.Count(g => g.Score <= 2),
                }).OrderByDescending(x => x.AverageGrade).ToList();

                ViewBag.StudentStats = studentStats;
                ViewBag.GroupName    = groups.FirstOrDefault(g => g.Id == groupId)?.Name ?? "";

                // Средний балл по группе для графика
                ViewBag.ChartLabels = studentStats.Select(x => x.Student.LastName + " " + x.Student.FirstName[0] + ".").ToList();
                ViewBag.ChartData   = studentStats.Select(x => x.AverageGrade).ToList();
            }

            return View();
        }

        public async Task<IActionResult> Attendance(DateTime? startDate, DateTime? endDate, int? groupId)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-1);
            var end   = endDate   ?? DateTime.Today;

            ViewBag.StartDate = start;
            ViewBag.EndDate   = end;

            var groups = await _context.Groups.OrderBy(g => g.Name).ToListAsync();
            ViewBag.Groups         = new SelectList(groups, "Id", "Name", groupId);
            ViewBag.SelectedGroupId = groupId;

            var attendances = _context.Attendances
                .Include(a => a.Student).ThenInclude(s => s.Group)
                .Include(a => a.Subject)
                .Where(a => a.Date >= start && a.Date <= end)
                .AsQueryable();

            if (groupId.HasValue)
                attendances = attendances.Where(a => a.Student.GroupId == groupId.Value);

            var raw = await attendances.ToListAsync();

            var attendanceData = raw
                .GroupBy(a => a.Student)
                .Select(g => new
                {
                    Student        = g.Key,
                    Total          = g.Count(),
                    Present        = g.Count(a => a.Status == AttendanceStatus.Present),
                    Absent         = g.Count(a => a.Status == AttendanceStatus.Absent),
                    Late           = g.Count(a => a.Status == AttendanceStatus.Late),
                    Excused        = g.Count(a => a.Status == AttendanceStatus.Excused),
                    AttendanceRate = g.Count() > 0
                        ? Math.Round((double)g.Count(a => a.Status == AttendanceStatus.Present) / g.Count() * 100, 1)
                        : 0.0
                })
                .OrderByDescending(x => x.AttendanceRate)
                .ToList();

            ViewBag.AttendanceData = attendanceData;

            // Статистика для плашек
            ViewBag.TotalRecords    = raw.Count;
            ViewBag.TotalPresent    = raw.Count(a => a.Status == AttendanceStatus.Present);
            ViewBag.TotalAbsent     = raw.Count(a => a.Status == AttendanceStatus.Absent);
            ViewBag.TotalLate       = raw.Count(a => a.Status == AttendanceStatus.Late);
            ViewBag.TotalExcused    = raw.Count(a => a.Status == AttendanceStatus.Excused);
            ViewBag.AvgAttendance   = raw.Count > 0
                ? Math.Round((double)raw.Count(a => a.Status == AttendanceStatus.Present) / raw.Count * 100, 1)
                : 0.0;

            return View();
        }
    }
}
