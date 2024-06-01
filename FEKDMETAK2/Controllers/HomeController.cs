using FEKDMETAK.Data;
using FEKDMETAK.Models;
using FEKDMETAK2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

//var userlist = new List<User>();
//var app=WebApplication.CreateBuilder(args).Build();
//app.MapControllers();
//app.Run();
//public record User(int id,string name);
namespace FEKDMETAK2.Controllers
{
    public class HomeController : Controller
    {
        private readonly mvcdbcontext _dbContext;
        private readonly IWebHostEnvironment environment;

        public HomeController(mvcdbcontext dbContext, IWebHostEnvironment _envir)
        {
            _dbContext = dbContext;
            environment = _envir;

        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return RedirectToAction("Login");

            }
            ViewBag.email = HttpContext.Session.GetString("email");
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        // GET: /Account/Register

        [HttpGet]
        public IActionResult Register()
        {
            var specializations = _dbContext.Specializations.ToList();
            ViewBag.Specilizations = new SelectList(specializations, "Sid", "SName");


            var cities = _dbContext.Cities.ToList();
            ViewBag.Cities = new SelectList(cities, "Id", "Name");

            var towns = _dbContext.Towns.ToList();
            ViewBag.Towns = towns.Select(t => new { t.Id, t.Name, t.CityId }).ToList();

            return View(new User());
        }

        [HttpPost]
        public IActionResult Register(User user, IFormFile img_file)
        {
            if (user.clientFile != null)
            {
                if (!IsImageValid(user.clientFile))
                {
                    TempData["ErrorMessage"] = "Invalid format. Please upload a .jpg, .jpeg, .png, or .gif image.";
                    return RedirectToAction("Register");
                }

                using (var stream = new MemoryStream())
                {
                    user.clientFile.CopyTo(stream);
                    user.Photo = stream.ToArray();
                }
            }

            ViewBag.Cities = new SelectList(_dbContext.Cities.ToList(), "Id", "Name");
            ViewBag.Towns = _dbContext.Towns.Select(t => new { t.Id, t.Name, t.CityId }).ToList();
            var userTypes = Enum.GetValues(typeof(UserType))
                       .Cast<UserType>()
                       .Where(t => t == UserType.مستخدم || t == UserType.موظف)
                       .Select(t => new SelectListItem
                       {
                           Value = ((int)t).ToString(),
                           Text = t.ToString()
                       });
            if (_dbContext.Users.Any(s => s.email == user.email))
            {
                ViewBag.error = "Email already exists";
                return View();
            }

            if (!string.IsNullOrEmpty(user.password))
            {
                user.password = HashPassword.Hashpassword(user.password);
                user.ConfirmPassword = user.password;
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return RedirectToAction("index", "front");
            }

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return RedirectToAction("Login");
        }
        private bool IsImageValid(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }



        [HttpGet]
        public IActionResult Login()
        {
            
            if (Request.Cookies["LastLogged"] != null)
            {
                ViewBag.ltld = Request.Cookies["LastLogged"].ToString();
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(User model, string returnUrl)
        {
            // Check if the user exists with the given email and hashed password
            var obj = _dbContext.Users.FirstOrDefault(a => a.email.Equals(model.email) && a.password.Equals(HashPassword.Hashpassword(model.password)));

            if (obj != null)
            {
                // Define claims based on the user's role
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, obj.email),
            new Claim(ClaimTypes.Role, obj.Type == UserType.ادمن ? "Admin" : "use"),
            new Claim(ClaimTypes.Role, obj.Type == UserType.موظف ? "employee" : "use"),
            new Claim(ClaimTypes.Role, obj.Type == UserType.مستخدم ? "User" : "use")
        };

                // Create claims identity and authentication properties
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Store user's email and ID in the session
                HttpContext.Session.SetString("email", obj.email);
                HttpContext.Session.SetInt32("userId", obj.Uid);

                // Handle the return URL or redirect based on the user's role
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (obj.Type == UserType.ادمن)
                {
                    return RedirectToAction("AdminPage", "Home");
                }
                else
                {
                    Response.Cookies.Append("LastLogged", DateTime.Now.ToString());
                    return RedirectToAction("Profile", "Home");
                }
            }
            else
            {
                // If the user is not found, add an error to the ModelState
                ModelState.AddModelError("", "Invalid email or password");
            }

            // Return the view with the model if authentication fails
            return View(model);
        }


        private object AuthenticateUser(string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.email == email);

            if (user != null && user.password == password)
            {
                return user;
            }
            return null;

        }

        public IActionResult Privacy()
        {
            return View();

        }
        public IActionResult Dashbord()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return RedirectToAction("Login");

            }
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Profile()
        {

            var users = _dbContext.Users.
                 Include(u => u.Town)
                 .ThenInclude(t => t.City)
                 .ToList();

            //return View(users);
            // Check if user is authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return RedirectToAction("Login");
            }

            //User use=new User();
            // Retrieve email from session
            var userEmail = HttpContext.Session.GetString("email");

            // Query the database to get the user by email
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == userEmail);
            // Check if user exists
            if (user == null)
            {
                // Handle the case where the user does not exist
                // You can redirect to an error page or return an appropriate message
                return RedirectToAction("Error");
            }
            var userId = user.Uid;
            var user1 = _dbContext.Users.FirstOrDefault(u => u.Uid == userId);
            if (user1.Type == UserType.موظف)
            {
                bool hasReviewed = _dbContext.Reviews.Any(r => r.ReviewedUserId == userId);
                if (hasReviewed)
                {
                    ViewBag.check = 1;
                    //decimal? totalrate = _dbContext.Reviews
                    //.Where(r => r.ReviewedUserId == userId)
                    //.Select(r => r.TotalRate)
                    //.FirstOrDefault();

                    //ViewBag.total = totalrate;
                    decimal? totalrate = _dbContext.Reviews
      .Where(r => r.ReviewedUserId == userId)
      .OrderByDescending(r => r.RId)  // Assuming Id is auto-incrementing
      .Select(r => r.TotalRate)
      .FirstOrDefault();

                    ViewBag.total = totalrate;
                }
            }
            var user2 = _dbContext.Users.FirstOrDefault(u => u.Uid == userId);
            if (user2.Type == UserType.موظف)
            {

                // Pass the user object and user ID to the view
                ViewBag.UserId = userId;
                int matchingRowsCount = _dbContext.Notifications.Count(record => record.RecieverId == userId && !record.IsAccepted && !record.IsRejected);
                ViewBag.matchingRowsCount = matchingRowsCount;
            }
          
            // Pass the user object to the view
            if (user.specializationId.HasValue)
            {
                var specialization = await _dbContext.Specializations
                    .FirstOrDefaultAsync(s => s.Sid == user.specializationId.Value);
                ViewBag.SpecializationName = specialization?.SName;
            }
            else
            {
                ViewBag.SpecializationName = "No Specialization";
            }
            return View(user);
        }



        public IActionResult changeProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult changeProfile(int id, [Bind("Id,FName,LName,email,phone,Address,age,TownId")] User user)
        {
            if (id != user.Uid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(user);
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Uid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to the user profile page or any other page after successful update
            }
            return View(user);
        }

        private bool UserExists(int id)
        {
            return _dbContext.Users.Any(e => e.Uid == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //[HttpGet("first")]
        //public ActionResult<User> worker()
        //{
        //    var user = _userServices;
        //    var user =new List<User>();
        //    var use = user.FirstOrDefault();
        //    return View(use);
        //}

    }
}