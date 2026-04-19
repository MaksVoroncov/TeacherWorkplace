using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;
using TeacherWorkplace.Filters;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Controllers
{
    [AuthorizeFilter]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AssignmentsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(int? subjectId, bool? activeOnly)
        {
            var assignments = _context.Assignments.Include(a => a.Subject).AsQueryable();
            
            if (subjectId.HasValue) assignments = assignments.Where(a => a.SubjectId == subjectId.Value);
            if (activeOnly == true) assignments = assignments.Where(a => a.IsActive);

            ViewBag.Subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            
            return View(await assignments.OrderByDescending(a => a.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            
            var assignment = await _context.Assignments
                .Include(a => a.Subject)
                .Include(a => a.StudentAssignments).ThenInclude(sa => sa.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (assignment == null) return NotFound();
            
            return View(assignment);
        }

        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s => s.IsActive), "Id", "Name");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", assignment.SubjectId);
            return View(assignment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();
            
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", assignment.SubjectId);
            return View(assignment);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assignment assignment)
        {
            if (id != assignment.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", assignment.SubjectId);
            return View(assignment);
        }

        public async Task<IActionResult> CheckSubmissions(int? id)
        {
            if (id == null) return NotFound();
            
            var assignment = await _context.Assignments
                .Include(a => a.Subject)
                .Include(a => a.StudentAssignments).ThenInclude(sa => sa.Student).ThenInclude(s => s.Group)
                .FirstOrDefaultAsync(a => a.Id == id);
                
            if (assignment == null) return NotFound();
            
            return View(assignment);
        }

        [HttpPost]
        public async Task<IActionResult> GradeSubmission(int studentAssignmentId, int score, string? comment)
        {
            var studentAssignment = await _context.StudentAssignments.FindAsync(studentAssignmentId);
            if (studentAssignment == null) return NotFound();

            studentAssignment.Score = score;
            studentAssignment.TeacherComment = comment;
            studentAssignment.Status = AssignmentStatus.Graded;
            
            _context.Update(studentAssignment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CheckSubmissions), new { id = studentAssignment.AssignmentId });
        }
    }
}
