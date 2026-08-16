namespace MyShop.PL.ViewModels
{
    public class DeletedProductVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string? DeletedBy { get; set; }
    }
}
