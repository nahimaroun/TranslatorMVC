// ViewModels/ShiftViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TranslatorMVC.ViewModels
{
    public class ShiftViewModel
    {
        public int ShiftID { get; set; }

        [Display(Name = "Shift Start")]
        public DateTime Shift_Start { get; set; }

        [Display(Name = "Shift End")]
        public DateTime Shift_End { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Employee Capacity")]
        public int Emp_Capacity { get; set; }

        [Display(Name = "Assigned")]
        public int Emp_Assigned { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Shift_CreateDate { get; set; }

        [Display(Name = "Active")]
        public bool Shift_Active { get; set; }
    }
}
