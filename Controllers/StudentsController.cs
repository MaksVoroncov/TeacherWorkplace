using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;
using TeacherWorkplace.Filters;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Controllers
{
    [AuthorizeFilter]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string search, int? groupId)
        {
            var students = _context.Students.Include(s => s.Group).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                students = students.Where(s => 
                    s.LastName.Contains(search) || 
                    s.FirstName.Contains(search) ||
                    s.Phone.Contains(search));
            }

            if (groupId.HasValue)
            {
                students = students.Where(s => s.GroupId == groupId.Value);
            }

            ViewBag.Groups = await _context.Groups.ToListAsync();
            return View(await students.OrderBy(s => s.LastName).ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Group)
                .Include(s => s.Grades).ThenInclude(g => g.Subject)
                .Include(s => s.Attendances).ThenInclude(a => a.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            // Статистика студента
            ViewBag.AverageGrade = student.Grades.Any() 
                ? Math.Round(student.Grades.Average(g => g.Score), 2) 
                : 0;
            
            var totalAttendance = student.Attendances.Count;
            var presentCount = student.Attendances.Count(a => a.Status == AttendanceStatus.Present);
            ViewBag.AttendanceRate = totalAttendance > 0 
                ? Math.Round((double)presentCount / totalAttendance * 100, 1) 
                : 0;

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "FullName");
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();

                // Логирование
                var userId = HttpContext.Session.GetInt32("UserId");
                _context.LogEntries.Add(new LogEntry
                {
                    UserId = userId,
                    Action = $"Добавлен студент: {student.FullName}",
                    Details = $"ID: {student.Id}, Группа: {student.GroupId}"
                });
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "FullName", student.GroupId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "FullName", student.GroupId);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();

                    // Логирование
                    var userId = HttpContext.Session.GetInt32("UserId");
                    _context.LogEntries.Add(new LogEntry
                    {
                        UserId = userId,
                        Action = $"Изменен студент: {student.FullName}",
                        Details = $"ID: {student.Id}"
                    });
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "FullName", student.GroupId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                // Логирование
                var userId = HttpContext.Session.GetInt32("UserId");
                _context.LogEntries.Add(new LogEntry
                {
                    UserId = userId,
                    Action = $"Удален студент: {student.FullName}",
                    Details = $"ID: {student.Id}"
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
