using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Entities
{
    public class Review:BaseEntity
    {
        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;
    }
}
