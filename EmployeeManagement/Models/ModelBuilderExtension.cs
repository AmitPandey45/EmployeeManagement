using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void SeedEmployeeData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Mary",
                    Email = "mary@pragimtech.com",
                    Department = DepartmentEnum.IT
                },
                new Employee
                {
                    Id = 2,
                    Name = "John",
                    Email = "john@pragimtech.com",
                    Department = DepartmentEnum.HR
                });
        }
    }
}
