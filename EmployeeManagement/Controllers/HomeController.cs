using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        public ViewResult Details(int id = 1)
        {
            var homeDetailsViewModel = new HomeDetailsViewModel
            {
                Employee = employeeRepository.GetEmployee(id),
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
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach(IFormFile photo in model.Photos)
                    {
                        string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                        uniqueFileName = $"{Guid.NewGuid().ToString()}_{photo.FileName}";
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }

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
    }
}
