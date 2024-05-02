using Jumia.Application.Services;
using Jumia.Dtos.ViewModel.User;
using Jumia.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jumia.Mvc.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;

        public RolesController(IRoleService roleService, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _roleService = roleService;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleService.CreateRoleAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "admin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error creating role.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterUserWithRole()
        {
            // Populate the roles for the view
            ViewBag.Roles = await _roleService.GetRolesAsync();
            return View();
        }







        [HttpPost]
        public async Task<IActionResult> RegisterUserWithRole(RegisterViewModel model, string roleName)
        {
            // Populate ViewBag.Roles
            ViewBag.Roles = await _roleService.GetRolesAsync();

            if (ModelState.IsValid)
            {
                var result = await _roleService.CreateUserAsync(model.UserName, model.Email, model.Password, roleName);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin"); // or any other page
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }














        /// only have problem with same username 

        //[HttpPost]
        //public async Task<IActionResult> RegisterUserWithRole(RegisterViewModel model, string roleName)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _roleService.CreateUserAsync(model.UserName, model.Email, model.Password, roleName);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index", "Admin"); // or any other page
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    ViewBag.Roles = _roleService.GetRolesAsync(); // Repopulate roles in case of failure
        //    return View(model);
        //}




    }
}
