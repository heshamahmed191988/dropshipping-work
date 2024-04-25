using Jumia.Dtos.ViewModel.Product;

namespace Jumia.Application.Services
{
    public interface IProductImageService
    {
        Task<ProductImageDto> CreateAsync(ProductImageDto productImageDto);
        Task<IEnumerable<ProductImageDto>> GetByProductIdAsync(int productId);
        Task<bool> DeleteImageAsync(int imageId);
        Task<bool> DeleteAsync(int productImageId);
        Task<ProductImageDto> GetByIdAsync(int imageId);

    }
}