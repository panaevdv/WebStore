﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class ProductController : Controller
    {
        public int PageSize = 9;
        // GET: Product
        public ViewResult List(int page = 1)
        {
            ProductModel product = new ProductModel() { Name = "Памперсы", Description = "Памерсы для детей", Price = 15.60, Category="Медицина" };
            using (ProductContext db = new ProductContext())
            {
                return View(db.Products.ToList());
            }
        }

        [HttpPost]
        public ActionResult Add(ProductViewModel m)
        {
            if(ModelState.IsValid)
            {
                // Mapping current ProductViewModel for ProductModel
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ProductViewModel, ProductModel>();
                });
                IMapper mapper = config.CreateMapper();
                var item = mapper.Map<ProductViewModel, ProductModel>(m);

                // Saving Product in DB
                using(ProductContext db = new ProductContext())
                {
                    db.Products.Add(item);
                    db.SaveChanges();
                }
                return RedirectToAction("List");
            }
            return View(m);
        }
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
    }
}