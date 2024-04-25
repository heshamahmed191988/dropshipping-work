using Jumia.Dtos.ResultView;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Contract
{
    public interface IProductReposatory : IRepository<Product, int>
    {
        //to add any bonus functionality that not found on ireposatory 
        Task<IQueryable<Product>> SearchByName(string name);
        Task<IQueryable<Product>> SearchByPrice(decimal minprice, decimal maxprice);
        Task<IQueryable<Product>> SearchByCategoriey(int catid);
        //Task<IQueryable<Product>> SearchByNameAr(string name);
        Task<IQueryable<Product>> SearchByBrand(string name);

        Task<List<string>> GetAllBrands();

        Task<IEnumerable<Product>> GetByBrandAsync(string brandName);
        Task<IEnumerable<Product>> GetByCategoryAndBrandAsync(int categoryId, string brandName);
    }
}