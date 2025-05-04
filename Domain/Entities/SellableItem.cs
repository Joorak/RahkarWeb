using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class SellableItem : BaseEntity
    {
        public string ImageUrl { get; set; }
        public string ClickTarget { get; set; }
        public string TemplateType { get; set; } // "ImageOnly", "ImageWithText", "Product"
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Text { get; set; }
        public long OriginalPrice { get; set; }
        public long DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; }
    }
}