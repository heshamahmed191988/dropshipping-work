using Jumia.Dtos.ViewModel.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public interface IUserService
    {
        Task<UserViewModel> CreateUserAsync(UserViewModel model);
        Task<UserViewModel> GetUserByIdAsync(string id);

        Task<UserViewModel> UpdateUserAsync(UserViewModel model);
        //Task<LoginViewModel> UpdatePasswordAsync(LoginViewModel model);
        Task<UpdatUserInfo> UpdateUsernameAndPasswordAsync(string userId, string currentPassword, string newUsername, string newPassword, string confirmPassword);

        Task<bool> DeleteUserAsync(string id);
        Task<IEnumerable<UserViewModel>> GetAllUsersAsync();
        Task<decimal?> GetUserEarningAsync(string userId);

    }
}