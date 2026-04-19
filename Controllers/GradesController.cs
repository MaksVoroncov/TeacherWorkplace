using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;
using TeacherWorkplace.Filters;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Controllers
{
    [AuthorizeFilter]
    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GradesController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(int? studentId, int? subjectId)
        {
            var grades = _context.Grades.Include(g => g.Student).Include(g => g.Subject).AsQueryable();
            
            if (studentId.HasValue) grades = grades.Where(g => g.StudentId == studentId.Value);
            if (subjectId.HasValue) grades = grades.Where(g => g.SubjectId == subjectId.Value);

            ViewBag.Students = await _context.Students.ToListAsync();
            ViewBag.Subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            
            return View(await grades.OrderByDescending(g => g.Date).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "Id", "FullName");
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s => s.IsActive), "Id", "Name");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade grade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", grade.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", grade.SubjectId);
            return View(grade);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return NotFound();
            
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", grade.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", grade.SubjectId);
            return View(grade);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Grade grade)
        {
            if (id != grade.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", grade.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", grade.SubjectId);
            return View(grade);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var grade = await _context.Grades
                .Include(g => g.Student).Include(g => g.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            return grade == null ? NotFound() : View(grade);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade != null) _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
