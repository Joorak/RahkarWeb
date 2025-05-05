using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CarouselItem : BaseEntity
    {
        public string ImageUrl { get; set; }
        public string ClickTarget { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ButtonText { get; set; }
        public string ButtonTarget { get; set; }
        public int DisplayDuration { get; set; } // مدت نمایش به میلی‌ثانیه
    }
}