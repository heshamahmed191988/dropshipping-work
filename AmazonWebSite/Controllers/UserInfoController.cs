using Jumia.Application.Services;
using Jumia.Dtos.ViewModel.User;
using Jumia.Model;
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

        [HttpGet("{userId}")]
        public async Task<ActionResult<decimal?>> GetUserEarning(string userId)
        {
            var earning = await _userService.GetUserEarningAsync(userId);
            if (earning == null)
            {
                return NotFound();
            }
            return Ok(earning);
        }

        [HttpPost("withdrawal")]
        public async Task<IActionResult> RequestWithdrawal([FromBody] WithdrawalRequest withdrawalRequest)
        {
            var success = await _userService.RequestWithdrawal(withdrawalRequest.UserId, withdrawalRequest.RequestedAmount, withdrawalRequest.WithdrawalMethod, 
                withdrawalRequest.PhoneNumber,withdrawalRequest.Status,withdrawalRequest.NumberOfWithdrawl);
            if (success)
            {
                return Ok("Withdrawal request successful.");
            }
            else
            {
                return BadRequest("Withdrawal request failed. Please check your requested amount and try again.");
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactionsByUserId(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var transactions = await _userService.GetTransactionsByUserIdAsync(userId, pageNumber, pageSize);
            return Ok(transactions);
        }
    }


}


 






