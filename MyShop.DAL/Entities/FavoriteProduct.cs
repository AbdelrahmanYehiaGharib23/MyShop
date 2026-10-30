using MyShop.DAL.Entities.IdentityEntity;

namespace MyShop.DAL.Entities;

public class FavoriteProduct : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}
