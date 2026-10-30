using MyShop.BLL.Models.Dto.ProductDto;

namespace MyShop.BLL.Services.FavoriteServices;

public interface IFavoriteService
{
    Task<IReadOnlyList<ProductDto>> GetCurrentFavoritesAsync();
    Task AddAsync(int productId);
    Task RemoveAsync(int productId);
    Task SyncAsync(IEnumerable<int> productIds);
}
