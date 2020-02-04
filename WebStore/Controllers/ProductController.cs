using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class ProductController : Controller
    {
        public int _PageSize = 6;
        byte[] defaultPhoto;
        public List<string> Categories = new List<string>() { "Электроника", "Одежда", "Книги", "Медицина" };

        // GET
        // Editing product
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var product = db.Products.Find(id);
                // Mapping current ProductModel for ProductViewModel
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ProductModel, ProductViewModel>();
                });
                IMapper mapper = config.CreateMapper();
                var item = mapper.Map<ProductModel, ProductViewModel>(product);
                return View(item);
            }
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(ProductViewModel model, HttpPostedFileBase UploadedPhoto)
        {   
            if (ModelState.IsValid)
            {
                // Mapping current ProductViewModel for ProductModel
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ProductViewModel, ProductModel>();
                });
                IMapper mapper = config.CreateMapper();
                var item = mapper.Map<ProductViewModel, ProductModel>(model);


                // Processing input photo file to add to DB

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if (UploadedPhoto != null && UploadedPhoto.ContentLength != 0)
                    {
                        var oldPhoto = db.Photos.Find(item.ProductId);
                        db.Photos.Remove(oldPhoto);
                        ProductPhoto productPhoto = new ProductPhoto();
                        productPhoto.MimeType = UploadedPhoto.ContentType;
                        productPhoto.Photo = new byte[UploadedPhoto.ContentLength];
                        UploadedPhoto.InputStream.Read(productPhoto.Photo, 0, UploadedPhoto.ContentLength);
                        productPhoto.ProductId = item.ProductId;
                        db.Photos.Add(productPhoto);
                    }
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Get", "Product", new { id = item.ProductId });
                }
            }
            return View(model);
        }

        // GET
        // Deletes product by id
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var product = db.Products.Include("Photo").Where(p => p.ProductId == id).FirstOrDefault();
                db.Products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("List", "Product", new { category = product.Category });
            }
        }

        // GET
        // Returns product by id
        public ViewResult Get(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var product = db.Products.Find(id);
                return View(product);
            }
        }

        // GET
        // Returns product's image by id
        public FileContentResult Image(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var productPhoto = db.Photos.Find(id);
                byte[] image;
                string mime = "image/png";
                // if there is no photo of product, return default photo
                if (productPhoto==null||productPhoto.Photo == null)
                {
                    if (defaultPhoto == null)
                    {
                        string defaultProductPhotoPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Content\\Images\\DefaultProductPhoto.png");
                        defaultPhoto = System.IO.File.ReadAllBytes(defaultProductPhotoPath);
                    }
                    image = defaultPhoto;
                }
                else
                {
                    image = productPhoto.Photo;
                    mime = productPhoto.MimeType;
                }
                return File(image, mime);
            }
        }

        // GET
        // Displays list of elements by category
        public ActionResult List(string category, int page = 1)
        {
            if (!Categories.Contains(category))
                return HttpNotFound();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var products = db.Products
                    .Where(p => p.Category == category)
                    .OrderBy(p => p.ProductId)
                    .Skip((page - 1) * _PageSize)
                    .Take(_PageSize)
                    .ToList();
                var itemsCount = db.Products.Where(p => p.Category == category).Count();
                ViewBag.Categories = Categories;
                PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = _PageSize, TotalItems =  itemsCount, CurrentCategory = category};
                ProductListViewModel results = new ProductListViewModel { PageInfo = pageInfo, Products = products };
                return View(results);
            }
        }

        // POST
        // Adds new Product element to the db 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Add(ProductViewModel m, HttpPostedFileBase Photo)
        {
            if (ModelState.IsValid)
            {
                // Mapping current ProductViewModel for ProductModel
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ProductViewModel, ProductModel>();
                });
                IMapper mapper = config.CreateMapper();
                var item = mapper.Map<ProductViewModel, ProductModel>(m);


                ProductPhoto productPhoto = new ProductPhoto();
                // Processing input photo file to add to DB
                if (Photo != null && Photo.ContentLength != 0)
                {
                    productPhoto.MimeType = Photo.ContentType;
                    productPhoto.Photo = new byte[Photo.ContentLength];
                    Photo.InputStream.Read(productPhoto.Photo, 0, Photo.ContentLength);
                }
                item.Photo = productPhoto;
                // Saving Product in DB
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Products.Add(item);
                    db.SaveChanges();
                }
                return RedirectToAction("List", new { category = item.Category });
            }
            return View(m);
        }

        // GET
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            ViewBag.Categories = new SelectList(Categories);
            return View();
        }
    }
}