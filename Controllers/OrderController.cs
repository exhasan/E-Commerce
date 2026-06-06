using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using MVCProject.Data;
using MVCProject.Dtos;
using MVCProject.Migrations;
using MVCProject.Models;
using Newtonsoft.Json;

namespace MVCProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var cartItems = _context.Carts.Where(c => c.UserId == userId.Value).ToList();

            if (cartItems.Count == 0)
            {
                return View("EmptyCart");
            }

            var orderDto = new OrderDto
            {
                FullName = "", // You can pre-fill this if you have user info
                Address = "",
                PhoneNumber = "",
                PaymentMethod = ""
            };

            return View("OrderForm", orderDto);
        }



        public async Task<IActionResult> PlaceOrder(OrderDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var cartItems = _context.Carts
                .Where(c => c.UserId == userId.Value)
                .ToList();

            if (!cartItems.Any())
            {
                return View("EmptyCart");
            }


            // Convert cart items to JSON
            var productList = cartItems.Where(c => c.Quantity > 0).Select(c => new
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                ImageUrl = c.ImageUrl??"/images/default.png", // Placeholder, you can fetch actual image if needed
                Quantity = c.Quantity,
                UnitPrice = c.TotalPrice / c.Quantity,   // safer (no divide)
                TotalPrice = c.TotalPrice,
                
            }).ToList();

            var order = new Order
            {
                UserId = userId.Value,
                FullName = dto.FullName,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                // IMPORTANT
                ProductListJson = JsonConvert.SerializeObject(productList),

                Quantity = cartItems.Sum(c => c.Quantity),
                TotalProductPrice = cartItems.Sum(c => c.TotalPrice),
                GrandTotal = cartItems.Sum(c => c.TotalPrice),

                PaymentMethod = dto.PaymentMethod,
                OrderNumber = "ORD" + DateTime.Now.Ticks,
                OrderStatus = "Pending",
                OrderTime = DateTime.Now,
                DeliveryDate = DateTime.Now.AddDays(7),

                Email = _context.Users
                    .Where(u => u.Id == userId.Value)
                    .Select(u => u.Email)
                    .FirstOrDefault()
            };

            // ❌ DO NOT set ProductImage anywhere

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return View("OrderSuccess", order);
        }

    }
}
