using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using MVCProject.Data;
using MVCProject.Dtos;
using MVCProject.Migrations;
using MVCProject.Models;

namespace MVCProject.Controllers
{
    public class DashboardController : Controller
    {

        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList(); // get all data from DB
            return View(products); // send to view
        }
        public IActionResult Create()
        {
            return View("AddProduct"); // opens AddProduct.cshtml
        }
        public IActionResult EmptyProduct()
        {
            return View();
        }


        // Add product to sell by user and save in database
        public async Task<IActionResult> AddProduct(ProductDto dto, IFormFile ImageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            string imagePath = null;

            // ✅ image handling added
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                imagePath = "/images/" + fileName;
            }


            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                OwnerId = userId.Value,
                ImageUrl = imagePath
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            var products = _context.Products.ToList(); // get all data from DB
            return RedirectToAction("Index");
        }
        // Delete the product by user who added the product for sell
        public IActionResult Delete(int id)
        {
            var item = _context.Products.FirstOrDefault(c => c.Id == id);
            if (item == null)
                return NotFound();
            _context.Products.Remove(item);
            _context.SaveChanges();
            var userId = HttpContext.Session.GetInt32("UserId");
            var product = _context.Products.Where(p => p.OwnerId == userId.Value).ToList();
            if (product == null)
                return NotFound("Product not found");
            return View("ViewProduct", product);

        }
        // Edit the product details by user who added the product for sell       
        public IActionResult Edit(int Id)
        {
            var product = _context.Products.FirstOrDefault(c => c.Id == Id);
            if (product == null)
                return NotFound();

            var dto = new ProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };
            ViewBag.ProductId = product.Id;
            return View("Edit", dto);
        }
        // Update the product details by user who added the product for sell
        public IActionResult Update(int Id, ProductDto dto)
        {
            var product = _context.Products.FirstOrDefault(c => c.Id == Id);
            if (product == null)
                return NotFound();
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            _context.SaveChanges();
            return RedirectToAction("ViewProducts");
        }

        // Add product to cart and increase cart by 1
        public IActionResult AddToCart(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found");

            var cartItem = _context.Carts
                .FirstOrDefault(c => c.ProductId == id && c.UserId == userId.Value);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                cartItem.TotalPrice = cartItem.Quantity * product.Price;
            }
            else
            {
                cartItem = new Cart
                {
                    ProductId = id,
                    UserId = userId.Value,
                    Quantity = 1,
                    TotalPrice = product.Price,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl
                };
                _context.Carts.Add(cartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        // View all products added by user for sell
        public IActionResult ViewProducts()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var product = _context.Products.Where(p => p.OwnerId == userId.Value).ToList();
            if (!product.Any())
            {
                return RedirectToAction("EmptyProduct");
            }
            return View("ViewProduct", product);
        }
        public IActionResult ProductDetails(int id)
        {
            var product =_context.Products.Where(p=>p.Id==id).ToList();
            return View("ProductDetails",product);
        }

    }
}
