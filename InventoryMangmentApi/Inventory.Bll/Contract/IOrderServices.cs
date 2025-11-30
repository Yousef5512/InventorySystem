using Inventory.Bll.DTOs;
using Inventory.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.Contract
{
    public interface IOrderServices
    {
        Task<Order> CreateOrderAsync(Order o);
        Task<bool> CheckAvailabilityAsync(int productId, int requiredQty);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderAsync(int orderId);
        

    }
}
