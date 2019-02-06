using Cart.MVC.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cart.MVC.Controllers
{
    public class CartController : Controller
    {
        OrderController orderController = new OrderController();
        ProductController productController = new ProductController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Cart
        public ActionResult Index()
        {
            IEnumerable<Product> allProducts = null;
            IEnumerable<OrderHeader> myOrders = null;
            IEnumerable<OrderDetail> orderDetails = null;
            IEnumerable<CartProducts> myCartProducts = null;
            string currentUser = string.Empty;
            orderController.InitializeController(this.Request.RequestContext);
            productController.InitializeController(this.Request.RequestContext);

            try
            {
                myOrders = orderController.GetAllOrders().Where(x => x.Status.Equals("Draft", StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (myOrders.Count() > 0)
                {
                    //orderDetails = new OrderController().GetOrderDetails(myOrders.FirstOrDefault().Id);

                    orderDetails = orderController.GetOrderDetails(myOrders.FirstOrDefault().Id);
                    allProducts = productController.GetAllProducts();
                    myCartProducts = allProducts
                        .Join(orderDetails,
                        a => a.Id,
                        b => b.ProductId,
                        (a, b) => new CartProducts { ProductId = a.Id, ProductTitle = a.ProductTitle, ProductCategory = a.ProductCategory, ProductPrice = a.ProductPrice, OrderDetailId = b.Id, Quantity = b.Quantity, OrderHeaderId = b.OrderHeaderId })
                        .Where(ab => ab.ProductId == ab.ProductId).ToList();
                    //myCartProducts = GetOrderDetails(myOrders);
                    //int orderAmount = Convert.ToInt32(myCartProducts.Sum(x => x.ProductPrice * x.Quantity));
                    ViewBag.TotalAmount = myOrders.FirstOrDefault().TotalAmount;
                    ViewBag.OrderHeaderID = myOrders.FirstOrDefault().Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(myCartProducts);
        }

        // GET: Cart/Delete/5
        public ActionResult Delete(int id)
        {
            string currentUser = string.Empty;

            try
            {
                if (Session["CurrentUser"] != null)
                {
                    currentUser = ((UserManager)Session["CurrentUser"]).EmailID;
                    Task<string> callTask = Task.Run(() => DeleteAsync(currentUser, id));
                    callTask.Wait();
                    // Get the result
                    string astr = callTask.Result;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return RedirectToAction("Index");
            //return View();
        }

        public ActionResult CheckOut()
        {
            return View();
        }

        //public ActionResult ConfirmOrder(string shippingAddress, string paymentMode)
        [HttpPost]
        public ActionResult ConfirmOrder(string shippingAddress, string paymentMethod)
        {
            string status = string.Empty;
            string currentUser = string.Empty;
            ConfirmOrder confirmOrder = new ConfirmOrder();
            string orderStatus = string.Empty;
            try
            {
                if (Session["CurrentUser"] != null)
                {
                    currentUser = ((UserManager)Session["CurrentUser"]).EmailID;
                    confirmOrder.shippingAddress = shippingAddress;
                    confirmOrder.paymentMethod = paymentMethod;
                    confirmOrder.userId = currentUser;
                }
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(confirmOrder);

                string apiUrl = ConfigurationManager.AppSettings["HCLCartServiceAPI"];
                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                string json = client.UploadString(apiUrl + "Cart/ConfirmOrder", data);
                if (json == "true")
                {
                    orderStatus = "Order Placed";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("Index", "Product");
            //return View();
        }

        public IEnumerable<CartProducts> GetOrderDetails(List<OrderDetail> cartOrders)
        {
            IEnumerable<Product> allProducts = null;
            IEnumerable<CartProducts> cartProducts = new List<CartProducts>();
            productController.InitializeController(this.Request.RequestContext);

            try
            {
                allProducts = productController.GetAllProducts();

                var test = from a in allProducts
                           join b in cartOrders
                           on a.Id equals b.ProductId
                           select new { a.ProductTitle, a.ProductCategory, a.ProductPrice, b.Quantity };

                cartProducts = allProducts
                    .Join(cartOrders,
                    a => a.Id,
                    b => b.ProductId,
                    (a, b) => new CartProducts { ProductId = a.Id, ProductTitle = a.ProductTitle, ProductCategory = a.ProductCategory, ProductPrice = a.ProductPrice, OrderDetailId = b.Id, Quantity = b.Quantity, OrderHeaderId = b.OrderHeaderId })
                    .Where(ab => ab.ProductId == ab.ProductId).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return cartProducts;
        }



        public async Task<string> DeleteAsync(string userId, int id)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.userId = userId;
            userEntity.productId = id;

            try
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(userEntity);

                string apiUrl = ConfigurationManager.AppSettings["HCLCartServiceAPI"] + "Cart/DeleteProductFromCart";
                using (var client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent(data, Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(apiUrl)
                    };
                    var a = await client.SendAsync(request);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return "true";
        }
    }
}
