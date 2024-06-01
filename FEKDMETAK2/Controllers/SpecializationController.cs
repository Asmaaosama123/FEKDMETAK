using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FEKDMETAK.Data;
using FEKDMETAK.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace FEKDMETAK2.Controllers
{
    public class SpecializationController : Controller
    {
        private readonly mvcdbcontext _context;

        public SpecializationController(mvcdbcontext context)
        {
            _context = context;
        }
        private decimal CalculateAverageRateForUser(int userId)
        {
            // Retrieve all reviews where the user is either the reviewer or the reviewed user
            var reviews = _context.Reviews.Where(r => r.ReviewedUserId == userId || r.ReviewerId == userId).ToList();

            // Calculate the total rate
            decimal totalRate = reviews.Sum(r => r.RateofUser);

            // Calculate the average rate
            decimal averageRate = reviews.Count > 0 ? totalRate / reviews.Count : 0;

            return averageRate;
        }

        public async Task<IActionResult> Specialization(string Search)
        {
            if (!String.IsNullOrEmpty(Search))
            {
                var search = await _context.Specializations
                    .Where(n => n.SName.ToLower().Contains(Search.ToLower()))
                    .ToListAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_SpecializationList", search);
                }

                return View(search);
            }
            else
            {
                var specializations = await _context.Specializations.ToListAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_SpecializationList", specializations);
                }

                return View(specializations);
            }
        }

        // GET: Specialization
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
              return _context.Specializations != null ? 
                          View(await _context.Specializations.ToListAsync()) :
                          Problem("Entity set 'mvcdbcontext.Specializations'  is null.");
        }

        // GET: Specialization/Details/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Specializations == null)
            {
                return NotFound();
            }

            var specialization = await _context.Specializations
                .FirstOrDefaultAsync(m => m.Sid == id);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        // GET: Specialization/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialization/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([Bind("Sid,SName")] Specialization specialization)
        {
                _context.Add(specialization);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            return View(specialization);
        }

        // GET: Specialization/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Specializations == null)
            {
                return NotFound();
            }

            var specialization = await _context.Specializations.FindAsync(id);
            if (specialization == null)
            {
                return NotFound();
            }
            return View(specialization);
        }

        // POST: Specialization/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Sid,SName")] Specialization specialization)
        {
            if (id != specialization.Sid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecializationExists(specialization.Sid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(specialization);
        }

        // GET: Specialization/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Specializations == null)
            {
                return NotFound();
            }

            var specialization = await _context.Specializations
                .FirstOrDefaultAsync(m => m.Sid == id);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }
        //public IActionResult userbyspe(int uid)
        //{
        //    // Retrieve users belonging to the specified specialization
        //    var users = _context.Users.Where(u => u.Uid == uid).ToList();

        //    // You can also include additional data if needed, such as the specialization name
        //    var User = _context.Users.FirstOrDefault(s => s.Uid == uid);

        //    ViewData["User"] = User;

        //    return View(users);
        //}



        // POST: Specialization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Specializations == null)
            {
                return Problem("Entity set 'mvcdbcontext.Specializations'  is null.");
            }
            var specialization = await _context.Specializations.FindAsync(id);
            if (specialization != null)
            {
                _context.Specializations.Remove(specialization);
            }
            var usersToDelete = await _context.Users.Where(u => u.specializationId == id).ToListAsync();

            // Remove the found users
            _context.Users.RemoveRange(usersToDelete);

            // Remove the specialization
            _context.Specializations.Remove(specialization);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //[HttpGet]
        //public IActionResult UserProfile()
        //{
        //    if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
        //    {
        //        return RedirectToAction("Login", "Home");

        //    }
        //    ViewBag.email = HttpContext.Session.GetString("email");

        //    return View();
        //}
        //[HttpPost]
        //public IActionResult UserProfile(int uid)
        //{


        //    //string reviewerIdString= HttpContext.Session.GetString("email");
        //    //if (int.TryParse(reviewerIdString, out uid))
        //    //{
        //    //    // Valid reviewer ID retrieved from session
        //    //    // ... (use reviewerId)
        //    //}
        //    if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
        //    {
        //        return RedirectToAction("Login", "Home");

        //    }
        //    ViewBag.email = HttpContext.Session.GetString("email");

        //    var user = _context.Users.FirstOrDefault(u => u.Uid == uid);

        //    if (user == null)
        //    {
        //        return NotFound(); // Handle the case where the user is not found
        //    }

        //    int reviewerId;
        //    string reviewerIdString = HttpContext.Session.GetString("ReviewerId");

        //    if (int.TryParse(reviewerIdString, out uid))
        //    {
        //        // Valid reviewer ID retrieved from session

        //        // Get the model for the user being reviewed (replace with your logic)
        //        User reviewedUser = _context.Users.Find(uid);

        //        if (reviewedUser != null)
        //        {
        //            // Access form data (replace with your form names)
        //            int rateOfUser = int.Parse(Request.Form["rateOfUser"]);

        //            // Create a new review object
        //            var review = new Review
        //            {
        //                ReviewerId = uid,
        //                ReviewedUserId = reviewedUser.Uid, // Use Id property from User model
        //                RateofUser = rateOfUser
        //            };


        //            _context.Reviews.Add(review);
        //            _context.SaveChanges();

        //            // ... other logic after saving review (e.g., calculate total rate)
        //        }
        //        else
        //        {
        //            // Handle case where reviewed user is not found
        //            ModelState.AddModelError("", "User not found.");
        //        }
        //        ViewBag.reviwer = uid;
        //        ViewBag.Uid = reviewedUser.Uid;
        //    }
        //    else
        //    {
        //        // Handle case where reviewer ID is not found in session
        //        ModelState.AddModelError("", "Reviewer ID not available.");
        //    }

        //    // ... other logic to prepare the view model

        //    return View(user);
        //}
        //public IActionResult UserProfile(int uid)
        //{
        //    // Check if the user is logged in
        //    if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    // Get the logged-in user's ID
        //    int reviewerId = HttpContext.Session.GetInt32("LoggedUserId") ?? 0;

        //    // Retrieve the user being reviewed
        //    var reviewedUser = _context.Users.FirstOrDefault(u => u.Uid == uid);
        //    if (reviewedUser == null)
        //    {
        //        return NotFound(); // Handle the case where the user is not found
        //    }
        //    var users = _context.Users.ToList();
        //    ViewBag.user = users.FindIndex(u => u.Uid == uid);
        //    ViewBag.userId = reviewerId;

        //    if (Request.Method == "POST")
        //    {
        //        // Retrieve the rate of the user from the form
        //        if (!int.TryParse(Request.Form["rateOfUser"], out int rateOfUser))
        //        {
        //            // Invalid rate value, handle the error
        //            ModelState.AddModelError("", "Invalid rate value.");
        //            return View(reviewedUser);
        //        }

        //        // Create a new review object
        //        var review = new Review
        //        {
        //            ReviewerId = reviewerId,
        //            ReviewedUserId = reviewedUser.Uid,
        //            RateofUser = rateOfUser
        //        };

        //        // Add the review to the database
        //        _context.Reviews.Add(review);
        //        _context.SaveChanges();

        //        // Perform any additional logic (e.g., calculate total rate)

        //        // Redirect to the user profile page
        //        return RedirectToAction("UserProfile", new { uid });
        //    }

        //    // If it's a GET request, simply display the user profile
        //    return View(reviewedUser);
        //}

        [HttpGet]
        [Route("Specialization/UserProfile/{uid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UserProfileAsync(int uid,int value)
        {
            var users = _context.Users.
                Include(u => u.Town)
                .ThenInclude(t => t.City)
                .ToList();
            // Check if the user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return RedirectToAction("Login", "Home");
            }

            // Get the logged-in user's ID
            // Get the logged-in user's ID (Reviewer's ID)
            int reviewerId = HttpContext.Session.GetInt32("userId") ?? 0;
            HttpContext.Session.SetInt32("reviewerId", reviewerId);
            HttpContext.Session.SetInt32("reviewedUserId", uid);

            // Retrieve the user being reviewed
            var reviewedUser = _context.Users.FirstOrDefault(u => u.Uid == uid);
            if (reviewedUser == null)
            {
                return NotFound(); // Handle the case where the user is not found
            }



           // var users = _context.Users.ToList();
         //   ViewBag.user = users.FindIndex(u => u.Uid == uid);
          //  ViewData["reciverid"] = ViewBag.user;
         //   ViewBag.userId = reviewerId;
            ViewBag.userId = reviewerId;
            ViewBag.reviewed = uid;
            var user = _context.Users.FirstOrDefault(u => u.Uid == uid);
            bool hasReviewed = _context.Reviews.Any(r => r.ReviewedUserId == uid && r.ReviewerId == reviewerId);
            if (hasReviewed)
            {
                ViewBag.check = 1;
                decimal? rate = _context.Reviews
                 .Where(r => r.ReviewedUserId == uid && r.ReviewerId == reviewerId)
                 .Select(r => r.RateofUser)
                 .FirstOrDefault();
                decimal? totalrate = _context.Reviews
                .Where(r => r.ReviewedUserId == uid && r.ReviewerId == reviewerId)
                .Select(r => r.TotalRate)
                .FirstOrDefault();

                ViewBag.rate = rate;
                ViewBag.total = totalrate;
            }
            if (reviewedUser.specializationId.HasValue)
            {
                var specialization = await _context.Specializations
                    .FirstOrDefaultAsync(s => s.Sid == reviewedUser.specializationId.Value);
                ViewBag.SpecializationName = specialization?.SName;
            }
            else
            {
                ViewBag.SpecializationName = "No Specialization";
            }
            //   pass((int)ViewData["reciverid"]);
            // Display the user profile form
            //reviewedUser
            return View(reviewedUser);
        }
        public int pass(int value)
        {
            return value;
        }
        public IActionResult recieverid()
        {
            int Id = pass((int)ViewData["reciverid"]);
            return RedirectToAction("SentNotification", "Notification", new { id = Id });
        }
        [HttpPost]
        public IActionResult UserProfile(int uid)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return RedirectToAction("Login", "Home");
            }

            int reviewerId = HttpContext.Session.GetInt32("reviewerId") ?? 0;

            var loggedInUser = _context.Users.FirstOrDefault(u => u.email == HttpContext.Session.GetString("email"));
            if (loggedInUser != null)
            {
                HttpContext.Session.SetInt32("userId", loggedInUser.Uid);
                ViewBag.userId = loggedInUser.Uid;
            }

            var reviewedUser = _context.Users.FirstOrDefault(u => u.Uid == uid);
            if (reviewedUser == null)
            {
                return NotFound();
            }

            var existingReview = _context.Reviews.FirstOrDefault(r => r.ReviewedUserId == uid && r.ReviewerId == reviewerId);
            bool hasReviewed = existingReview != null;

            decimal totalRate = _context.Reviews.Where(r => r.ReviewedUserId == uid).Any()
                ? _context.Reviews.Where(r => r.ReviewedUserId == uid).Average(r => r.RateofUser)
                : 0;

            ViewBag.ReviewedUserId = uid;
            ViewBag.TotalRate = totalRate;
            ViewBag.HasReviewed = hasReviewed;

            return View(reviewedUser);
        }


        public IActionResult AddReview( int id , int rateOfUser)
        {

            Console.WriteLine(id + "aaaaaaaaaaaa2aaaaaaaaaaa");
            Console.WriteLine(rateOfUser + "aaaaaaaa2aaaaaaaaaaa");
            Review review = new Review();
            review.ReviewedUserId = id;
            review.ReviewerId= HttpContext.Session.GetInt32("reviewerId") ?? 0;

            review.RateofUser = rateOfUser;
            var res = _context.Reviews.Where(r => r.ReviewedUserId  ==id).Sum(r => r.RateofUser);
            var cnt = _context.Reviews.Where(r => r.ReviewedUserId == id).Count();
            res += rateOfUser;
            cnt++;
            review.TotalRate = Convert.ToInt32( res / cnt);
            

            _context.Reviews.Add(review);
            _context.SaveChanges();
            Console.WriteLine("endaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            //return View();////jason or partial
            return PartialView("SetRate", review);
        }




            private bool SpecializationExists(int id)
        {
          return (_context.Specializations?.Any(e => e.Sid == id)).GetValueOrDefault();
        }
    }
}
