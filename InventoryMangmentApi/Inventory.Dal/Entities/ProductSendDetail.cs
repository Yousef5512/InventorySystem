using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.Entities
{
    public class ProductSendDetail
    {
        public int Id { get; set; }
        public int ProductSendId { get; set; }
        public ProductSend ProductSend { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public bool Received { get; set; } = false;
        public bool Available { get; set; } = false;
    }
}
