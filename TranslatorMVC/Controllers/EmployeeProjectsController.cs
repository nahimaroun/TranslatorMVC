using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
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
        // GET: EmployeeProjects/Create
        public IActionResult Create(int projectId)
        {
            // Fetch the project with the given projectId
            var project = _context.Project.Where(p => p.ProjectID == projectId).FirstOrDefault();

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            // Fetch the total assigned words for the current project
            var totalAssignedForProject = _context.EmployeeProject
                .Where(ep => ep.ProjectID == projectId && !ep.isCompleted) // Only include ongoing tasks
                .Sum(ep => (int?)ep.Assigned) ?? 0; // Sum the assigned words, default to 0 if no records

            // Calculate the remaining capacity for the project
            var remainingProjectCapacity = project.Proj_Capacity - totalAssignedForProject;

            // Populate ViewBag with necessary data for the view
            ViewBag.EmployeeID = _context.Employee?
                .OrderBy(e => e.Emp_Name)?
                .Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Emp_Name })
                .ToList();

            ViewBag.ProjectID = projectId;
            ViewBag.RemainingProjectCapacity = remainingProjectCapacity; // Pass remaining capacity to the view

            // Create the model with the provided projectId
            var model = new EmployeeProject
            {
                ProjectID = projectId,
                Project = project
            };

            return View(model);
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

        [HttpGet]
        public IActionResult GetShiftsForDate(DateTime selectedDate)
        {
            try
            {
                // Fetch shifts for the selected date and include the Employee navigation property
                var shifts = _context.Shift
                    .Where(s => s.Shift_Start.Date == selectedDate.Date && s.Shift_IsDeleted == false)
                    .Include(s => s.Employee) // Assuming Employee is a navigation property in Shift
                    .ToList();



                var response = new List<object>();

                if (shifts.Any())
                {
                    foreach (var s in shifts)
                    {
                        // Get the employee name from the Employee table using the EmployeeID
                        var employeeName = s.Employee?.Emp_Name; // Assuming Emp_Name is a field in Employee

                        // Fetch rows from EmployeeProject where ShiftID matches the current shift's ShiftID
                        var employeeProjects = _context.EmployeeProject
                            .Where(ep => ep.EmployeeID == s.EmployeeID && ep.ShiftID == s.ShiftID && ep.isCompleted == false) // Filter by EmployeeID and ShiftID
                            .ToList();

                        // Sum the Assigned values from these EmployeeProject rows
                        var totalAssigned = employeeProjects.Sum(ep => ep.Assigned);

                        // Calculate the remaining capacity by subtracting the total assigned from the shift capacity
                        var remainingCapacity = s.Emp_Capacity - totalAssigned;

                        // Build response object with employee name, shift details, and remaining capacity
                        response.Add(new
                        {

                            shiftID = s.ShiftID,
                            EmployeeId = s.EmployeeID,
                            Emp_Name = employeeName,
                            Shift_Start = s.Shift_Start,
                            Shift_End = s.Shift_End,
                            Remaining_Capacity = remainingCapacity
                        });
                    }
                }

                return Json(new
                {
                    success = true,
                    shifts = response
                });
            }
            catch (Exception ex)
            {
                // Log the exception and return a user-friendly error message
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while fetching shifts.");
            }
        }


        [HttpPost]
        public IActionResult AssignWords(int ShiftID, int EmployeeID, int ProjectID, int Assigned)
        {

            try
            {
                // Fetch the shift to validate if the shift exists and capacity
                var shift = _context.Shift
                    .FirstOrDefault(s => s.ShiftID == ShiftID);

                if (shift == null)
                {
                    return BadRequest("Invalid shift ID.");
                }

                // Fetch any existing assignment for the same employee and shift (optional, if needed)
                var existingAssignment = _context.EmployeeProject
                    .FirstOrDefault(ep => ep.ShiftID == ShiftID && ep.EmployeeID == EmployeeID && ep.isCompleted == false);

                // Calculate remaining capacity (subtract assigned words from capacity)
                var remainingCapacity = shift.Emp_Capacity - (existingAssignment?.Assigned ?? 0);

                // this is for testing purposes
                // return BadRequest(remainingCapacity);

                if (Assigned > remainingCapacity)
                {
                    return BadRequest("Assigned words exceed remaining capacity.");
                }

                // Create a new EmployeeProject record with the provided EmployeeID, ProjectID, ShiftID, and assignedWords
                var newEmployeeProject = new EmployeeProject
                {

                    ShiftID = ShiftID,
                    EmployeeID = EmployeeID,
                    ProjectID = ProjectID,  // Make sure ProjectID is added in the EmployeeProject model
                    Assigned = Assigned
                };

                // Add the new record to the database
                _context.EmployeeProject.Add(newEmployeeProject);
                _context.SaveChanges();

                return Ok("Words assigned successfully!");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging and return a user-friendly error message
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while assigning words.");
            }
        }


    }
}
