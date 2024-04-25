using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.InfraStructure.Repository;
using Jumia.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jumia.Infrastructure.Repository
{
    public class ReviewRepository : Repository<Review, int>, IReviewRepository
    {
        public ReviewRepository(JumiaContext jumiaContext) : base(jumiaContext) { }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId)
        {
            return await _Dbset.Where(r => r.UserID == userId).ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
        {
            return await _Dbset.Where(r => r.ProductID == productId).ToListAsync();
        }

        // Additional methods as needed...
    }
}
