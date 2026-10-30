using Moq;
using MyShop.BLL.Models.Dto.ReviewDto;
using MyShop.BLL.Services.CurrentUserServices;
using MyShop.BLL.Services.ReviewServices;
using MyShop.DAL.Contracts.Repositories;
using MyShop.DAL.Contracts.Specifications;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;

namespace MyShop.UnitTests.Services;

[Trait("Category", "Unit")]
public sealed class ReviewServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IReviewRepository> _reviewRepository = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();

    [Fact]
    public async Task GetReviewSummaryAsync_ReturnsZeroesWhenProductHasNoReviews()
    {
        _reviewRepository
            .Setup(repository => repository.GetAllWithSpecAsync(
                It.IsAny<ISpecification<Review>>()))
            .ReturnsAsync(Array.Empty<Review>());

        var result = await CreateService().GetReviewSummaryAsync(productId: 42);

        Assert.Equal(0, result.AverageRating);
        Assert.Equal(0, result.ReviewCount);
    }

    [Fact]
    public async Task GetReviewSummaryAsync_ReturnsAverageAndCountForProductReviews()
    {
        Review[] reviews =
        [
            new() { ProductId = 42, Rating = 5 },
            new() { ProductId = 42, Rating = 4 },
            new() { ProductId = 42, Rating = 3 }
        ];

        _reviewRepository
            .Setup(repository => repository.GetAllWithSpecAsync(
                It.IsAny<ISpecification<Review>>()))
            .ReturnsAsync(reviews);

        var result = await CreateService().GetReviewSummaryAsync(productId: 42);

        Assert.Equal(4, result.AverageRating);
        Assert.Equal(3, result.ReviewCount);
    }

    [Fact]
    public async Task AddReviewAsync_RejectsUnauthenticatedUserBeforeRepositoryAccess()
    {
        _currentUser.Setup(service => service.GetCurrentUserId()).Returns((string?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            CreateService().AddReviewAsync(CreateReviewDto()));

        _reviewRepository.Verify(repository => repository.Add(It.IsAny<Review>()), Times.Never);
        _unitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task AddReviewAsync_RejectsSecondReviewFromSameUser()
    {
        _currentUser.Setup(service => service.GetCurrentUserId()).Returns("customer-1");
        _reviewRepository
            .Setup(repository => repository.GetEntityWithSpecAsync(
                It.IsAny<ISpecification<Review>>()))
            .ReturnsAsync(new Review());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateService().AddReviewAsync(CreateReviewDto()));

        _reviewRepository.Verify(repository => repository.Add(It.IsAny<Review>()), Times.Never);
        _unitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task AddReviewAsync_SavesReviewForAuthenticatedCustomer()
    {
        _currentUser.Setup(service => service.GetCurrentUserId()).Returns("customer-1");
        _unitOfWork
            .Setup(unitOfWork => unitOfWork.CompleteAsync())
            .ReturnsAsync(1);

        Review? addedReview = null;
        _reviewRepository
            .Setup(repository => repository.Add(It.IsAny<Review>()))
            .Callback<Review>(review => addedReview = review);

        var result = await CreateService().AddReviewAsync(CreateReviewDto());

        Assert.NotNull(addedReview);
        Assert.Equal("customer-1", addedReview.UserId);
        Assert.Equal(42, addedReview.ProductId);
        Assert.Equal(5, addedReview.Rating);
        Assert.Equal("Excellent quality", addedReview.Comment);
        Assert.Equal(42, result.ProductId);
        Assert.Equal("customer-1", result.UserId);
        _unitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync(), Times.Once);
    }

    private ReviewService CreateService()
    {
        _unitOfWork
            .SetupGet(unitOfWork => unitOfWork.ReviewRepository)
            .Returns(_reviewRepository.Object);

        return new ReviewService(_unitOfWork.Object, _currentUser.Object);
    }

    private static CreateReviewDto CreateReviewDto() => new()
    {
        ProductId = 42,
        Rating = 5,
        Comment = "Excellent quality"
    };
}
