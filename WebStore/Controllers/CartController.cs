using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        // List of current items in cart
        public ActionResult Index()
        {
            Cart cart = GetCart();
            return View(cart);
        }

        // Current cart state (Total items and total value)
        public JsonResult State()
        {
            Cart cart = GetCart();
            return Json(new { count = cart.GetTotalItems(), value = cart.TotalValue().ToString("c") }, JsonRequestBehavior.AllowGet);
        }

        // Adding or removing an item to cart
        [HttpPost]
        public JsonResult UpdateCart(int id, int quantity)
        {
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                var product = db.Products.Find(id);
                if(product!=null)
                {
                    var currentLine = GetCart().Lines.Where(p => p.Product.ProductId == product.ProductId).FirstOrDefault();
                    int qnt = 1;
                    int totalQnt = 1;
                    if (currentLine != null)
                    {
                        qnt = quantity - currentLine.Quantity;
                        totalQnt = (currentLine.Quantity + qnt);
                    }
                    GetCart().AddItem(product, qnt);
                    return Json(new { price = (totalQnt * product.Price).ToString("c") });
                }
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Error! There is no such item in store" });
            }
        }

        // Removing line from cart
        public ActionResult RemoveLineFromCart(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var product = db.Products.Find(id);
                if (product != null)
                {
                    GetCart().RemoveLine(product);
                    return PartialView("_PartialCart", GetCart());
                }
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Error! There is no such item in store" });
            }
        }
        
        private Cart GetCart()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
    }
}