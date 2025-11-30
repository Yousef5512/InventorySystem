using Inventory.Bll.Contract;
using Inventory.Bll.DTOs;
using Inventory.Dal.Contracts;
using Inventory.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.services
{
    public class OrderServices:IOrderServices
    {
        private readonly IUnitOfWork _uow;
        public OrderServices(IUnitOfWork uow) => _uow = uow;

        public async Task<Order> CreateOrderAsync(Order o)
        {
            await _uow.Orders.AddAsync(o);
            await _uow.CompleteAsync();
            return o;
        }

        public async Task<bool> CheckAvailabilityAsync(int productId, int requiredQty)
        {
            var p = await _uow.Products.GetAsync(productId);
            if (p == null) return false;
            return p.Quantity >= requiredQty;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _uow.Orders.GetAsync(orderId);
            if (order == null) throw new System.ArgumentException("Order not found");
            order.Status = status;
            _uow.Orders.Update(order);
            await _uow.CompleteAsync();
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _uow.Orders.GetAllAsync();

        public async Task<Order> GetOrderAsync(int orderId) => await _uow.Orders.GetAsync(orderId);
      

    }
}
