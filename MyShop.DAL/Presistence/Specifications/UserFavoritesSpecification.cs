using Microsoft.EntityFrameworkCore;

namespace MyShop.DAL.Presistence.Specifications;

public sealed class UserFavoritesSpecification : BaseSpecification<FavoriteProduct>
{
    public UserFavoritesSpecification(string userId)
        : base(favorite => favorite.UserId == userId)
    {
        AddIncludeChain(query => query
            .Include(favorite => favorite.Product)
            .ThenInclude(product => product.Category));
        AddOrderByDescending(favorite => favorite.CreatedOn ?? DateTime.MinValue);
    }
}
