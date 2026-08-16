using System.ComponentModel.DataAnnotations;


namespace MyShop.PL.ViewModels
{
    public class CategoryVM
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
