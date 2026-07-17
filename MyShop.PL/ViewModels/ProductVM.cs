using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class ProductVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Product name must be between 3 and 100 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 1000000,
        ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000,
        ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = null!;

    public IFormFile? Image { get; set; }

    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Please select a category.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "Please select a valid category.")]
    public int CategoryId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> CategoryList { get; set; } = Enumerable.Empty<SelectListItem>();
}