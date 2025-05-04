using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Request : BaseEntity
    {
        public string RequestNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerName { get; set; }
        public string FacilityPlan { get; set; }
        public string Status { get; set; }
    }
}