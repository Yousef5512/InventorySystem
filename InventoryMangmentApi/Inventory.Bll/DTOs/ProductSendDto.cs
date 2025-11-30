using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.DTOs
{
    public class ProductSendDto
    {
        public int Id { get; set; }
        public DateTime SendDate { get; set; }
        public string Status { get; set; }
        public List<ProductSendDetailDto> Details { get; set; }
    }
}
