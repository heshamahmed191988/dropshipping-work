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
    public class ProductImageRepository : Repository<ProductImage, int>, IProductImageRepository
       {
        private readonly JumiaContext _context;

        public ProductImageRepository(JumiaContext context): base(context)
        {
            _context = context;
        }

        public async Task<ProductImage> CreateAsync(ProductImage productImage)
        {
            _context.productImages.Add(productImage);
            await _context.SaveChangesAsync();
            return productImage;
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _context.productImages
                                 .Where(pi => pi.Product.Id == productId)
                                 .ToListAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var productImage = await _context.productImages.FindAsync(id);
            if (productImage != null)
            {
                productImage.IsDeleted = true;
                _context.Entry(productImage).State = EntityState.Modified;
                _context.productImages.Update(productImage);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<ProductImage> GetByIdAsync(int id)
        {
            var productImage = await _context.productImages
                                              .FirstOrDefaultAsync(pi => pi.Id == id && !pi.IsDeleted);
            if (productImage == null)
            {
                throw new KeyNotFoundException($"A product image with the ID {id} was not found or has been deleted.");
            }
            return productImage;
        }

    }
}
