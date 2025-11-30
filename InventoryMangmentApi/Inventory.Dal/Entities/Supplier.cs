using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = "MySupplier"; // only one supplier
        public List<ProductSend> ProductSends { get; set; } = new();
        public string SupplierType { get; set; } 
    }
}
