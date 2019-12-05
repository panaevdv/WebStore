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
        public ActionResult Index()
        {
            Cart cart = GetCart();
            return View(cart);
        }

        public JsonResult State()
        {
            Cart cart = GetCart();
            return Json(new { count = cart.GetTotalItems(), value = cart.TotalValue().ToString("c") }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateCart(int id, int quantity)
        {
            using(ProductContext db = new ProductContext())
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
        public ActionResult RemoveLineFromCart(int id)
        {
            using (ProductContext db = new ProductContext())
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