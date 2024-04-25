using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.Model;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.InfraStructure.Repository
{
    public class OrderProductReposatory : Repository<OrderProduct, int>, IOrderProuduct
    {
        private JumiaContext _context;

        public OrderProductReposatory(JumiaContext jumiaContext) : base(jumiaContext)
        {
            _context = jumiaContext;
        }

        public async Task<OrderProduct> Sreach(int OrderID)
        {
            var orderProduct = await _context.orderProducts
                .Include(op => op.Order)
                .Include(op => op.Product)
                .FirstOrDefaultAsync(op =>
                    op.OrderId == OrderID)
                 ;

            return orderProduct;


        }

      
    }
}