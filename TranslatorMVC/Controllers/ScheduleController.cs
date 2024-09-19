using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslatorMVC.Models;

namespace TranslatorMVC.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(AppDBContext context, ILogger<ScheduleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Schedule()
        {
            var shifts = await _context.Shift
                .Include(s => s.Employee)
                .Select(s => new
                {
                    s.ShiftID,
                    s.Shift_Start,
                    s.Shift_End,
                    s.Emp_Capacity,
                    // Sum of assigned employees from EmployeeProject where project corresponds to the shift
                    Assigned = _context.EmployeeProject
                        .Where(ep => ep.EmployeeID == s.EmployeeID)
                        .Sum(ep => ep.Assigned),
                    RemainingCapacity = s.Emp_Capacity - _context.EmployeeProject
                        .Where(ep => ep.EmployeeID == s.EmployeeID)
                        .Sum(ep => ep.Assigned),
                    EmployeeName = s.Employee.Emp_Name + " " + s.Employee.Emp_LastName
                }).ToListAsync();

            return View(shifts);
        }


        [HttpGet]
        public async Task<IActionResult> GetShifts()
        {
            var shifts = await _context.Shift
                .Where(s => s.Shift_Active && !s.Shift_IsDeleted)
                .Select(s => new
                {
                    title = s.Employee.Emp_Name + s.Employee.Emp_LastName + " (Capacity: " + s.Emp_Capacity + ")",
                    start = s.Shift_Start,
                    end = s.Shift_End
                }).ToListAsync();

            return Json(shifts);
        }



    }
}