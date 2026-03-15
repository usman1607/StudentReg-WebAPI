using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CreatedBy { get; set; } = default!;
        public string? UpdatedBy { get; set;}
        public DateTime CreatedDate { get; set;} = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set;}
        public bool IsDeleted { get; set; } = false;
    }
}
