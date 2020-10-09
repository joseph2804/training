using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models
{
    [Table("Receipts")]
    public class Receipt
    {
        
        [Key]
        public long Receipt_ID { get; set; }
        public long Customer_ID { get; set; }
        public DateTime ReceiptDate { get; set; }
        public virtual ICollection<ReceiptItem> ReceiptItems { get; set; }
    }
}
