using Jumia.Application.Services;
using Jumia.Dtos.ViewModel.Review;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmazonWebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewUserDTO>>> GetReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync(); // Ensure this method exists
                return Ok(reviews);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<ActionResult<ReviewUserDTO>> PostReview([FromBody] ReviewUserDTO reviewDto)
        {
            try
            {
                if (reviewDto == null)
                    return BadRequest();

                var createdReview = await _reviewService.AddReviewAsync(reviewDto);

                return CreatedAtAction(nameof(GetReview), new { id = createdReview.Id }, createdReview);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the review");
            }
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewUserDTO>> GetReview(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id); // Ensure this method exists

                if (review == null)
                {
                    return NotFound();
                }

                return review;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
       [HttpGet("Product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewUserDTO>>> GetReviewsByProductId(int productId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
                if (reviews == null || reviews.Count == 0)
                {
                    return NotFound($"No reviews found for product ID {productId}.");
                }
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database: {ex.Message}");
            }
        }

    }
}
