using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StudentsCourses: BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public Student Student { get; set; } = default!;
        public Course Course { get; set; } = default!;
    }
}
