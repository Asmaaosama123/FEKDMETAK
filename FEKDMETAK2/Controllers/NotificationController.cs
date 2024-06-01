using FEKDMETAK.Models;
using Microsoft.AspNetCore.Mvc;
using FEKDMETAK.Data;
using FEKDMETAK2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System;
using System.Security.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;


namespace FEKDMETAK2.Controllers
{
    public class NotificationController : Controller
    {
        private readonly mvcdbcontext _context;
        public NotificationController(mvcdbcontext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateNotification()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateNotification(Notification model)
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SentNotificationAsync(int id)
        {
            var notifications = await _context.Notifications
.Include(n => n.Sender).Include(n => n.Reciever) // Ensure the Sender information is included
.ToListAsync();
            bool hasUsersOfType = _context.Users.Any(record => record.Type == UserType.موظف && record.Uid == id);
            if (hasUsersOfType)
            {
                var matchingRows = _context.Notifications.Where(record => record.RecieverId == id && record.IsAccepted == false && record.IsRejected == false).ToList();
                // Pass the matchingRows to the view
                //return View(matchingRows);

                int matchingRowsCount = _context.Notifications.Count(record => record.RecieverId == id && !record.IsAccepted && !record.IsRejected);
                TempData["count"] = matchingRowsCount;
                ViewBag.num = 0;
                var profileUrl = GetProfileUrl(id);
                ViewBag.ProfileUrl = profileUrl;
                return View(matchingRows);

            }
            else
            {
                var matchingRows = _context.Notifications.Where(record => record.SenderId == id && (record.IsAccepted == true || record.IsRejected == true)).ToList();
                // Pass the matchingRows to the view
                //return View(matchingRows);

                int matchingRowsCount = _context.Notifications.Count(record => record.SenderId == id && (record.IsAccepted || record.IsRejected));
                TempData["key"] = matchingRowsCount;
                var url = "/Home/Profile?key=" + TempData["count"];
                ViewBag.num = 1;
                return View(matchingRows);

            }
            return View();
        }

        private string GetProfileUrl(int senderId)
        {
            // Logic to generate the profile URL based on the SenderId
            // Replace this with your actual logic
            var user = _context.Users.FirstOrDefault(u => u.Uid == senderId);
            return user != null ? $"/Home/Profile/{senderId}" : string.Empty;
        }
        [HttpPost]
        public IActionResult getvalue(int reviewed)
        {
            TempData["reviewed"] = reviewed;
            return RedirectToAction("SentNotification");
        }
        [HttpPost]

        public IActionResult SentNotification(string message, DateTime date, Clock clock, AmPm amPm)
        {
            int reviewedId = Convert.ToInt32(TempData["reviewed"]);
            //if (date < DateTime.Today)
            //{
            //    ModelState.AddModelError("Date", "The date cannot be in the past.");
            //    return View("Error", ModelState);  // Adjust the view name as needed
            //}
            var model = new Notification
            {
                Message = message,
                Date = date,
                Clock = clock,
                AmPm = amPm
            };

            User user = new User();
            var senderId = 1;
            //var loggedInUserEmail = User.Identity.Name;
            var loggedInUser = _context.Users.FirstOrDefault(u => u.email == user.email);
            if (loggedInUser != null)
            {
                var UserId = loggedInUser.Uid; // Assuming the user ID property is named "Uid"
                                               // Use userId as needed (e.g., store it in session, pass it to view, etc.)
                HttpContext.Session.SetInt32("userId", UserId);
                senderId = UserId;
                HttpContext.Session.SetInt32("userId", senderId);
                ViewBag.SenderName = $"{loggedInUser.FName} {loggedInUser.LName} {loggedInUser.Photo}";
            }


            bool senderExists = _context.Users.Any(u => u.Uid == senderId);
            // Proceed with saving the Notification object if the sender exists
            if (senderExists)
            {
                Notification notification = new Notification
                {
                    Message = message,
                    Date = date,
                    Clock = clock,
                    AmPm = amPm,
                    SenderId = HttpContext.Session.GetInt32("userId") ?? 0,
                    RecieverId = reviewedId,  // Set the appropriate RecieverId value
                    IsAccepted = false,
                    IsRejected = false
                };
                //var id2 = HttpContext.Session.GetInt32("userId") ?? 0;
                int newNotificationId = notification.NOId;

                _context.Notifications.Add(notification);
                _context.SaveChanges();
                Console.WriteLine("stored successully");             // Handle the case when the sender does not exist

                //TempData["MatchingRows"] = id;
                //RedirectToAction("SentNotification",id);
                //RedirectToAction("Profile","Home", senderId);
                return Ok();
                RedirectToAction("isaccept", newNotificationId);

            }
            else
            {
                Console.WriteLine("cannot store");             // Handle the case when the sender does not exist
                // Return an appropriate error message or redirect the user
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult isaccept([FromBody] JsonElement data, int newNotificationId)
        {
            string action2 = data.GetProperty("action").GetString();
            string notificationId = data.GetProperty("notificationId").GetString();
            int notificationId2 = int.Parse(notificationId);

            var notification = _context.Notifications.FirstOrDefault(n => n.NOId == notificationId2);
            if (notification != null)
            {

                if (action2 == "Accept")
                {
                    notification.IsAccepted = true;
                    notification.IsRejected = false;
                }
                else if (action2 == "Reject")
                {
                    notification.IsAccepted = false;
                    notification.IsRejected = true;
                }
                else
                {
                    // Handle invalid action value
                    return RedirectToAction("OtherAction");
                }

                _context.SaveChanges();
            }

            // Redirect to a different action or return an appropriate view
            return Ok();
        }
        [HttpGet]
        public IActionResult UserRecievedNo()
        {
            //var matchingRows = _context.Notifications.Where(record => record.SenderId == id && (record.IsAccepted == true || record.IsRejected == true)).ToList();

            //int matchingRowsCount = _context.Notifications.Count(record => record.RecieverId == id && !record.IsAccepted && !record.IsRejected);
            //ViewData["count"] = matchingRowsCount;

            // Pass the matchingRows data to the view
            return View();
        }



        //[HttpPost]
        //public IActionResult HandleNotification(string action, string message, DateTime date, Clock clock, AmPm amPm, int SenderId, int ReceiverId, int value)
        //{
        //    try
        //    {
        //        Check if the receiver exists in the Users table
        //        if (!_context.Users.Any(u => u.Uid == ReceiverId))
        //        {
        //            return Json(new { success = false, message = "Receiver does not exist." });
        //        }

        //        Create a new instance of the Notification model and set the values
        //        var notification = new Notification
        //        {
        //            Message = message,
        //            Date = date,
        //            Clock = clock,
        //            AmPm = amPm,
        //            SenderId = HttpContext.Session.GetInt32("userId") ?? 0,
        //            IsAccepted = action == "Accept",
        //            IsRejected = action == "Reject"
        //        };

        //        _context.Add(notification);
        //        _context.SaveChanges();

        //        return Json(new { success = true, message = "Notification saved successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error occurred while saving the notification" });
        //    }
        //}






    }
}