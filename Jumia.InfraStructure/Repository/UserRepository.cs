using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.Dtos.ViewModel.Order;
using Jumia.Dtos.ViewModel.User;
using Jumia.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.InfraStructure.Repository
{
    public class UserRepository : Repository<ApplicationUser, string>, IUserRepository
    {

        private readonly JumiaContext _jumiacontext;

        public UserRepository(JumiaContext jumiaContext) : base(jumiaContext)
        {
            _jumiacontext = jumiaContext;
        }



        public async Task<IncreaseEarning> IncreaseEarnings(string userId, decimal amountToAdd)
        {
            var user = await _jumiacontext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null; // Handle case where user is not found

            user.Earning = (user.Earning ?? 0) + amountToAdd;
            await _jumiacontext.SaveChangesAsync();

            return new IncreaseEarning
            {
                Id = user.Id,
                Earning = user.Earning
            };
        }


        public async Task<decimal?> GetEarningByUserIdAsync(string userId)
        {
            return await _jumiacontext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Earning)
                .FirstOrDefaultAsync();
        }
        // Implement any additional methods specific to ApplicationUser here
    }
}
