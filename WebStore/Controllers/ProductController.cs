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
        public int PageSize = 9;
        byte[] defaultPhoto;
        // GET: Product
        public FileContentResult Image(int id)
        {
            using (ProductContext db = new ProductContext())
            {
                var photo = db.Photos.Find(id);
                return File(photo.Photo, photo.MimeType);
            }
        }
        // Displays list of elements
        public ViewResult List(int page = 1)
        {
            using (ProductContext db = new ProductContext())
            {
                var results = db.Products.ToList()
                    .OrderBy(p => p.ProductId)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize);
                return View(results);
            }
        }
        // Adds new Product element to the db 
        [HttpPost]
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
                // If there were no uploaded photo, we'll use default photo
                else
                {
                    if (defaultPhoto == null)
                    {
                        string defaultProductPhotoPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Content\\Images\\DefaultProductPhoto.png");
                        defaultPhoto = System.IO.File.ReadAllBytes(defaultProductPhotoPath);
                    }
                    productPhoto.Photo = defaultPhoto;
                    productPhoto.MimeType = "image/png";
                }
                item.Photo = productPhoto;
                // Saving Product in DB
                using (ProductContext db = new ProductContext())
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