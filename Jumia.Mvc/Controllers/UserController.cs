using Jumia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jumia.Mvc.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                // Handle error, maybe show an error message
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

