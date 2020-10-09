using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models
{
    [Table("Todos")]
    public class TodoItem
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Required field!")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Least 6 character")]
        public string Name { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
