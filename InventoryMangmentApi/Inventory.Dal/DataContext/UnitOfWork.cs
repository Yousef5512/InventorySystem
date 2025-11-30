using Inventory.Dal.Contracts;
using Inventory.Dal.Entities;
using Inventory.Dal.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal.DataContext
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _db;

        private readonly Lazy<IRepository<Product>> _products;
        private readonly Lazy<IRepository<Order>> _orders;
        private readonly Lazy<IRepository<Supplier>> _suppliers;
        private readonly Lazy<IRepository<ProductSend>> _productSends;
        private readonly Lazy<IRepository<ProductSendDetail>> _productSendDetails;
        private readonly Lazy<IRepository<External>> _externals;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            _products = new Lazy<IRepository<Product>>(() => new Repository<Product>(_db));
            _orders = new Lazy<IRepository<Order>>(() => new Repository<Order>(_db));
            _suppliers = new Lazy<IRepository<Supplier>>(() => new Repository<Supplier>(_db));
            _productSends = new Lazy<IRepository<ProductSend>>(() => new Repository<ProductSend>(_db));
            _productSendDetails = new Lazy<IRepository<ProductSendDetail>>(() => new Repository<ProductSendDetail>(_db));
            _externals = new Lazy<IRepository<External>>(() => new Repository<External>(_db));
        }

        public IRepository<Product> Products => _products.Value;
        public IRepository<Order> Orders => _orders.Value;
        public IRepository<Supplier> Suppliers => _suppliers.Value;
        public IRepository<ProductSend> ProductSends => _productSends.Value;
        public IRepository<ProductSendDetail> ProductSendDetails => _productSendDetails.Value;
        public IRepository<External> Externals => _externals.Value;


        public async Task<int> CompleteAsync() => await _db.SaveChangesAsync();

        public void Dispose() => _db.Dispose();
    }
}
