using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TranslatorMVC.Models
{
    public class Shift
    {
        [Key]
        public int ShiftID { get; set; }  // Assuming you have a primary key for Shift
        public DateTime Shift_Start { get; set; }  // Start time of the shift
        public DateTime Shift_End { get; set; }  // End time of the shift
        public int EmployeeID { get; set; }  // Foreign key to Employee


        //public int EmployeeProjectID { get; set; }  // Foreign key to EmployeePRoject
        //public int Assigned { get; set; }


        public int Emp_Capacity { get; set; }  // Capacity for the shift (could be number of employees or similar)
        public bool Shift_IsDeleted { get; set; }
        public DateTime? Shift_CreateDate { get; set; }  // Date when the shift was created
        public DateTime? Shift_UpdateDate { get; set; }  // Nullable date when the shift was last updated
        public DateTime? Shift_DeletedDate { get; set; }
        public bool Shift_Active { get; set; }  // Flag indicating if the shift is active

        // Navigation properties to related entities
        [ValidateNever]
        public Employee Employee { get; set; }

        [ValidateNever]
        // Navigation property for related EmployeeProjects
        public ICollection<EmployeeProject> Employeeproject { get; set; }

    }

}
