using MyShop.BLL.Models.Dto.ProductDto;

namespace MyShop.PL.ViewModels
{
    public class ShopViewModel
    {
        public string? SearchTerm { get; set; }

        public string? Sort { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 9;

        public int Count { get; set; }

        public IEnumerable<ProductDto> Products { get; set; } = [];

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(Count / (double)PageSize);

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;
    }
}
