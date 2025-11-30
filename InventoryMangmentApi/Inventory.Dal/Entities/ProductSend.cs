using Inventory.Dal.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.Entities
{
    public class ProductSend
    {
        public int Id { get; set; }
        public DateTime SendDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";

        public int ExternalId { get; set; }
        public External External { get; set; }
   
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<ProductSendDetail> Details { get; set; }
    }

}
