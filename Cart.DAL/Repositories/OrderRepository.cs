using Cart.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cart.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ModernAppDBContext _context;

        public OrderRepository(ModernAppDBContext context)
        {
            _context = context;
        }

        public async Task<List<Products>> GetAllProducts(CancellationToken ct = default(CancellationToken))
        {
            return await Task.Run(() => _context.Products.AsNoTracking().ToListAsync(ct));
        }

        public async Task<bool> AddProductToCart(string UserId, int ProductID, CancellationToken ct = default(CancellationToken))
        {
            Products products = await GetProductByIdAsync(ProductID);
            OrderHeader orders = await GetOrderHeaderByUserId(UserId);
            if (orders != null)
            {
                List<OrderDetails> orderDetails = await GetOrderDeatilsbyOrderIdAsync(orders.Id);
                OrderDetails productOrder = orderDetails.Where(p => p.ProductId == ProductID).FirstOrDefault();
                if (productOrder != null)
                {
                    productOrder.Quantity += 1;
                    productOrder.Amount += products.ProductPrice;
                    _context.Update(productOrder);

                    orderDetails = await GetOrderDeatilsbyOrderIdAsync(orders.Id);
                    decimal? total = orderDetails.Sum(item => item.Amount);
                    orders.TotalAmount = total;
                    _context.Update(orders);

                    await _context.SaveChangesAsync(ct);
                }
                else
                {
                    await AddOrderDetailsAsync(orders.Id, products, ct);
                    orderDetails = await GetOrderDeatilsbyOrderIdAsync(orders.Id);
                    await UpdateTotalAmountOrderHeaderAsync(orders, orderDetails, products, ct);
                }
            }
            else
            {
                OrderHeader orderHeader = await AddOrderAsync(UserId, ct);
                await AddOrderDetailsAsync(orderHeader.Id, products, ct);
                List<OrderDetails> orderDetails = await GetOrderDeatilsbyOrderIdAsync(orderHeader.Id);
                await UpdateTotalAmountOrderHeaderAsync(orderHeader, orderDetails, products, ct);
            }

            return true;
        }
        public async Task<bool> DeleteProductFromCart(string UserId, int ProductID, CancellationToken ct = default(CancellationToken))
        {
            OrderHeader orders = await GetOrderHeaderByUserId(UserId);
            if (orders != null)
            {
                List<OrderDetails> orderDetails = await GetOrderDeatilsbyOrderIdAsync(orders.Id);
                var toRemove = orderDetails.Where(p => p.ProductId == ProductID).FirstOrDefault();
                var rateRemove = toRemove.Amount;
                orders.TotalAmount = orders.TotalAmount - rateRemove;
                _context.OrderDetails.Remove(toRemove);
                _context.Update(orders);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<OrderHeader>> GetUserOrders(string UserId, CancellationToken ct = default(CancellationToken))
        {            
            List<OrderHeader> orders = await GetAllOrderHeaderByUserId(UserId);
            return orders;
        }

        public async Task<List<OrderDetails>> GetOrderDetails(int OrderId, CancellationToken ct = default(CancellationToken))
        {
            List<OrderDetails> orderDetails = await OrderDeatilsbyOrderIdAsync(OrderId);

            return orderDetails;
        }

        public async Task<List<Products>> GetProductsByKey(string value, CancellationToken ct = default(CancellationToken))
        {
            List<Products> products = null;

            await Task.Run(() =>
            {
                products = _context.Products.AsNoTracking().Where(p => p.ProductCategory.Contains(value) || p.ProductTitle.Contains(value)).ToList<Products>();
            });
            return products;
        }
        public async Task<bool> ConfirmOrder(string userId, string address, string paymentOption, CancellationToken ct = default(CancellationToken))
        {
            OrderHeader orders = await GetOrderHeaderByUserId(userId);
            if (orders != null)
            {
                orders.ShippingAddress = address;
                orders.PaymentMethod = paymentOption;
                orders.Status = "Ordered";
                orders.OrderDate = DateTime.Now.Date;
                _context.Update(orders);
                await _context.SaveChangesAsync(ct);

                List<OrderDetails> orderDetails = await OrderDeatilsbyOrderIdAsync(orders.Id);
                orderDetails.ForEach(c => c.Status = "Ordered");
                _context.UpdateRange(orderDetails);
                await _context.SaveChangesAsync(ct);

                return true;
            }
            else
            {
                return false;
            }
        }

        #region private members
        private async Task<Products> GetProductByIdAsync(int id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }
        private async Task<OrderHeader> GetOrderHeaderByUserId(string userid)
        {
            return await _context.OrderHeader.AsNoTracking().FirstOrDefaultAsync(t => t.OrderByUserId == userid && t.Status == "Draft");
        }
        private async Task<List<OrderHeader>> GetAllOrderHeaderByUserId(string userid)
        {
            return await _context.OrderHeader.AsNoTracking().Where(t => t.OrderByUserId == userid).ToListAsync();
        }

        private async Task<List<OrderDetails>> GetOrderDeatilsbyOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails.AsNoTracking().Where(t => t.OrderHeaderId == orderId && t.Status == "Draft").ToListAsync();
        }

        private async Task<List<OrderDetails>> OrderDeatilsbyOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails.AsNoTracking().Where(t => t.OrderHeaderId == orderId).ToListAsync();
        }
        private async Task UpdateTotalAmountOrderHeaderAsync(OrderHeader order, List<OrderDetails> orderDetails, Products products, CancellationToken ct)
        {
            decimal? total = 0;
            decimal? finaltotal = 0;
            total = orderDetails.Sum(item => item.Amount);
            finaltotal = total;
            order.TotalAmount = finaltotal;
            _context.Update(order);
            await _context.SaveChangesAsync(ct);
        }

        private async Task<OrderHeader> AddOrderAsync(string userId, CancellationToken ct)
        {
            OrderHeader orderHeasder = new OrderHeader()
            {
                OrderByUserId = userId,
                OrderByUserName = userId,
                OrderDate = DateTime.Now.Date,
                Status = "Draft"
            };
            _context.OrderHeader.Add(orderHeasder);
            await _context.SaveChangesAsync(ct);
            return orderHeasder;
        }
        private async Task AddOrderDetailsAsync(int orderID, Products products, CancellationToken ct)
        {
            OrderDetails orderDetails = new OrderDetails()
            {
                OrderHeaderId = orderID,
                ProductId = products.Id,
                Quantity = 1,
                Rate = products.ProductPrice,
                Amount = products.ProductPrice,
                Status = "Draft"
            };
            _context.OrderDetails.Add(orderDetails);
            await _context.SaveChangesAsync(ct);
        }
        #endregion
    }
}
