using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.ProductDto
{
    public class DeletedProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string? DeletedBy { get; set; }
    }
}
