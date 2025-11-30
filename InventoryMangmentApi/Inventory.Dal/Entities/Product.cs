using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Material { get; set; }
        public int Size { get; set; }
        public string Flavour { get; set; }
        public string Type { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ProductIdCode { get; set; }
    }
}
