using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public long Price { get; set; }
        public int CategoryId { get; set; }
        public bool IsSelected { get; set; }
    }
}