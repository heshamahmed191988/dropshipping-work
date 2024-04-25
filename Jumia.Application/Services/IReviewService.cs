using Jumia.Dtos.ViewModel.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public interface IReviewService
    {
        Task<ReviewUserDTO> AddReviewAsync(ReviewUserDTO reviewDto);
        Task<List<ReviewAdminDTO>> GetAllReviewsAsync();
        Task<ReviewUserDTO> GetReviewByIdAsync(int id);
        Task<List<ReviewUserDTO>> GetReviewsByProductIdAsync(int productId);

        Task<bool> DeleteReviewAsync(int id);
    }

}
