using Domain.Entities;

namespace Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public string RequestNumber { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}