using Jumia.Application.Contract;
using Jumia.Dtos.ViewModel.Review;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserService userService;

        public ReviewService(IReviewRepository reviewRepository,IUserService userService)
        {
            _reviewRepository = reviewRepository;
            this.userService = userService;
        }

        public async Task<ReviewUserDTO> AddReviewAsync(ReviewUserDTO reviewDto)
        {
            var review = new Review
            {
                ProductID = reviewDto.ProductID.Value,
                UserID = reviewDto.UserID,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                DatePosted = DateTime.UtcNow,
            };

            await _reviewRepository.CreateAsync(review);
            await _reviewRepository.SaveChangesAsync();

            // Assuming userService has a method to get user by ID that returns an ApplicationUser or a similar user object.
            var user = await userService.GetUserByIdAsync(review.UserID);

            // Make sure to check if user is not null to avoid NullReferenceException
            if (user != null)
            {
                // Update the DTO to include UserName before returning
                reviewDto.UserName = user.UserName;
            }
            else
            {
                // Handle the case where user might not be found, possibly set UserName to "Unknown" or similar
                reviewDto.UserName = "Unknown";
            }

            return reviewDto; // Return the DTO with UserName set
        }


        public async Task<List<ReviewAdminDTO>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync(); // Assume this method exists and fetches all reviews
            var reviewDtos = reviews.Select(r => new ReviewAdminDTO
            {
                Id = r.Id,
                ProductID = r.ProductID,
                UserID = r.UserID,
                Rating = r.Rating,
                Comment = r.Comment,
                DatePosted = r.DatePosted
            }).ToList();

            return reviewDtos;
        }
        public async Task<ReviewUserDTO> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id); // Assume GetByIdAsync exists in the repository
            if (review == null)
            {
                return null; // Or handle the null case as per your application's needs
            }

            var reviewDto = new ReviewUserDTO
            {
                Id = review.Id,
                ProductID = review.ProductID,
                UserID = review.UserID,
                
                Rating = review.Rating,
                Comment = review.Comment,
                DatePosted = review.DatePosted
            };

            return reviewDto;
        }
        public async Task<List<ReviewUserDTO>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            var reviewDtos = new List<ReviewUserDTO>();

            foreach (var review in reviews)
            {
                // Assuming userService.GetUserByIdAsync() exists and fetches the user details.
                var user = await userService.GetUserByIdAsync(review.UserID);

                var reviewDto = new ReviewUserDTO
                {
                    Id = review.Id,
                    ProductID = review.ProductID,
                    UserID = review.UserID,
                    UserName = user?.UserName ?? "Unknown", // Use "Unknown" if the user isn't found or the UserName is null
                    Rating = review.Rating,
                    Comment = review.Comment,
                    DatePosted = review.DatePosted
                };

                reviewDtos.Add(reviewDto);
            }

            return reviewDtos;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                // Return false or throw an exception if the review doesn't exist
                return false;
            }

            await _reviewRepository.DeleteAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return true; // Return true to indicate the review was successfully deleted
        }
    }
}
