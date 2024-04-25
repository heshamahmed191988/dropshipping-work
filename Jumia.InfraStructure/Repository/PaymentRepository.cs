using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.Model; // Ensure this is the correct namespace for your Payment entity
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Jumia.InfraStructure.Repository
{
    public class PaymentRepository : Repository<Payment, int>, IPaymentReposatory
    {
        private readonly JumiaContext _context;

        public PaymentRepository(JumiaContext jumiaContext) : base(jumiaContext)
        {
            _context = jumiaContext;
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Set<Payment>().ToListAsync();
        }


    }
}