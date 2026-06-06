using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using MVCProject.Data;
using MVCProject.Dtos;
using MVCProject.Models;
namespace MVCProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }


        // Register new user and save to database and go to login page
        public IActionResult CreateUser(UserDto dto, IFormFile ImageFile)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                ModelState.AddModelError("", "Please fill all the details");
                return View("Register", dto);
            }

            bool isUserExist = _context.Users.Any(u => u.Email == dto.Email);
            bool isUsernameExist = _context.Users.Any(u => u.Username == dto.Username);

            if (isUsernameExist)
            {
                ModelState.AddModelError("Username", "Username is already in use.");
                return View("Register", dto);
            }

            if (isUserExist)
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View("Register", dto);
            }

            string imagePath = null;

            // ✅ Image upload logic
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // unique file name (VERY IMPORTANT)
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                // folder create if not exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                imagePath = "/images/" + fileName;
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                ImagePath = imagePath, // ✅ save image path
                Mobilenumber = dto.Mobilenumber
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
        // Login and to go index page
        public IActionResult LogedIn(UserDto dto)
        {

            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                ModelState.AddModelError("", "Please fill all the details");
                return View("Login", dto);
            }
            var user = _context.Users
         .FirstOrDefault(u => u.Username == dto.Username && u.Password == dto.Password);
            if (user == null)
            {
                TempData["Error"] = "Wrong Username or Password";
                return View("Login", dto);
            }
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserImage", user.ImagePath);
            return RedirectToAction("Index", "Dashboard");

        }
        // Logout user and clear session also go back Login page
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Redirect to Login page (root URL)
            return RedirectToAction("Login");
        }

        public IActionResult ViewProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return View("ViewProfile", user);
        }

        public IActionResult VerifyPasswordForEditProfile(int UserId, string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == UserId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user.Password != Password)
            {
                TempData["Error"] = "Incorrect password. Please try again.";
                return View("ViewProfile", user);
            }

            var model = new UserDto
            {
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                Mobilenumber = user.Mobilenumber
            };

            ViewBag.UserId = user.Id;
            ViewBag.ImagePath = user.ImagePath;

            return View("EditProfile", model);
        }
        public IActionResult Editprofile(UserDto dto, IFormFile ImageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return NotFound("User not logged in");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
            {
                return NotFound("User not found");
            }

            bool isUserExist = _context.Users.Any(u => u.Email == dto.Email);
            bool isUsernameExist = _context.Users.Any(u => u.Username == dto.Username);

            if (user.Username != dto.Username && isUsernameExist)
            {
                ModelState.AddModelError("Username", "Username is already in use.");
                return View("EditProfile", dto);
            }

            if (user.Email != dto.Email && isUserExist)
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View("EditProfile", dto);
            }

            // 🔥 Image upload logic
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                // Save image path to DB
                user.ImagePath = "/images/" + fileName;
                 HttpContext.Session.SetString("UserImage", user.ImagePath);
            }

            // Update other fields
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.Password = dto.Password;
            user.Mobilenumber = dto.Mobilenumber;

            _context.SaveChanges();

            return RedirectToAction("ViewProfile");
        }

    }
}
