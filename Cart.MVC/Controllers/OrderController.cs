using Cart.MVC.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cart.MVC.Controllers
{
    public class OrderController : Controller
    {
        ProductController productController = new ProductController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void InitializeController(RequestContext context)
        {
            base.Initialize(context);
        }

        // GET: Order
        public ActionResult MyOrders()
        {
            IEnumerable<OrderHeader> allOrders = null;
            IEnumerable<OrderHeader> myOrders = null;
            string currentUser = string.Empty;

            allOrders = GetAllOrders();
            if (allOrders !=null)
            {
                myOrders = allOrders.Where(x => !x.Status.Equals("Draft", StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            return View(myOrders);
        }

        public ActionResult OrderDetails(int orderHeaderId)
        {

            IEnumerable<OrderDetail> orderItems = null;
            IEnumerable<CartProducts> orderItemsDetailed = null;
            IEnumerable<Product> allProducts = null;

            try
            {
                orderItems = GetOrderDetails(orderHeaderId);
                allProducts = productController.GetAllProducts();
                if (orderItems.Count() > 0 && allProducts.Count() > 0)
                {
                    orderItemsDetailed = allProducts
                        .Join(orderItems,
                        a => a.Id,
                        b => b.ProductId,
                        (a, b) => new CartProducts { ProductId = a.Id, ProductTitle = a.ProductTitle, ProductCategory = a.ProductCategory, ProductPrice = a.ProductPrice, OrderDetailId = b.Id, Quantity = b.Quantity, OrderHeaderId = b.OrderHeaderId })
                        .Where(ab => ab.ProductId == ab.ProductId).ToList();
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(orderItemsDetailed);
        }

        public IEnumerable<OrderHeader> GetAllOrders()
        {
            IEnumerable<OrderHeader> allOrders = null;
            string currentUser = string.Empty;

            try
            {
                if (Session["CurrentUser"] != null)
                {
                    currentUser = ((UserManager)Session["CurrentUser"]).EmailID;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["HCLCartServiceAPI"]);
                        var responseTask = client.GetAsync("Cart/GetUserOrders/" + currentUser);
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<IList<OrderHeader>>();
                            readTask.Wait();
                            allOrders = readTask.Result.ToList();
                            log.Info(allOrders.Count()+ " orders retrieved for user: " + currentUser);
                        }
                        else
                        {
                            allOrders = Enumerable.Empty<OrderHeader>().ToList();
                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return allOrders;
        }

        public IEnumerable<OrderDetail> GetOrderDetails(int orderHeaderId)
        {
            IEnumerable<OrderDetail> orderItems = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["HCLCartServiceAPI"]);
                    var responseTask = client.GetAsync("Cart/GetOrderDetails/" + orderHeaderId);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<OrderDetail>>();
                        readTask.Wait();
                        orderItems = readTask.Result.ToList();
                        log.Info(orderItems.Count() + " orders items found in OrderHeaderID: " + orderHeaderId);
                    }
                    else
                    {
                        orderItems = Enumerable.Empty<OrderDetail>().ToList();
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return orderItems;
        }
    }
}