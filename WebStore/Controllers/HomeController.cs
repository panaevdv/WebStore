using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public int pageSize = 6;
        public ActionResult Index()
        {
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                var products = db.Products
                    .Include("Photo")
                    .OrderByDescending(x => x.CountInStore)
                    .Take(pageSize)
                    .ToList();
                return View(products);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}