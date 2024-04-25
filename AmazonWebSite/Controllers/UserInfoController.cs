using Jumia.Application.Services;
using Jumia.Dtos.ViewModel.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
namespace AmazonWebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public UserInfoController(IAuthService authService, IConfiguration config, IRoleService roleService, IUserService userService)
        {
            _authService = authService;
            _config = config;
            _roleService = roleService;
            _userService = userService;
        }



        [HttpPut]
        public async Task<IActionResult> Update(string userId, UpdatUserInfo model)
        {
            //    if (model.NewPassword != model.ConfirmPassword)
            //    {
            //        ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
            //        return BadRequest(ModelState);
            //    }

            var updatedUser = await _userService.UpdateUsernameAndPasswordAsync(userId, model.CurrentPassword, model.UserName, model.NewPassword, model.ConfirmPassword);

            if (updatedUser != null)
            {
                return Ok(updatedUser);
            }
            else
            {
                return Unauthorized("Invalid current password or username");
            }
        }
    }

}
