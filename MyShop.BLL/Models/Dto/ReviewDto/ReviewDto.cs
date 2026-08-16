using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.BLL.Models.Dto.ReviewDto
{
    public class ReviewDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime? CreatedOn { get; set; }
    }
}
