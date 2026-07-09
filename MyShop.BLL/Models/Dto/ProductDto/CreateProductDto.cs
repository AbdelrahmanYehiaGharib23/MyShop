using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyShop.BLL.Models.Dto.ProductDto
{
    public class CreateProductDto
    {

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Product name must be between 3 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000000,
            ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [StringLength(500,
            ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }
    }
}
