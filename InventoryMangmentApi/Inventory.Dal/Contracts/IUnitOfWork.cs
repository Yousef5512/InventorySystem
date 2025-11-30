using Inventory.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.Contracts
{
    public interface IUnitOfWork: IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Order> Orders { get; }
        IRepository<Supplier> Suppliers { get; }
        IRepository<ProductSend> ProductSends { get; }
        IRepository<ProductSendDetail> ProductSendDetails { get; }
       IRepository<External> Externals { get; }

        Task<int> CompleteAsync();
        
    }
}
