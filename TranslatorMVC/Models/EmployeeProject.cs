using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TranslatorMVC.Models
{
    public class EmployeeProject
    {
        [Key]
        public int EmployeeProjectID { get; set; } // Primary Key
        [Required]
        public int ProjectID { get; set; } // Foreign Key to Project
        [Required]
        public int Assigned { get; set; } // Assigned flag
        [Required]
        public int EmployeeID { get; set; } // Foreign Key to Employees
        public int ShiftID { get; set; } // Foreign Key to Employees

        public bool isCompleted { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? CompleteDate { get; set; }

        // Navigation properties to related entities

        [ValidateNever]
        public Project Project { get; set; }

        [ValidateNever]
        public Shift Shift { get; set; }

        [ValidateNever]
        public Employee Employee { get; set; }


    }
}
