using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TranslatorMVC.Models;

namespace TranslatorMVC.Controllers
{
    public class EmployeeProjectsController : Controller
    {
        private readonly AppDBContext _context;

        public EmployeeProjectsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: EmployeeProjects
        public async Task<IActionResult> Index()
        {
            var employeeProjects = await _context.EmployeeProject
            .Include(ep => ep.Project) // Include related Project
            .Include(ep => ep.Employee) // Include related Employee
            .Where(e => !e.isCompleted)
            .ToListAsync();

            return View(employeeProjects);
        }

        // GET: EmployeeProjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProject = await _context.EmployeeProject
                .FirstOrDefaultAsync(m => m.EmployeeProjectID == id);
            if (employeeProject == null)
            {
                return NotFound();
            }

            return View(employeeProject);
        }

        // GET: EmployeeProjects/Create
        public IActionResult Create(int projectId)
        {
            ViewBag.EmployeeID = _context.Employee?.OrderBy(e => e.Emp_Name)?.Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Emp_Name }).ToList();
            // ViewBag.ProjectID = _context.Project?.OrderBy(e => e.Proj_Name)?.Select(e => new SelectListItem { Value = e.ProjectID.ToString(), Text = e.Proj_Name }).ToList();

            var p = _context.Project.Where(e => e.ProjectID == projectId).FirstOrDefault();
            // Use the projectId parameter here
            var model = new EmployeeProject
            {
                ProjectID = projectId,
                Project = p
            };

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeID, ProjectID, Assigned,")] EmployeeProject employeeProject)
        {


            if (ModelState.IsValid)
            {
                _context.Add(employeeProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Re - populate ViewBag in case of validation errors
            ViewBag.EmployeeID = _context.Employee?.OrderBy(e => e.Emp_Name)?.Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Emp_Name }).ToList();

            return View(employeeProject);
        }



        // GET: EmployeeProjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProject = await _context.EmployeeProject.FindAsync(id);
            if (employeeProject == null)
            {
                return NotFound();
            }
            ViewBag.EmployeeID = _context.Employee?.OrderBy(e => e.Emp_Name)?.Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Emp_Name }).ToList();
            ViewBag.ProjectID = _context.Project?.OrderBy(e => e.Proj_Name)?.Select(e => new SelectListItem { Value = e.ProjectID.ToString(), Text = e.Proj_Name }).ToList();
            return View(employeeProject);
        }

        // POST: EmployeeProjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeProjectID, EmployeeID, ProjectID, Assigned, isCompleted, EditDate, CompleteDate")] EmployeeProject employeeProject)
        {
            if (id != employeeProject.EmployeeProjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    employeeProject.EditDate = DateTime.Now;

                    _context.Update(employeeProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeProjectExists(employeeProject.EmployeeProjectID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));

            }

            // Repopulate dropdown lists in case of validation errors
            ViewBag.EmployeeID = _context.Employee?.OrderBy(e => e.Emp_Name)?.Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Emp_Name }).ToList();
            ViewBag.ProjectID = _context.Project?.OrderBy(e => e.Proj_Name)?.Select(e => new SelectListItem { Value = e.ProjectID.ToString(), Text = e.Proj_Name }).ToList();

            return View(employeeProject);
        }

        // GET: EmployeeProjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeProject = await _context.EmployeeProject
                .FirstOrDefaultAsync(m => m.EmployeeProjectID == id);
            if (employeeProject == null)
            {
                return NotFound();
            }

            return View(employeeProject);
        }

        // POST: EmployeeProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeProject = await _context.EmployeeProject.FindAsync(id);
            if (employeeProject != null)
            {
                employeeProject.CompleteDate = DateTime.Now;
                employeeProject.isCompleted = true;
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeProjectExists(int id)
        {
            return _context.EmployeeProject.Any(e => e.EmployeeProjectID == id);
        }

        [HttpPost]
        public IActionResult GetShiftsForDate(DateTime selectedDate)
        {
            try
            {
                var shifts = _context.Shift
                    .Where(s => s.Shift_Start.Date == selectedDate.Date && s.Shift_Active)
                    .Include(s => s.Employeeproject) // Include EmployeeProjects navigation property
                    .ToList();

                var shiftEmployees = shifts.SelectMany(s => s.Employeeproject)
                    .GroupBy(ep => ep.EmployeeID)
                    .Select(g => new
                    {
                        Employee = g.FirstOrDefault()?.Employee, // Get employee details
                        Shift = g.FirstOrDefault()?.Shift, // Get shift details
                        RemainingCapacity = g.FirstOrDefault()?.Shift.Emp_Capacity - g.Sum(ep => ep.Assigned)
                    })
                    .ToList();

                return Json(shiftEmployees);
            }
            catch (Exception ex)
            {
                // Log the exception and return a user-friendly error message
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while fetching shifts.");
            }
        }

        [HttpPost]
        public IActionResult AssignWords(int shiftId, int employeeId, int assignedWords)
        {
            try
            {
                var employeeProject = _context.EmployeeProject
                    .FirstOrDefault(ep => ep.ShiftID == shiftId && ep.EmployeeID == employeeId);

                if (employeeProject == null)
                {
                    employeeProject = new EmployeeProject
                    {
                        ShiftID = shiftId,
                        EmployeeID = employeeId,
                        Assigned = assignedWords
                    };
                    _context.EmployeeProject.Add(employeeProject);
                }
                else
                {
                    employeeProject.Assigned += assignedWords;
                }

                _context.SaveChanges();
                return Ok("Words assigned successfully!");
            }
            catch (Exception ex)
            {
                // Log the exception and return a user-friendly error message
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while assigning words.");
            }
        }

    }
}
