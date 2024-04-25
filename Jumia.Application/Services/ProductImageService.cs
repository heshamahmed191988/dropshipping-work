using Jumia.Application.Contract;
using Jumia.Dtos.ViewModel.Product;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductReposatory productReposatory;

        public ProductImageService(IProductImageRepository productImageRepository,IProductReposatory productReposatory)
        {
            _productImageRepository = productImageRepository;
            this.productReposatory = productReposatory;
        }

        public async Task<ProductImageDto> CreateAsync(ProductImageDto productImageDto)
        {
            var productImage = new ProductImage
            {
                Path = productImageDto.Path,
                 ProductID= productImageDto.ProductID
            };

            await _productImageRepository.CreateAsync(productImage);
            await _productImageRepository.SaveChangesAsync();

            productImageDto.Id = productImage.Id; // Assuming ID is auto-generated and now populated
            return productImageDto;
        }

        public async Task<IEnumerable<ProductImageDto>> GetByProductIdAsync(int productId)
        {
            var images = await _productImageRepository.GetByProductIdAsync(productId);

            // Filtering out images where IsDeleted is true
            var filteredImages = images.Where(img => !img.IsDeleted)
                                       .Select(img => new ProductImageDto
                                       {
                                           Id = img.Id,
                                           Path = img.Path,
                                           ProductID = img.ProductID
                                       }).ToList();

            return filteredImages;
        }


        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var productImage = await _productImageRepository.GetByIdAsync(imageId);
            if (productImage == null)
            {
                return false;
            }

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", productImage.Path.TrimStart('/'));
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            await _productImageRepository.DeleteAsync(productImage);
            await _productImageRepository.SaveChangesAsync();

            return true;
        }



        public async Task<bool> DeleteAsync(int productImageId)
        {
            if (productImageId == 0)
            {
                return false;
            }

            var productImage = await _productImageRepository.GetByIdAsync(productImageId);

            if (productImage == null || productImage.IsDeleted)
            {
                return false;
            }

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", productImage.Path.TrimStart('/'));
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            // Soft delete the image
            await _productImageRepository.SoftDeleteAsync(productImageId);

            // Now, remove the image reference from the Product's collection of images, if applicable
            var product = await productReposatory.GetByIdAsync(productImage.ProductID); // Make sure this is the correct method name
            if (product != null && product.ProductImages != null)
            {
                // Find the image in the collection
                var imageToRemove = product.ProductImages.FirstOrDefault(pi => pi.Id == productImageId);
                if (imageToRemove != null)
                {
                    // Remove the image from the collection
                    product.ProductImages.Remove(imageToRemove);
                    // Update the product
                    await productReposatory.UpdateAsync(product); // Assuming an UpdateAsync method exists
                }
            }

            return true;
        }
        public async Task<ProductImageDto> GetByIdAsync(int imageId)
        {
            var productImage = await _productImageRepository.GetByIdAsync(imageId);
            if (productImage == null)
            {
                return null;
            }

            return new ProductImageDto
            {
                Id = productImage.Id,
                Path = productImage.Path,
                ProductID = productImage.ProductID
            };
        }
    }
}
