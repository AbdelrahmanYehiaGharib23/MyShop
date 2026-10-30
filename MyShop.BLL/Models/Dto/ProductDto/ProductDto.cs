using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.ProductDto
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; } = null!;

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
