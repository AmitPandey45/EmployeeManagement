using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            IEnumerable<ApplicationUser> users = userManager.Users.ToList();

            return View(users);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            IEnumerable<IdentityRole> roles = roleManager.Roles.ToList();

            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("listroles", "administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            IEnumerable<ApplicationUser> users = userManager.Users.ToList();

            foreach (var user in users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = await roleManager.FindByIdAsync(model.Id);

                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                    return View("NotFound");
                }

                role.Name = model.RoleName;
                IdentityResult result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("listroles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            IdentityRole role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            ViewBag.roleId = role.Id;
            var viewModel = new List<UserRoleViewModel>();

            IEnumerable<ApplicationUser> users = userManager.Users.ToList();

            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }

                viewModel.Add(userRoleViewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(string roleId, IEnumerable<UserRoleViewModel> model)
        {
            IdentityRole role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            IdentityResult result = null;

            foreach (UserRoleViewModel userRoleViewModel in model)
            {
                result = null;
                ApplicationUser user = await userManager.FindByIdAsync(userRoleViewModel.UserId);
                bool isUserExistInRole = await userManager.IsInRoleAsync(user, role.Name);

                if (userRoleViewModel.IsSelected && !isUserExistInRole)
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!userRoleViewModel.IsSelected && isUserExistInRole)
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            IEnumerable<string> userRoles = await userManager.GetRolesAsync(user);
            IEnumerable<Claim> userClaims = await userManager.GetClaimsAsync(user);

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Roles = userRoles,
                Claims = userClaims.Select(s => s.Value).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                    return View("NotFound");
                }

                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                IdentityResult result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("listusers");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);

                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                    return View("NotFound");
                }

                IdentityResult result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("listusers");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("listusers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = await roleManager.FindByIdAsync(id);

                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                    return View("NotFound");
                }

                try
                {
                    IdentityResult result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("listroles");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Error deleting role {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $@"{role.Name} role cannot be deleted as there are users in this role.
                                             If you want to delete this role, please remove the users from the role
                                             and then try to delete";

                    return View("Error");
                }
            }

            return View("listroles");
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            ViewBag.UserId = user.Id;

            var viewModel = new List<UserRolesViewModel>();
            IEnumerable<IdentityRole> roles = roleManager.Roles.ToList();

            foreach (var role in roles)
            {
                var userRole = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRole.IsSelected = true;
                }

                viewModel.Add(userRole);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(string userId, IEnumerable<UserRolesViewModel> model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                    return View("NotFound");
                }

                IList<string> roles = await userManager.GetRolesAsync(user);
                IdentityResult result = null;

                //// First Approach - Remove existing roles for User and Add all selected roles for user
                //result = await userManager.RemoveFromRolesAsync(user, roles);

                //if (!result.Succeeded)
                //{
                //    ModelState.AddModelError(string.Empty, "Cannot remove user existing roles");
                //    return View(model);
                //}

                //result = await userManager.AddToRolesAsync(user,
                //    model.Where(s => s.IsSelected).Select(s => s.RoleName));

                //if (!result.Succeeded)
                //{
                //    ModelState.AddModelError(string.Empty, "Cannot add selected roles to user");
                //    return View(model);
                //}

                //// Second Approach
                //// Step1: Remove those roles which are exist for user but not selected while updating for user
                IEnumerable<string> selectedAllRoles = model.Where(w => w.IsSelected)
                    .Select(s => s.RoleName);
                IEnumerable<string> removeNotSelectedRoles = roles.Except(selectedAllRoles);
                result = await userManager.RemoveFromRolesAsync(user, removeNotSelectedRoles);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Cannot remove user existing roles");
                    return View(model);
                }

                //// Step2: Add those selected roles which do not exist for user while updating
                IEnumerable<string> addSelectedRoles = selectedAllRoles.Except(roles);
                result = await userManager.AddToRolesAsync(user, addSelectedRoles);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Cannot add selected roles to user");
                    return View(model);
                }
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }

    }
}
