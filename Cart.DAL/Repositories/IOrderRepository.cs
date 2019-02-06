using Cart.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cart.DAL.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Products>> GetAllProducts(CancellationToken ct = default(CancellationToken));
        Task<bool> AddProductToCart(string userID, int productId, CancellationToken ct = default(CancellationToken));
        Task<bool> DeleteProductFromCart(string userID, int productId, CancellationToken ct = default(CancellationToken));
        Task<List<OrderHeader>> GetUserOrders(string userID, CancellationToken ct = default(CancellationToken));
        Task<List<OrderDetails>> GetOrderDetails(int orderId, CancellationToken ct = default(CancellationToken));
        Task<List<Products>> GetProductsByKey(string value, CancellationToken ct = default(CancellationToken));
        Task<bool> ConfirmOrder(string userID,string shippingAddress, string paymentOption, CancellationToken ct = default(CancellationToken));

    }
}
