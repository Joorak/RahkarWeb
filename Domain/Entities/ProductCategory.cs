using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool IsBusinessCategory { get; set; }
        public List<Product> Products { get; set; }
    }
}