using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.Entities
{
    public class External
    {
        public int Id { get; set; }          
        public string Name { get; set; }

        public List<ProductSend> ProductSends { get; set; } = new List<ProductSend>();
    }
}
