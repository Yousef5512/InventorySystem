using Inventory.Bll.Contract;
using Inventory.Bll.DTOs;
using Inventory.Dal.Contracts;
using Inventory.Dal.DataContext;
using Inventory.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Bll.services
{
    public class ProductServices : IProductServices
    {
        private readonly IUnitOfWork _uow;
        private readonly ApplicationDbContext _context;

        public ProductServices(IUnitOfWork uow, ApplicationDbContext context)
        {
            _uow = uow;
            _context = context;
        }

        public async Task<Product> GetAsync(int id) => await _uow.Products.GetAsync(id);

        public async Task<IEnumerable<Product>> GetAllAsync() => await _uow.Products.GetAllAsync();

        public async Task AssignProductIdCodeAsync(int productId, string code)
        {
            var p = await _uow.Products.GetAsync(productId);
            if (p == null) throw new ArgumentException("Product not found");
            p.ProductIdCode = code;
            _uow.Products.Update(p);
            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<Product>> GetNearExpirationAsync(int daysThreshold)
        {
            var all = await _uow.Products.GetAllAsync();
            var now = DateTime.UtcNow;
            return all.Where(x => x.ExpirationDate.HasValue && (x.ExpirationDate.Value - now).TotalDays <= daysThreshold);
        }
        public async Task<Product> CreateProductAsync(ProductCreateDto dto)
        {
            string code = $"{dto.Material}-{dto.Size}-{dto.Type}-{dto.Flavour}-{Guid.NewGuid().ToString()[..4]}";

            var product = new Product
            {
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                Material = dto.Material,
                Size = dto.Size,
                Flavour = dto.Flavour,
                Type = dto.Type,
                ExpirationDate = dto.ExpirationDate,
                ProductIdCode = code
            };

            await _uow.Products.AddAsync(product);
            await _uow.CompleteAsync();

            return product;
        }
        public async Task UpdateAsync(Product p)
        {
            _uow.Products.Update(p);
            await _uow.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _uow.Products.GetAsync(id);
            if (product != null)
            {
                _uow.Products.Remove(product); 
                await _uow.CompleteAsync();
            }
        }
        public async Task<ProductSend> SendProductsToInventoryAsync(List<ProductSendDetailDto> products)
        {
      
            var external = await _uow.Externals.GetFirstOrDefaultAsync();
            if (external == null)
                throw new InvalidOperationException("No external company found.");

            
            var send = new ProductSend
            {
                Status = "Pending",
                ExternalId = external.Id,
                External = external,
                Details = new List<ProductSendDetail>()
            };

            foreach (var dto in products)
            {
                var product = await _uow.Products.GetAsync(dto.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product {dto.ProductId} not found");

                // Add details
                send.Details.Add(new ProductSendDetail
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    Available = dto.Available,
                    Received = dto.Received
                });
            }

            // Save to database
            await _uow.ProductSends.AddAsync(send);
            await _uow.CompleteAsync();

            return send;
        }

        public async Task<ProductSend?> GetProductSendAsync(int sendId)
        {
            var productSend = await _context.ProductSends
                .Where(ps => ps.Id == sendId)             
                .Include(ps => ps.Details)                 
                    .ThenInclude(d => d.Product)          
                .FirstOrDefaultAsync();

            return productSend;
        }



        public async Task UpdateProductSendAsync(ProductSend send)
        {
            _uow.ProductSends.Update(send);
            await _uow.CompleteAsync();
        }
        public async Task<ProductSendDto> SendToExternalAsync(List<ProductSendDetailDto> detailsDto)
        {
            // Get the external company (assuming only one)
            var external = await _context.Externals.FirstOrDefaultAsync();
            if (external == null)
            {
                external = new External { Name = "MyCompany" };
                _context.Externals.Add(external);
                await _context.SaveChangesAsync();
            }

            var productSend = new ProductSend
            {
                ExternalId = external.Id,
                External = external,
                Status = "Pending",
                Details = new List<ProductSendDetail>() // ✅ Initialize the Details list
            };

            foreach (var item in detailsDto)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null) continue; // skip if product doesn't exist

                bool available = product.Quantity >= item.Quantity;
                if (available)
                {
                    product.Quantity -= item.Quantity; // update quantity in DB
                }

                productSend.Details.Add(new ProductSendDetail
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Available = available,
                    Received = false
                });
            }

            _context.ProductSends.Add(productSend);
            await _context.SaveChangesAsync();

            return await MapToDto(productSend);
        }


        public async Task<List<ProductSendDto>> GetAllSendsAsync()
        {
            var sends = await _context.ProductSends
                .Include(s => s.Details)
                .ThenInclude(d => d.Product)
                .Include(s => s.External)
                .ToListAsync();

            var result = new List<ProductSendDto>();
            foreach (var send in sends)
            {
                result.Add(await MapToDto(send));
            }

            return result;
        }

        public async Task<ProductSendDto> MapToDto(ProductSend send)
        {
            var detailsDtoTasks = send.Details.Select(async d => new ProductSendDetailDto
            {
                ProductId = d.ProductId,
                ProductName = (await _context.Products.FindAsync(d.ProductId))?.ProductName,
                Quantity = d.Quantity,
                Available = d.Available,
                Received = d.Received
            }).ToList();

            
            var detailsDto = await Task.WhenAll(detailsDtoTasks);

            var dto = new ProductSendDto
            {
                Id = send.Id,
                SendDate = send.SendDate,
                Status = send.Status,
                Details = detailsDto.ToList() 
            };

            return dto;
        }

        public async Task<ProductSendDto> SendOrderToSupplierAsync(CreateOrderDto dto, string supplierId)
        {
            // 1️⃣ Ensure there is a valid External
            var external = await _context.Externals.FirstOrDefaultAsync();
            if (external == null)
            {
                external = new External { Name = "Inventory External" };
                _context.Externals.Add(external);
                await _context.SaveChangesAsync();
            }

            // 2️⃣ Create the ProductSend using that External
            var productSend = new ProductSend
            {
                ExternalId = external.Id,
                External = external,
                Status = "Pending",
                Details = new List<ProductSendDetail>()
            };

            // 3️⃣ Add details from the DTO
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                throw new ArgumentException($"Product {dto.ProductId} not found");

            productSend.Details.Add(new ProductSendDetail
            {
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Available = product.Quantity >= dto.Quantity,
                Received = false
            });

            // 4️⃣ Save to DB
            _context.ProductSends.Add(productSend);
            await _context.SaveChangesAsync();

            // 5️⃣ Map to DTO and return
            return await MapToDto(productSend);
        }
        public async Task<ProductSendDto> SendToReceivingClerkAsync(int productSendId)
        {
            // Fetch the ProductSend including details and external
            var send = await _context.ProductSends
                .Include(x => x.Details)
          
                .FirstOrDefaultAsync(x => x.Id == productSendId);

            if (send == null)
                return null;

            // Update status
            send.Status = "SentToReceivingClerk";

            await _context.SaveChangesAsync();

            return await MapToDto(send);
        }

        public async Task<ProductSendDto> ReceiveBySupplierAsync(int id)
        {
            var send = await _context.ProductSends
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (send == null)
                return null;

            send.Status = "ReceivedBySupplier";
            await _context.SaveChangesAsync();

            return await MapToDto(send);
        }


        public async Task<ProductSendDto> ReceiveByReceivingClerkAsync(int sendId)
        {
            var send = await _context.ProductSends
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == sendId);

            if (send == null)
                return null;

            send.Status = "ReceivedByClerk";

            await _context.SaveChangesAsync();

            return await MapToDto(send);
        }
       
        public async Task ReceiveProductsAsync(List<ReceivingCheckDto> receivedList)
        {
            foreach (var item in receivedList)
            {
                if (item.IsDamaged)
                    continue; // skip damaged products

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
        }



    }
}

