

using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{

    public abstract class BaseEntity
    {
        public int Id { get; set; }

    }
}
