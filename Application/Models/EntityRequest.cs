using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class EntityRequest<T> where T : BaseEntity
    {
        public T? Props { get; set; }
    }
}
