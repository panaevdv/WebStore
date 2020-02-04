using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebStore.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Страна")]
        public string Country { get; set; }

        [Display(Name = "Город")]
        public string City { get; set; }

        [Display(Name = "Улица")]
        public string Street { get; set; }

        [Display(Name = "Дом")]
        public int House { get; set; }

        [Display(Name = "Строение")]
        public string Building { get; set; }
        public List<Purchase> Purchases { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ProductPhoto> Photos { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<CartLine> CartLines { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class AppDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var adminRole = new IdentityRole { Name = "Admin" };
            var userRole = new IdentityRole { Name = "User" };
            roleManager.Create(adminRole);
            roleManager.Create(userRole);

            var admin = new ApplicationUser
            {
                Email = "dimon@hu2.ru",
                UserName = "dimon@hu2.ru",
                FirstName = "Dmitry",
                LastName = "Panaev",
                City = "Ufa",
                Country = "Russia",
                House = 222,
                Street = "Odesskaya"
            };
            string password = "TestPass123";
            var result = userManager.Create(admin, password);

            if (result.Succeeded)
            {
                userManager.AddToRole(admin.Id, userRole.Name);
                userManager.AddToRole(admin.Id, adminRole.Name);
            }
            for (int i = 0; i < 16; i++)
            {
                ProductModel product;
                if (i < 5)
                {
                    product = new ProductModel()
                    {
                        Category = "Медицина",
                        CountInStore = 100,
                        Description = "None",
                        Name = "Product" + i,
                        Price = 100 * i,
                        Subcategory = "Расходные материалы"
                    };
                }
                else
                {
                    product = new ProductModel()
                    {
                        Category = "Электроника",
                        CountInStore = 100,
                        Description = "None",
                        Name = "Product" + i,
                        Price = 5000 * i,
                        Subcategory = "Телефоны"
                    };
                }
                context.Products.Add(product);

            }
            base.Seed(context);
        }
    }
}