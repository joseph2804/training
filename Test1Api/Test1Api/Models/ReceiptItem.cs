using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models
{
    [Table("Receipt_Items")]
    public class ReceiptItem
    {
        [Key]
        public long ReceiptItem_ID { get; set; }
        public long Receipt_ID { get; set; }
        public long Product_ID { get; set; }
        [Column("ProductQuantity")]
        public int Quantity { get; set; }
        [Column("ProductPrice")]
        public decimal Price { get; set; }
        [ForeignKey("Receipt_ID")]
        public virtual Receipt Receipt { get; set; }
    }
}
