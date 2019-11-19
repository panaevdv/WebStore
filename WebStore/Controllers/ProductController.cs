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
        public List<string> Categories = new List<string>() { "Электроника","Одежда","Книги","Медицина"};
        // GET: Product
        public FileContentResult Image(int id)
        {
            using (ProductContext db = new ProductContext())
            {
                var productPhoto = db.Photos.Find(id);
                byte[] image;
                string mime = "image/png";
                // if there is no photo of product, return default photo
                if (productPhoto.Photo == null)
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
        // Displays list of elements
        public ViewResult List(string category, int page = 1)
        {
            using (ProductContext db = new ProductContext())
            {
                var results = db.Products.ToList().Where(p=>p.Category==category)
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
            ViewBag.Categories = new SelectList(Categories);
            return View();
        }
    }
}