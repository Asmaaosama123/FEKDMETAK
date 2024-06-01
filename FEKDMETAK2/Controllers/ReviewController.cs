using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FEKDMETAK.Models;
using FEKDMETAK.Data;

namespace FEKDMETAK.Controllers
{
    public class ReviewController : Controller
    {
        private readonly mvcdbcontext _context; 

        public ReviewController(mvcdbcontext context)
        {
            _context = context;
        }
        

        // Action to display the total rate value for a specific user
        public IActionResult Review()
        {
            // Get the reviewer ID from the session (assuming it's stored in a session variable named "UserId")
            int? reviewerId = HttpContext.Session.GetInt32("ReviewerId");
            if (reviewerId == null)
            {
                // Handle case where reviewer ID is not found in session
                return RedirectToAction("Login", "Home"); // Redirect to login page or handle as appropriate
            }

            // Get the reviewed user from the profile or session
            // For example, if the reviewed user ID is stored in a session variable named "ReviewedUserId"
            int? reviewedUserId = HttpContext.Session.GetInt32("ReviewedUserId");
            if (reviewedUserId == null)
            {
                // Handle case where reviewed user ID is not found in session
                return NotFound(); // Redirect to a page indicating the reviewed user is not found
            }

            // Fetch all the reviews given to the reviewed user
            var reviews = _context.Reviews.Where(r => r.ReviewedUserId == reviewedUserId).ToList();
           // var r = _context.Reviews.ToList();
            //foreach (var review in r) {
            //  if(review.ReviewedUserId == reviewedUserId&& review.ReviewerId==reviewerId)
            //    {

            //    }
            //}

            // Calculate the total rate for the reviewed user
            decimal totalRate = reviews.Any() ? reviews.Average(r => r.RateofUser) : 0;
            bool hasReviewed = reviews.Any(r => r.ReviewerId == reviewerId);


            // Pass the reviewed user ID and total rate value to the view
            ViewBag.ReviewedUserId = reviewedUserId;
            ViewBag.TotalRate = totalRate;
            ViewBag.HasReviewed = hasReviewed;

            return View();
        }
    }
}
