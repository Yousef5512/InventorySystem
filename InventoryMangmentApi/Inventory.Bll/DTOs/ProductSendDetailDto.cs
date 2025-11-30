using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.DTOs
{
    public class ProductSendDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public bool Available { get; set; }
        public bool Received { get; set; }
    }
}
