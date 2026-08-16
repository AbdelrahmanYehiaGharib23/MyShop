using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.BLL.Models.Dto.ReviewDto;
using MyShop.BLL.Services.ReviewServices;

namespace MyShop.PL.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _reviewService
                .GetProductReviewsAsync(productId);

            return Ok(reviews);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviewSummary(int productId)
        {
            var summary = await _reviewService
                .GetReviewSummaryAsync(productId);

            return Ok(summary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var review = await _reviewService
                    .AddReviewAsync(dto);

                return Ok(review);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int reviewId,
            CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _reviewService
                    .UpdateReviewAsync(reviewId, dto);

                return Ok(new
                {
                    message = "Review updated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] int reviewId)
        {
            if (reviewId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid review id."
                });
            }

            try
            {
                await _reviewService
                    .DeleteReviewAsync(reviewId);

                return Ok(new
                {
                    message = "Review deleted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
