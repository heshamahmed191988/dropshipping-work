using Jumia.Context;
using Jumia.Dtos;
using Jumia.Dtos.ViewModel.User;
using Jumia.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JumiaContext context;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,JumiaContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            this.context = context;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {


            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = model.Password,



            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Perform the sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);
            }


            return result;
        }
        public async Task<SignInResult> SignInAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            return result;
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();

        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            // Fetch all users
            var users = _userManager.Users.ToList();

            // Map users to your UserViewModel
            // This step assumes you have a UserViewModel defined somewhere that fits your needs
            var userViewModels = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
                // Add other properties as needed
            }).ToList();

            return userViewModels;
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;

        }

        public async Task<int?> GetAddressIdByUserIdAsync(string userId)
        {
            // Directly query the Addresses table to find an address for the given userId
            var latestAddressId = await context.addresses
        .Where(a => a.UserID == userId)
        .OrderByDescending(a => a.Id) // Order by Id in descending order
        .Select(a => a.Id) // Select the Id of the latest address
        .FirstOrDefaultAsync(); // Get the Id of the latest address that matches the userId

            return latestAddressId; // Return the ID of the address, or null if not found
        }



    }
}
    