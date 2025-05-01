using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using RDMA_Graduation_Prep.Data;
using RDMA_Graduation_Prep.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RDMA_Graduation_Prep.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery, int page = 1, int pageSize = 10)
        {
            var studentsQuery = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                studentsQuery = studentsQuery.Where(s =>
                    s.LastName.Contains(searchQuery) ||
                    s.FirstName.Contains(searchQuery) ||
                    s.ClassName.Contains(searchQuery) ||
                    s.CurrentBelt.Contains(searchQuery));
            }

            int totalStudents = await studentsQuery.CountAsync();
            var students = await studentsQuery
                                    .OrderBy(s => s.LastName)
                                    .ThenBy(s => s.FirstName)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalStudents / pageSize);
            ViewBag.SearchQuery = searchQuery;

            return View(students);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,ClassName,CurrentBelt,StripeCount")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("LastAddedStudent", student.FirstName + " " + student.LastName);
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public IActionResult LastAdded()
        {
            ViewBag.LastAddedStudent = HttpContext.Session.GetString("LastAddedStudent") ?? "No student added yet.";
            return View();
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,ClassName,CurrentBelt,StripeCount")] Student student)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
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
