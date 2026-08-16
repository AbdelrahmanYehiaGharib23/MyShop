using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.DAL.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; } //PK

        // ⭐⭐⭐ Soft Delete Columns ⭐⭐⭐
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Audit
        public string CreatedBy { get; set; } = string.Empty; //User Id
        public DateTime? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty; //User Id Modified
        public DateTime? LastModifiedOn { get; set; } // Automatic Calculated in (Configuration)
    }
}
