using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int RequestedQuantity { get; set; }
        public string Status { get; set; }
    }
}
