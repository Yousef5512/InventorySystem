using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderType { get; set; }
        public string Status { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
