using MyShop.BLL.Models.Dto.ReviewDto;
using MyShop.BLL.Services.CurrentUserServices;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;
using MyShop.DAL.Presistence.Specifications;

namespace MyShop.BLL.Services.ReviewServices
{
    public class ReviewService:IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ReviewService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<ReviewDto> AddReviewAsync(CreateReviewDto dto)
        {
            var userId = _currentUserService.GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            var existingReview =
                await _unitOfWork.ReviewRepository
                    .GetEntityWithSpecAsync(
                        new ReviewSpecification(
                            userId,
                            dto.ProductId));

            if (existingReview is not null)
                throw new InvalidOperationException(
                    "You have already reviewed this product.");

            var review = new Review
            {
                ProductId = dto.ProductId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _unitOfWork.ReviewRepository.Add(review);

            await _unitOfWork.CompleteAsync();

            return new ReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedOn = review.CreatedOn
            };
        }

        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(
            int productId)
        {
            var reviews =
                await _unitOfWork.ReviewRepository
                    .GetAllWithSpecAsync(
                        new ProductReviewsSpecification(productId));

            return reviews.Select(review => new ReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedOn = review.CreatedOn
            });
        }

        public async Task UpdateReviewAsync(
            int reviewId,
            CreateReviewDto dto)
        {
            var userId = _currentUserService.GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            var review =
                await _unitOfWork.ReviewRepository
                    .GetByIdAsync(reviewId);

            if (review is null || review.IsDeleted)
                throw new KeyNotFoundException(
                    "Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You cannot edit this review.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            _unitOfWork.ReviewRepository.Update(review);

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var userId = _currentUserService.GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            var review =
                await _unitOfWork.ReviewRepository
                    .GetByIdAsync(reviewId);

            if (review is null || review.IsDeleted)
                throw new KeyNotFoundException(
                    "Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You cannot delete this review.");

            _unitOfWork.ReviewRepository.Remove(review);

            await _unitOfWork.CompleteAsync();
        }
        public async Task<ReviewSummaryDto> GetReviewSummaryAsync(int productId)
        {
            var spec = new ProductReviewsSpecification(productId);

            var reviews = await _unitOfWork.ReviewRepository
                .GetAllWithSpecAsync(spec);

            var reviewList = reviews.ToList();

            if (reviewList.Count == 0)
            {
                return new ReviewSummaryDto
                {
                    AverageRating = 0,
                    ReviewCount = 0
                };
            }

            return new ReviewSummaryDto
            {
                AverageRating = reviewList.Average(r => r.Rating),
                ReviewCount = reviewList.Count
            };
        }

        public async Task<IReadOnlyDictionary<int, ReviewSummaryDto>> GetReviewSummariesAsync(IEnumerable<int> productIds)
        {
            var ids = productIds.Where(id => id > 0).Distinct().ToArray();
            if (ids.Length == 0)
                return new Dictionary<int, ReviewSummaryDto>();

            var reviews = await _unitOfWork.ReviewRepository
                .GetAllWithSpecAsync(new ProductReviewsForProductsSpecification(ids));

            return reviews
                .GroupBy(review => review.ProductId)
                .ToDictionary(
                    group => group.Key,
                    group => new ReviewSummaryDto
                    {
                        AverageRating = group.Average(review => review.Rating),
                        ReviewCount = group.Count()
                    });
        }
    }
}

