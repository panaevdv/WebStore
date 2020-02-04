using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class PurchaseController : Controller
    {
        // GET: Purchase
        // List of user's purchases
        [Authorize]
        public ActionResult Index()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var purchases = db.Purchases.Include("Items").Include("Items.Product").OrderByDescending(x => x.PurchaseTime).ToList();
                return View(purchases);
            }
        }


        // GET
        // Adding a new purchase
        [Authorize]
        public ActionResult Add()
        {
            Cart cart = GetCart();
            if (cart != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var user = db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    Purchase purchase = new Purchase
                    {
                        PurchaseTime = DateTime.Now,
                        ApplicationUser = user,
                        ApplicationUserId = userId,
                        TotalValue = cart.TotalValue()
                    };
                    for (int i = 0; i < cart.Lines.Count(); i++)
                    {
                        // Changing in-stock quantity
                        var product = db.Products.Find(cart.Lines.ElementAt(i).Product.ProductId);
                        product.CountInStore -= cart.Lines.ElementAt(i).Quantity;
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;

                        // Adding products to a new purchase
                        cart.Lines.ElementAt(i).Product = null;
                        cart.Lines.ElementAt(i).PurchaseId = purchase.Id;
                        db.CartLines.Add(cart.Lines.ElementAt(i));
                    }
                    db.Purchases.Add(purchase);
                    db.SaveChanges();
                }
                Session["Cart"] = new Cart();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Cart");
        }

        public Cart GetCart()
        {
            Cart cart = (Cart)Session["Cart"];
            return cart;
        }
    }
}