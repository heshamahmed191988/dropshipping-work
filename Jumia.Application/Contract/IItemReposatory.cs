using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Contract
{
    public interface IItemReposatory : IRepository<Item,int>
    {
         Task<int> GetProductID(string ProductName);
        Task<string> GetProductName(int ID);
        Task<string> GetColorByProductIdAsync(int productId);
    }
}
