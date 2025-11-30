using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.DTOs
{
    public class ReceivingCheckDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsDamaged { get; set; }
    }
}

