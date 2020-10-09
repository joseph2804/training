using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models
{
    [Table("Courses")]
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string Name { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
