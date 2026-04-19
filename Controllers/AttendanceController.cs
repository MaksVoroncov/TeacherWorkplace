using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;
using TeacherWorkplace.Filters;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Controllers
{
    [AuthorizeFilter]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AttendanceController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(DateTime? date, int? subjectId)
        {
            var selectedDate = date ?? DateTime.Today;
            var attendances = _context.Attendances
                .Include(a => a.Student).ThenInclude(s => s.Group)
                .Include(a => a.Subject)
                .Where(a => a.Date == selectedDate).AsQueryable();
            
            if (subjectId.HasValue) attendances = attendances.Where(a => a.SubjectId == subjectId.Value);

            ViewBag.SelectedDate = selectedDate;
            ViewBag.Subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            
            return View(await attendances.ToListAsync());
        }

        public async Task<IActionResult> TakeAttendance(int? subjectId, int? groupId)
        {
            var subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            var groups = await _context.Groups.ToListAsync();
            
            ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
            ViewBag.Groups = new SelectList(groups, "Id", "FullName");
            
            if (subjectId.HasValue && groupId.HasValue)
            {
                var students = await _context.Students
                    .Where(s => s.GroupId == groupId.Value)
                    .OrderBy(s => s.LastName)
                    .ToListAsync();
                    
                var existingAttendance = await _context.Attendances
                    .Where(a => a.Date == DateTime.Today && a.SubjectId == subjectId.Value)
                    .ToListAsync();

                ViewBag.Students = students;
                ViewBag.ExistingAttendance = existingAttendance;
                ViewBag.SelectedSubjectId = subjectId.Value;
                ViewBag.SelectedGroupId = groupId.Value;
            }
            
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAttendance(int subjectId, int groupId, List<int> presentStudents, List<int> lateStudents, List<int> excusedStudents)
        {
            var students = await _context.Students.Where(s => s.GroupId == groupId).ToListAsync();
            
            // Удаляем старые записи за сегодня
            var existing = await _context.Attendances
                .Where(a => a.Date == DateTime.Today && a.SubjectId == subjectId && students.Select(s => s.Id).Contains(a.StudentId))
                .ToListAsync();
            _context.Attendances.RemoveRange(existing);

            foreach (var student in students)
            {
                var status = AttendanceStatus.Absent;
                if (presentStudents?.Contains(student.Id) == true) status = AttendanceStatus.Present;
                else if (lateStudents?.Contains(student.Id) == true) status = AttendanceStatus.Late;
                else if (excusedStudents?.Contains(student.Id) == true) status = AttendanceStatus.Excused;

                _context.Attendances.Add(new Attendance
                {
                    StudentId = student.Id,
                    SubjectId = subjectId,
                    Date = DateTime.Today,
                    Status = status
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
