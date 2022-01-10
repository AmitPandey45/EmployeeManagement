using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> employees;

        public MockEmployeeRepository()
        {
            employees = new List<Employee>
            {
                new Employee
                {
                    Id =1,
                    Name = "Mary",
                    Department = DepartmentEnum.HR,
                    Email = "mary@pragimtech.com"
                },
                new Employee
                {
                    Id = 2,
                    Name = "John",
                    Department = DepartmentEnum.IT,
                    Email = "john@pragimtech.com"
                },
                new Employee
                {
                    Id = 3,
                    Name = "Sam",
                    Department = DepartmentEnum.IT,
                    Email = "sam@pragimtech.com"
                }
            };
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return employees;
        }

        public Employee GetEmployee(int id)
        {
            return employees.FirstOrDefault(s => s.Id == id);
        }

        public Employee Add(Employee employee)
        {
            employee.Id = employees.Max(s => s.Id) + 1;
            employees.Add(employee);
            return employee;
        }

        public Employee Update(Employee employeeChanges)
        {
            Employee employee = employees.FirstOrDefault(s => s.Id == employeeChanges.Id);
            if (employee != null)
            {
                employee.Name = employeeChanges.Name;
                employee.Department = employeeChanges.Department;
                employee.Email = employeeChanges.Email;
            }

            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = employees.FirstOrDefault(s => s.Id == id);
            if (employee != null)
            {
                employees.Remove(employee);
            }

            return employee;
        }
    }
}
