using Jumia.Dtos.ViewModel.Order;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Contract
{
    public interface IUserRepository : IRepository<ApplicationUser, string>
    {
        Task<IncreaseEarning> IncreaseEarnings(string userId, decimal amountToAdd);
        Task<decimal?> GetEarningByUserIdAsync(string userId);
        Task<bool> RequestWithdrawal(string userId, decimal requestedAmount, string withdrawalMethod, string phoneNumber);
    }
}
