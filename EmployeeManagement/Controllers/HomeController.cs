using EmployeeManagement.Enums;
using EmployeeManagement.Extensions;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;

        public HomeController(IEmployeeRepository employeeRepository,
            IHostingEnvironment hostingEnvironment,
            ILogger<HomeController> logger)
        {
            this.employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            var description = ClaimRole.CreateRole.GetDescription();

            var dict1 = EnumExtension.ToDictionary<ClaimRole>();
            var dict2 = EnumExtension.ToDictionaryWithEnumAsKeyValue<ClaimRole>();
            var dict3 = EnumExtension.ToDictionaryWithDescriptionAsValue<ClaimRole>();

            var dict4 = EnumExtension.ToDictionary<ClaimPolicy>();
            var dict5 = EnumExtension.ToDictionaryWithEnumAsKeyValue<ClaimPolicy>();
            var dict6 = EnumExtension.ToDictionaryWithDescriptionAsValue<ClaimPolicy>();

            var ddd = User.Claims.Where(c => c.Type == ClaimTypes.Role);

            return View(employeeRepository.GetAllEmployee());
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
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
        public IActionResult Edit(EmployeeEditViewModel model)
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
