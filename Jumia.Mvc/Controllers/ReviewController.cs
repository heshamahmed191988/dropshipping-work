using Jumia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jumia.Mvc.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: Review
        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewService.GetAllReviewsAsync(); // Ensure this method exists in your service
            return View(reviews);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _reviewService.GetReviewByIdAsync(id.Value);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _reviewService.DeleteReviewAsync(id);
            if (!success)
            {
                // Handle the case where the review could not be deleted
                // This could be due to the review not being found or another issue
                // For simplicity, we'll just redirect to the index with a failure message
                TempData["ErrorMessage"] = "The review could not be deleted.";
                return RedirectToAction(nameof(Index));
            }

            // Redirect to the index action if the delete was successful
            return RedirectToAction(nameof(Index));
        }
    }
}
