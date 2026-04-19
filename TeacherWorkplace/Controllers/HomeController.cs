using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;
using TeacherWorkplace.Filters;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Controllers
{
    [AuthorizeFilter]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Статистика для Dashboard
            var totalStudents = await _context.Students.CountAsync();
            var totalGroups = await _context.Groups.CountAsync();
            var totalSubjects = await _context.Subjects.Where(s => s.IsActive).CountAsync();
            
            // Непроверенные задания
            var pendingAssignments = await _context.StudentAssignments
                .Where(sa => sa.Status == AssignmentStatus.Submitted)
                .CountAsync();

            // Средний балл по всем студентам
            var averageGrade = await _context.Grades
                .AverageAsync(g => (double?)g.Score) ?? 0;

            // Посещаемость за сегодня
            var todayAttendance = await _context.Attendances
                .Where(a => a.Date == DateTime.Today)
                .CountAsync();

            // Последние добавленные студенты
            var recentStudents = await _context.Students
                .Include(s => s.Group)
                .OrderByDescending(s => s.EnrollmentDate)
                .Take(5)
                .ToListAsync();

            // Ближайшие дедлайны заданий
            var upcomingDeadlines = await _context.Assignments
                .Where(a => a.IsActive && a.Deadline >= DateTime.Now)
                .OrderBy(a => a.Deadline)
                .Take(5)
                .Include(a => a.Subject)
                .ToListAsync();

            // Статистика посещаемости за последние 7 дней
            var weekAgo = DateTime.Today.AddDays(-7);
            var attendanceByDay = await _context.Attendances
                .Where(a => a.Date >= weekAgo)
                .GroupBy(a => a.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Present = g.Count(a => a.Status == AttendanceStatus.Present),
                    Absent = g.Count(a => a.Status == AttendanceStatus.Absent)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Топ-5 студентов по среднему баллу
            var topStudents = await _context.Students
                .Include(s => s.Grades)
                .Include(s => s.Group)
                .Where(s => s.Grades.Any())
                .Select(s => new
                {
                    Student = s,
                    AverageGrade = s.Grades.Average(g => g.Score)
                })
                .OrderByDescending(x => x.AverageGrade)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalGroups = totalGroups;
            ViewBag.TotalSubjects = totalSubjects;
            ViewBag.PendingAssignments = pendingAssignments;
            ViewBag.AverageGrade = Math.Round(averageGrade, 2);
            ViewBag.TodayAttendance = todayAttendance;
            ViewBag.RecentStudents = recentStudents;
            ViewBag.UpcomingDeadlines = upcomingDeadlines;
            ViewBag.AttendanceByDay = attendanceByDay;
            ViewBag.TopStudents = topStudents;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
