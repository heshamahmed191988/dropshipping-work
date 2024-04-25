using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.InfraStructure.Repository
{
    public class ItemRepostory : Repository<Item, int>, IItemReposatory
    {
        private readonly JumiaContext _context;

        public ItemRepostory(JumiaContext context) : base(context)
        {
            _context = context;
        }

        public Task<int> GetProductID(string ProductName)
        {
            return Task.FromResult(_context.products.Where(p => p.NameEn == ProductName).Select(p => p.Id).FirstOrDefault());
        }
        public Task<string> GetProductName(int ID)
        {
            return Task.FromResult(_context.products.Where(p => p.Id == ID).Select(p => p.NameEn).FirstOrDefault());
        }

        public async Task<string> GetColorByProductIdAsync(int productId)
        {
            
            var color = await _context.items
                                      .Where(i => i.ProductID == productId && !i.IsDeleted)
                                      .Select(i => i.Color)
                                      .FirstOrDefaultAsync();

            return color;
        }

    }
}
