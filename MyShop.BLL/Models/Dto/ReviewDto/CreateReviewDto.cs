using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyShop.BLL.Models.Dto.ReviewDto
{
    public class CreateReviewDto
    {
        public int ProductId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }
}
