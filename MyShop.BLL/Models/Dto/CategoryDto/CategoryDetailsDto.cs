using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.CategoryDto
{
    public class CategoryDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int ProductsCount { get; set; }
    }
}
