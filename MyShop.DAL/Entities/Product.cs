using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Entities
{
    public class Product:BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public virtual Category Category { get; set; } = null!;
    }
}

