using System.ComponentModel.DataAnnotations;

namespace TranslatorMVC.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }  // Assuming you already have this as the primary key

        public string Emp_Name { get; set; }
        public string Emp_LastName { get; set; }
        public string Emp_Type { get; set; }  // This could be an enum if you want to restrict the types
        public string? Emp_Notes { get; set; }
        public bool Emp_Active { get; set; }  // Active or inactive status
        public bool Emp_IsDeleted { get; set; }  // Soft delete flag

        public DateTime? Emp_CreateDate { get; set; }
        public DateTime? Emp_EditDate { get; set; }  // Nullable if edits are optional
        public DateTime? Emp_DeleteDate { get; set; }  // Nullable for soft delete date


    }

}
