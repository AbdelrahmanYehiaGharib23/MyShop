using MyShop.BLL.Models.Dto.ProductDto;
using MyShop.BLL.Services.CurrentUserServices;
using MyShop.DAL.Contracts.UnitOfWork;
using MyShop.DAL.Entities;
using MyShop.DAL.Presistence.Specifications;

namespace MyShop.BLL.Services.FavoriteServices;

public sealed class FavoriteService : IFavoriteService
{
    private const int MaximumFavoritesToSync = 100;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public FavoriteService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<ProductDto>> GetCurrentFavoritesAsync()
    {
        var userId = GetUserId();
        var favorites = await _unitOfWork.FavoriteRepository
            .GetAllWithSpecAsync(new UserFavoritesSpecification(userId));

        return favorites
            .Select(favorite => ToProductDto(favorite.Product))
            .ToList();
    }

    public async Task AddAsync(int productId)
    {
        var userId = GetUserId();
        var product = await _unitOfWork.ProductRepository
            .GetEntityWithSpecAsync(new ProductByIdSpecification(productId));

        if (product is null)
            throw new KeyNotFoundException("Product not found.");

        var existing = await _unitOfWork.FavoriteRepository
            .FindAsync(favorite => favorite.UserId == userId && favorite.ProductId == productId);

        if (existing.Any())
            return;

        _unitOfWork.FavoriteRepository.Add(new FavoriteProduct
        {
            UserId = userId,
            ProductId = productId
        });

        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveAsync(int productId)
    {
        var userId = GetUserId();
        var existing = await _unitOfWork.FavoriteRepository
            .FindAsync(favorite => favorite.UserId == userId && favorite.ProductId == productId);

        var favorite = existing.FirstOrDefault();
        if (favorite is null)
            return;

        _unitOfWork.FavoriteRepository.Remove(favorite);
        await _unitOfWork.CompleteAsync();
    }

    public async Task SyncAsync(IEnumerable<int> productIds)
    {
        var userId = GetUserId();
        var ids = productIds
            .Where(id => id > 0)
            .Distinct()
            .Take(MaximumFavoritesToSync)
            .ToArray();

        if (ids.Length == 0)
            return;

        var existingFavorites = await _unitOfWork.FavoriteRepository
            .FindAsync(favorite => favorite.UserId == userId && ids.Contains(favorite.ProductId));
        var existingIds = existingFavorites.Select(favorite => favorite.ProductId).ToHashSet();
        var products = await _unitOfWork.ProductRepository
            .FindAsync(product => ids.Contains(product.Id));

        foreach (var product in products)
        {
            if (existingIds.Contains(product.Id))
                continue;

            _unitOfWork.FavoriteRepository.Add(new FavoriteProduct
            {
                UserId = userId,
                ProductId = product.Id
            });
        }

        if (products.Any(product => !existingIds.Contains(product.Id)))
            await _unitOfWork.CompleteAsync();
    }

    private string GetUserId()
    {
        return _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }

    private static ProductDto ToProductDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            ImageUrl = product.ImageUrl
        };
    }
}
