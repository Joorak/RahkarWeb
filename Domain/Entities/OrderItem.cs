using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.oldEntities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? OrderId { get; set; } // اختیاری برای سبد خرید موقت

        [Required]
        public Guid SupplierId { get; set; }

        public Guid? ProductCategoryId { get; set; }

        [Required, MaxLength(200)]
        public string ProductName { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; } // محاسبه‌شده یا ذخیره‌شده

        public decimal Discount { get; set; } // تخفیف به درصد

        [MaxLength(200)]
        public string ImageUrl { get; set; } // آدرس تصویر محصول

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        //[ForeignKey("OrderId")]
        //public Order Order { get; set; }

        //[ForeignKey("SupplierId")]
        //public Person Supplier { get; set; }

        //[ForeignKey("ProductCategoryId")]
        //public ProductCategory ProductCategory { get; set; }
    }
}