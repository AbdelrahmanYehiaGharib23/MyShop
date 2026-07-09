using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Entities
{
    public class Category:BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
