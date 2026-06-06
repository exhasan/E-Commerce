using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using MVCProject.Data;
using MVCProject.Dtos;
using MVCProject.Migrations;
using MVCProject.Models;

namespace MVCProject.Controllers
{
    public class CartController : Controller
    {

        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult CartItem()
        {
            var products = _context.Carts.ToList(); // get all data from DB
            return View(products); // send to view
        }
// Show all cart product 
        public IActionResult Cart()
        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var product = _context.Carts.FirstOrDefault(p => p.UserId == userId.Value);

            if (product == null)
            {
                return View("CarttoDashboard");

            }
            var cartItem = _context.Carts
                .Where(c => c.UserId == userId.Value).ToList();


            return View("CartItem", cartItem);
        }

// Increase the quantity of product in cart list
        public IActionResult IncreaseQty(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var cartItem = _context.Carts
         .FirstOrDefault(c => c.UserId == userId.Value && c.ProductId == productId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            cartItem.Quantity += 1;
            cartItem.TotalPrice = cartItem.Quantity * product.Price;
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }
// Decrease the quantity of a product in the cart
        public IActionResult DecreaseQty(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var cartItem = _context.Carts
        .FirstOrDefault(c => c.UserId == userId.Value && c.ProductId == productId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            if (cartItem.Quantity >= 0)
            {
                cartItem.Quantity--;
                if(cartItem.Quantity < 0)
                {
                    cartItem.Quantity = 0;
                }
            }
            else
            {
                // optional: remove item if quantity becomes 0
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
                return RedirectToAction("Cart");
            }
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            cartItem.TotalPrice = cartItem.Quantity * product.Price;
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
// Totally remove any product from Cart list
        public IActionResult RemoveFromCart(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var cartItem = _context.Carts
        .FirstOrDefault(c => c.UserId == userId.Value && c.ProductId == productId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            _context.Carts.Remove(cartItem);
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }


    }
}
