using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;
using TeacherWorkplace.Filters;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Controllers
{
    [AuthorizeFilter]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GroupsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index() => 
            View(await _context.Groups.Include(g => g.Students).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var group = await _context.Groups.Include(g => g.Students).ThenInclude(s => s.Grades)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null) return NotFound();
            
            ViewBag.AverageGrade = group.Students.SelectMany(s => s.Grades).Any()
                ? Math.Round(group.Students.SelectMany(s => s.Grades).Average(g => g.Score), 2)
                : 0;
            return View(group);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var group = await _context.Groups.FindAsync(id);
            return group == null ? NotFound() : View(group);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Group group)
        {
            if (id != group.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var group = await _context.Groups.FirstOrDefaultAsync(m => m.Id == id);
            return group == null ? NotFound() : View(group);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group != null) _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
