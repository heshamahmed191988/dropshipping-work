using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Contract
{
    public interface IProductImageRepository : IRepository<ProductImage, int>
    {
        Task<ProductImage> CreateAsync(ProductImage productImage);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId);
        Task SoftDeleteAsync(int id);
        Task<ProductImage> GetByIdAsync(int id);
    }
}
