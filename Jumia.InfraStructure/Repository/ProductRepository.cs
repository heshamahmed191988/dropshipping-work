using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.Dtos.ResultView;
using Jumia.InfraStructure.Repository;
using Jumia.model;
using Jumia.Model;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.InfraStructure
{
    public class ProductRepository : Repository<Product, int>, IProductReposatory
    {
        private readonly JumiaContext _jumiacontext;

        public ProductRepository(JumiaContext jumiaContext) : base(jumiaContext)
        {
            _jumiacontext = jumiaContext;
        }

        public Task<IQueryable<Product>> SearchByCategoriey(int catid)
        {
            return Task.FromResult(_jumiacontext.products.Select(p => p).Where(p => p.CategoryID == catid));
        }

        public Task<IQueryable<Product>> SearchByName(string name)
        {
            return Task.FromResult(_jumiacontext.products.Where(p =>
                p.NameEn.Contains(name) || p.DescriptionEn.Contains(name) ||
                p.NameAr.Contains(name) || p.DescriptionAr.Contains(name)));
        }

        //public Task<IQueryable<Product>> SearchByNameAr(string name)
        //{
        //    return Task.FromResult(_jumiacontext.products.Where(p => p.NameAr.Contains(name) || p.DescriptionAr.Contains(name)));
        //}
        public Task<IQueryable<Product>> SearchByPrice(decimal minprice, decimal maxprice)
        {
            return Task.FromResult(_jumiacontext.products.Where(p => p.Price >= minprice && p.Price <= maxprice));
        }

        public Task<IQueryable<Product>> SearchByBrand(string brandName)
        {
            return Task.FromResult(_jumiacontext.products.Where(p => p.BrandNameEn == (brandName)));
        }
        public async Task<List<string>> GetAllBrands()
        {
            return await _jumiacontext.products.Select(p => p.BrandNameEn).Distinct().ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByBrandAsync(string brandName)
        {
            return await _jumiacontext.products.Where(p => p.BrandNameEn.Equals(brandName) && !p.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAndBrandAsync(int categoryId, string brandName)
        {
            return await _jumiacontext.products.Where(p => p.CategoryID == categoryId && p.BrandNameEn.Equals(brandName) && !p.IsDeleted).ToListAsync();
        }


        public async Task<int> GetTotalProductsCountAsync()
        {
            try
            {
                // Query the Orders DbSet and count the total number of orders
                var totalProductsCount = await _jumiacontext.products.CountAsync();
                return totalProductsCount;
            }
            catch (Exception ex)
            {
                // Handle any potential exceptions here
                // Log or throw as needed
                throw new Exception("Failed to retrieve total orders count: " + ex.Message, ex);
            }
        }
    }
}