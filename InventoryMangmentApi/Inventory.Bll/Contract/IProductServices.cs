using Inventory.Bll.DTOs;
using Inventory.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.Contract
{
    public interface IProductServices
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetAsync(int id);
        Task<Product> CreateProductAsync(ProductCreateDto dto);
        Task AssignProductIdCodeAsync(int productId, string code);
        Task<IEnumerable<Product>> GetNearExpirationAsync(int daysThreshold);
        Task UpdateAsync(Product p);
        Task DeleteAsync(int id);
        Task<ProductSend> SendProductsToInventoryAsync(List<ProductSendDetailDto> products);
        Task<ProductSend> GetProductSendAsync(int sendId);
        Task UpdateProductSendAsync(ProductSend send);

        Task<ProductSendDto> SendToExternalAsync(List<ProductSendDetailDto> detailsDto);
        Task<List<ProductSendDto>> GetAllSendsAsync();
        Task<ProductSendDto> SendOrderToSupplierAsync(CreateOrderDto dto, string supplierId);
        Task<ProductSendDto> ReceiveBySupplierAsync(int productSendId);
        Task<ProductSendDto> SendToReceivingClerkAsync(int productSendId);
        Task<ProductSendDto> ReceiveByReceivingClerkAsync(int id);
        Task ReceiveProductsAsync(List<ReceivingCheckDto> receivedList);
    }
}
