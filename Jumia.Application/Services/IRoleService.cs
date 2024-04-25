using Jumia.Dtos.ViewModel.User;
using Jumia.Model;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public interface IRoleService
    {
        Task<IdentityResult> CreateRoleAsync(RoleViewModel model);
        Task<List<string>> GetRolesAsync();
        Task<IdentityResult> CreateUserAsync(string userName, string email, string password, string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName);
    }
}