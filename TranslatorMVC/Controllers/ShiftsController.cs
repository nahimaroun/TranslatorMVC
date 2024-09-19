using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TranslatorMVC.Models;
using TranslatorMVC.ViewModels;

namespace TranslatorMVC.Controllers
{
    public class ShiftsController : Controller
    {
        private readonly AppDBContext _context;

        public ShiftsController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Load shifts and include the related Employee entity
            var shifts = await _context.Shift
                .Include(s => s.Employee) // Eager load the Employee entity
                .Where(s => !s.Shift_IsDeleted)
                .ToListAsync();

            // Create a view model for each shift, using the related Employee's name
            var viewModel = shifts.Select(s => new ShiftViewModel
            {
                ShiftID = s.ShiftID,
                Shift_Start = s.Shift_Start,
                Shift_End = s.Shift_End,
                EmployeeName = s.Employee.Emp_Name, // Access Employee name via navigation property
                Emp_Capacity = s.Emp_Capacity,
                Shift_Active = s.Shift_Active
            }).ToList();

            return View(viewModel);
        }


        // GET: Shifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift
                .FirstOrDefaultAsync(m => m.ShiftID == id);
            if (shift == null)
            {
                return NotFound();
            }

            return View(shift);
        }

        // GET: Shifts/Create
        public IActionResult Create()
        {
            ViewBag.Employees = new SelectList(_context.Employee.Where(e => !e.Emp_IsDeleted), "EmployeeID", "Emp_Name");
            return View();
        }

        // POST: Shifts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShiftID,Shift_Start,Shift_End,EmployeeID,Emp_Capacity,Emp_Assigned,Shift_CreateDate,Shift_UpdateDate,Shift_Active")] Shift shift)
        {

            // Check if the EmployeeID is valid
            var employee = await _context.Employee
                                         .Where(e => e.EmployeeID == shift.EmployeeID && !e.Emp_IsDeleted)
                                         .FirstOrDefaultAsync();



            if (employee == null)
            {
                ModelState.AddModelError("EmployeeID", "Invalid EmployeeID. Please choose a valid employee.");
                return View(shift);
            }

            if (ModelState.IsValid)
            {
                shift.Shift_CreateDate = DateTime.Now;
                _context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(shift);
        }

        // GET: Shifts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Employees = new SelectList(_context.Employee.Where(e => !e.Emp_IsDeleted), "EmployeeID", "Emp_Name");
            var shift = await _context.Shift.FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }
            return View(shift);
        }

        // POST: Shifts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShiftID,Shift_Start,Shift_End,EmployeeID,Emp_Capacity,Emp_Assigned,Shift_CreateDate,Shift_UpdateDate,Shift_Active")] Shift shift)
        {
            if (id != shift.ShiftID)
            {
                return NotFound();
            }

            // Check if the EmployeeID is valid
            var employee = await _context.Employee
                                         .Where(e => e.EmployeeID == shift.EmployeeID && !e.Emp_IsDeleted)
                                         .FirstOrDefaultAsync();

            if (employee == null)
            {
                ModelState.AddModelError("EmployeeID", "Invalid EmployeeID. Please choose a valid employee.");
                return View(shift);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    shift.Shift_UpdateDate = DateTime.Now;
                    _context.Update(shift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftExists(shift.ShiftID))
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

            return View(shift);
        }


        // GET: Shifts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift
                .FirstOrDefaultAsync(m => m.ShiftID == id);
            if (shift == null)
            {
                return NotFound();
            }

            return View(shift);
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _context.Shift.FindAsync(id);
            if (shift != null)
            {
                shift.Shift_IsDeleted = true;
                shift.Shift_DeletedDate = DateTime.Now;

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftExists(int id)
        {
            return _context.Shift.Any(e => e.ShiftID == id);
        }
    }
}
