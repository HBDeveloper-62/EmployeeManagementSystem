using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.ViewModel;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace EmployeeManagementSystem.Controllers
{

    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            // Prevent multiple admin registrations
            if (_context.Users.Any(u => u.Role == "Admin"))
            {
                TempData["Error"] = "Admin already exists. Please login.";
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Register(UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check again in case someone bypasses frontend
            if (_context.Users.Any(u => u.Role == "Admin"))
            {
                TempData["Error"] = "Admin already exists. Please login.";
                return RedirectToAction("Login");
            }

            string imagePath = null;
            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }

                imagePath = uniqueFileName;
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(model.Password);
                var hash = sha256.ComputeHash(bytes);
                model.Password = Convert.ToBase64String(hash);
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Contact = model.Contact,
                Password = model.Password,
                ConfirmedPassword = model.Password, // already matched
                ImagePath = imagePath,
                Role = "Admin"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SignupSuccess"] = "Admin registered successfully. Please login.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email or Password cannot be empty.";
                return View();
            }

            // Hash password
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                password = Convert.ToBase64String(hash);
            }

            // Find user
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Set session
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Email", user.Email);

                // ✅ Redirect directly (no role check needed)
                return RedirectToAction("EmployeeInfo", "Employee");
            }

            ViewBag.Error = "Invalid Email or Password.";
            return View();
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login","Account");

        }
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return View(user);
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                Name = user.Name,
                Contact = user.Contact,
                Email = user.Email // For display only
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            user.Name = model.Name;
            user.Contact = model.Contact;

            if (model.NewImage != null)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.NewImage.FileName);
                string filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.NewImage.CopyTo(stream);
                }
                user.ImagePath = fileName;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }





    }
}
