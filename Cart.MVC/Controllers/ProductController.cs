using Cart.MVC.Models;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cart.MVC.Controllers
{
    public class ProductController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void InitializeController(RequestContext context)
        {
            base.Initialize(context);
        }
        // GET: Product
        public ActionResult Index()
        {
            IEnumerable<Product> allProducts = null;
            allProducts = GetAllProducts();
            
            return View(allProducts);
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchProduct(string SearchString)
        {
            IEnumerable<Product> products = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["HCLCartServiceAPI"]);
                    //HTTP GET
                    var responseTask = client.GetAsync("Product/GetProductsByKey/" + SearchString);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<Product>>();
                        readTask.Wait();
                        products = readTask.Result;
                        log.Info(products.Count()+  " Products searched on key: " + SearchString);                        
                    }
                    else
                    {
                        //log.Info("");
                        products = Enumerable.Empty<Product>();
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }                
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return View("Index", products);            
        }

        public ActionResult AddToCart(int productId)
        {
            string status = string.Empty;
            string currentUser = string.Empty;
            string message = string.Empty;
            UserEntity userEntity = new UserEntity();

            try
            {
                if (Session["CurrentUser"] != null)
                {
                    currentUser = ((UserManager)Session["CurrentUser"]).EmailID;
                    userEntity.userId = currentUser;
                    userEntity.productId = productId;
                }
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(userEntity);


                string apiUrl = ConfigurationManager.AppSettings["HCLCartServiceAPI"];
                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;
                string json = client.UploadString(apiUrl + "Cart/AddProductToCart", data);
                
                if (json == "true")
                {
                    message = "Product added to cart";
                    log.Info("Product added to cart.");
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("Index", "Product", message);

            //return status;
            //return View();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            IEnumerable<Product> allProducts = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["HCLCartServiceAPI"]);                    
                    var responseTask = client.GetAsync("Product/GetAllProducts");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<Product>>();
                        readTask.Wait();

                        allProducts = readTask.Result.ToList();
                    }
                    else 
                    {
                        allProducts = Enumerable.Empty<Product>().ToList();
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return allProducts;
        }
    }
}
