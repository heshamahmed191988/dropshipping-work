using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.Dtos.ViewModel.Order;
using Jumia.Dtos.ViewModel.User;
using Jumia.model;
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

        public async Task<bool> RequestWithdrawal(string userId, decimal requestedAmount, string withdrawalMethod, string phoneNumber,
            string? status,decimal? NumberOfWithdrawl)
        {
            // Find the user by userId
            var user = await _jumiacontext.Users.FindAsync(userId);
            if (user == null)
            {
                // User not found
                return false;
            }

            // Check if the requested amount is greater than the user's earnings
            if (requestedAmount > user.Earning)
            {
                // Requested amount exceeds user's earnings
                return false;
            }

            // Deduct the requested amount from the user's earnings
            user.Earning -= requestedAmount;

            // Create a new transaction record
            var transaction = new Transaction
            {
                UserId = userId,
                User = user,
                userName = user.UserName,
                Amount = requestedAmount,
                WithdrawalMethod = withdrawalMethod,
                Phone = phoneNumber,
                Status=status,
                NumberOfWithdrawl=NumberOfWithdrawl,
                DatePlaced = DateTime.Now
            };

            // Add the transaction to the context
            _jumiacontext.Transactions.Add(transaction);

            // Save changes to the database
            await _jumiacontext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Transaction>> GetTransactionsWithPaginationAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1; // Ensure pageNumber is at least 1
            }

            // Calculate the number of records to skip
            int skipCount = (pageNumber - 1) * pageSize;

            return await _jumiacontext.Transactions
                .OrderByDescending(t => t.DatePlaced)
                .Skip(skipCount)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<List<Transaction>> GetTransactionsByUserIdAsync(string userId, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1; // Ensure pageNumber is at least 1
            }

            // Calculate the number of records to skip
            int skipCount = (pageNumber - 1) * pageSize;

            return await _jumiaContext.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.DatePlaced)
                .Skip(skipCount)
                .Take(pageSize)
                .ToListAsync();
        }
    

    public async Task UpdateTransactionStatusAsync(int transactionId, string status)
        {
            var transaction = await _jumiacontext.Transactions.FindAsync(transactionId);
            if (transaction != null)
            {
                transaction.Status = status;
                await _jumiacontext.SaveChangesAsync();
            }
        }


        public async Task<int> GetTotalTransactionCountAsync()
        {
            return await _jumiacontext.Transactions.CountAsync();
        }
    }



    // Implement any additional methods specific to ApplicationUser here
}

