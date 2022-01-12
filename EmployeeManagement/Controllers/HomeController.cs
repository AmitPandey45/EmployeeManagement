using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace EmployeeManagement.Controllers
{
    [Route("pragim/[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
            IHostingEnvironment hostingEnvironment)
        {
            this.employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [Route("")]
        [Route("~/pragim")]
        [Route("~/pragim/Home")]
        public ViewResult Index()
        {
            return View(employeeRepository.GetAllEmployee());
        }

        [Route("{id?}")]
        public ViewResult Details(int? id)
        {
            throw new Exception("An exception occurred while trying to get employee details");
            Employee employee = employeeRepository.GetEmployee(id.Value);

            if(employee ==null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            var homeDetailsViewModel = new HomeDetailsViewModel
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

                var newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };
                employeeRepository.Add(newEmployee);

                return RedirectToAction("Details", new { newEmployee.Id });
            }

            return View();
        }

        [HttpGet]
        [Route("{id?}")]
        public ViewResult Edit(int id)
        {
            Employee employee = employeeRepository.GetEmployee(id);
            var employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditViewModel);
        }

        [HttpPost]
        [Route("{id?}")]
        public IActionResult Edit(int id, EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if(model.Photo !=null)
                {
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                employeeRepository.Update(employee);

                return RedirectToAction("Details", new { employee.Id });
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = $"{Guid.NewGuid().ToString()}_{model.Photo.FileName}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
