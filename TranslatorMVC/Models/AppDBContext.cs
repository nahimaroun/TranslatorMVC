namespace TranslatorMVC.Models
{
    using Microsoft.EntityFrameworkCore;

    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        // Add DbSet properties for your tables
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<EmployeeProject> EmployeeProject { get; set; }



    }
}
