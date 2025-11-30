namespace InventoryMangmentMvc.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }

    public class UserInfoViewModel
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; } = new List<string>(); // Direct list
    }
    public class ProductViewModel
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

    public class ProductSendViewModel
    {
        public int Id { get; set; }
        public DateTime SendDate { get; set; }
        public string Status { get; set; }
        public List<ProductSendDetailViewModel> Details { get; set; }
    }

    public class ProductSendDetailViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public bool Available { get; set; }
        public bool Received { get; set; }
    }


    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class ReceivingCheckViewModel
    {

        public int ProductId { get; set; }
        public bool IsReceived { get; set; }
        public int ReceivedQuantity { get; set; }
    }
    public class ReceivingCheckApiDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsDamaged { get; set; }
    }


    public class LoginApiResponse
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string Message { get; set; }
        public bool Success => !string.IsNullOrEmpty(UserName);
    }

    public class RegisterApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class RolesWrapper
    {
        public List<string> Values { get; set; } // maps to $values
    }

    public class ProductCreateViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Material { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Flavour { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public class AssignRoleViewModel
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }

    public class SupplierOrderViewModel
    {
        public string SupplierId { get; set; }
        public string OrderCode { get; set; }  // REQUIRED by API
        public string OrderType { get; set; }  // REQUIRED by API
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }


    public class CreateOrderViewModel
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // REQUIRED by API
        public string OrderCode { get; set; }
        public string OrderType { get; set; }

        // Single product fields (matching your API's CreateOrderDto)
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}