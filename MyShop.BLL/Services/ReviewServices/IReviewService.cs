using System;
using System.Collections.Generic;
using System.Text;
using MyShop.BLL.Models.Dto.ReviewDto;

namespace MyShop.BLL.Services.ReviewServices
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(CreateReviewDto dto);

        Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId);

        Task UpdateReviewAsync(int reviewId, CreateReviewDto dto);

        Task DeleteReviewAsync(int reviewId);
        Task<ReviewSummaryDto> GetReviewSummaryAsync(int productId);

    }
}
