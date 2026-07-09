using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyShop.BLL.Models.Dto.CategoryDto
{
    public class UpdateCategoryDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Category name must be between 3 and 100 characters.")]
        public string Name { get; set; } = null!;
    }
}
