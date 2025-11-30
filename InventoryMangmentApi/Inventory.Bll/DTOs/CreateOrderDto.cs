using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.DTOs
{
    public class CreateOrderDto
    {
        public string OrderCode { get; set; }
        public string OrderType { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
     
    }
}
