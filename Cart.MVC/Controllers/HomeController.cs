using Cart.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using log4net;

namespace Cart.MVC.Controllers
{
    public class HomeController : Controller
    {
        UserManager user;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact.";

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SharePointAuthAutho objAuth)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    user = objAuth.Authorize(objAuth.UserName, objAuth.Password);

                    if (user != null && user.GroupPermission > 0)
                    {
                        Session.Add("IsOnline", user.IsOnline);

                        FormsAuthentication.SetAuthCookie(objAuth.DisplayName, objAuth.RememberMe);

                        //if (Session["ClientContext"] == null)
                        //{
                        //    Session.Add("ClientContext", objAuth.ClientContext);
                        //}
                        //else
                        //{
                        //    Session.Remove("ClientContext");
                        //    Session.Add("ClientContext", objAuth.ClientContext);
                        //}

                        if (Session["CurrentUser"] == null)
                        {
                            Session.Add("CurrentUser", user);
                            Session.Add("CurrentUserName", user.UserName);
                        }
                        else
                        {
                            Session.Remove("CurrentUser");
                            Session.Remove("CurrentUserName");
                            Session.Add("CurrentUser", user);
                            Session.Add("CurrentUserName", user.UserName);
                        }

                        if (Session["pwd"] == null)
                        {
                            Session.Add("pwd", objAuth.Password);
                        }
                        else
                        {
                            Session.Remove("pwd");
                            Session.Add("pwd", objAuth.Password);
                        }
                        return RedirectToAction("Index", "Product");
                    }

                    if (user != null && user.GroupPermission == 0)
                    {
                        ModelState.AddModelError("", "You are not Authorized to Log in");
                    }

                    if (user == null)
                    {
                        ModelState.AddModelError("", "Username and/or Password is incorrect");
                    }
                }

            }
            catch (Exception ex)
            {
                //Utilities.LogToEventVwr(ex.Message, 0);
                if (ex.Message.Contains("denied"))
                {
                    ModelState.AddModelError("", "You are not Authorized to Log in");
                }
                else
                {
                    ModelState.AddModelError("", "Username and/or Password is incorrect");
                }
            }

            return View(objAuth);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();


            // Clearing session variables
            System.Web.HttpContext.Current.Session["IsOnline"] = null;
            System.Web.HttpContext.Current.Session["CurrentUser"] = null;
            System.Web.HttpContext.Current.Session["pwd"] = null;
            System.Web.HttpContext.Current.Session["SPCredential"] = null;
            System.Web.HttpContext.Current.Session["CurrentUserName"] = null;

            return RedirectToAction("Login", "Home");
        }
    }
}