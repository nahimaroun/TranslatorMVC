using System.ComponentModel.DataAnnotations;

namespace TranslatorMVC.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }  // Assuming you already have this as the primary key

        public string Proj_Name { get; set; }  // Project name
        public string Proj_ClientName { get; set; }  // Client name
        public int Proj_Capacity { get; set; }  // Project capacity (adjust type as needed)
        public int Proj_Price { get; set; }  // Project price (decimal is good for monetary values)
        public DateTime Proj_DeadLine { get; set; }  // Deadline for the project
        public DateTime Proj_StartDate { get; set; }  // Start date of the project
        public string Proj_Note { get; set; }  // Any notes about the project
        public bool Proj_IsDeleted { get; set; }  // Soft delete flag
        public bool Proj_Active { get; set; }  // Active/inactive status

        public DateTime Proj_CreateDate { get; set; }  // Creation date
        public DateTime? Proj_EditDate { get; set; }  // Nullable edit date
        public DateTime? Proj_DeleteDate { get; set; }  // Nullable delete date


    }

}
