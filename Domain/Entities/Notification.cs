using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Notification:BaseEntity
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}