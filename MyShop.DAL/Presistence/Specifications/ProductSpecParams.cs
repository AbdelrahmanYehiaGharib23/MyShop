using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Presistence.Specifications
{
    public class ProductSpecParams
    {
        private const int MaxPageSize = 20;

        public string? Search { get; set; }

        public string? Sort { get; set; }

        private int pageIndex = 1;

        public int PageIndex
        {
            get => pageIndex;
            set => pageIndex = value < 1 ? 1 : value;
        }

        private int pageSize = 9;

        public int PageSize
        {
            get => pageSize;

            set
            {
                pageSize = value <= 0
                    ? 9
                    : value > MaxPageSize
                    ? MaxPageSize
                    : value;
            }
        }
    }
}
