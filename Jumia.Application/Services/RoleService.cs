using Jumia.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Jumia.Dtos.ViewModel.User;

namespace Jumia.Application.Services
{

    public class RoleService : IRoleService
        {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateRoleAsync(RoleViewModel model)
        {
            var role = new IdentityRole(model.RoleName);
            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> CreateUserAsync(string userName, string email, string password, string roleName)
        {
            var user = new ApplicationUser { UserName = userName, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Check if the role exists, if not, create it
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                // Assign the user to the role
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return result;
        }
        public async Task<List<string>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return roles;
        }
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
        {
            // If the role doesn't exist, create it
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Then, assign the user to the role
            return await _userManager.AddToRoleAsync(user, roleName);
        }
    }
    }



