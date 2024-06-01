using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FEKDMETAK.Data;
using FEKDMETAK.Models;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Authorization;
using FEKDMETAK2.Models;

namespace FEKDMETAK2.Controllers
{
    public class UserController : Controller
    {
        private readonly mvcdbcontext _context;

        public UserController(mvcdbcontext context)
        {
            _context = context;
            var cities = _context.Cities.ToList();
            ViewBag.Cities = cities;
            var towns = _context.Towns.ToList();
            ViewBag.Towns = towns;
        }

        // GET: User
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
            var users = _context.Users.
                Include(u => u.Town)
                .ThenInclude(t => t.City)
                .ToList();
            return View(users);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index2()
        {
            var users = _context.Users.
                Include(u => u.Town)
                .ThenInclude(t => t.City)
                .ToList();
            return View(users);
        }

        // GET: User/Details/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Town)
                .FirstOrDefaultAsync(m => m.Uid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            ViewData["TownId"] = new SelectList(_context.Towns, "Id", "Id");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Uid,Type,FName,LName,email,phone,Adderess,TownId,Gender,specializationId,age,password,ConfirmPassword,Photo")] User user)
        {
           
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewData["TownId"] = new SelectList(_context.Towns, "Id", "Id", user.TownId);
        }

        // GET: User/Edit/5
        [Authorize(Roles = "User")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["TownId"] = new SelectList(_context.Towns, "Id", "Id", user.TownId);
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Uid,Type,FName,LName,email,phone,Adderess,TownId,Gender,specializationId,age,password,ConfirmPassword,Photo")] User user)
        {

            string input = user.password;
            if (!string.IsNullOrEmpty(input))
            {
                user.password = HashPassword.Hashpassword(input);
                user.ConfirmPassword = HashPassword.Hashpassword(input); ;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            if (id != user.Uid)
            {
                return NotFound();
            }

           
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            
            ViewData["TownId"] = new SelectList(_context.Towns, "Id", "Id", user.TownId);
        }

        // GET: User/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Town)
                .FirstOrDefaultAsync(m => m.Uid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'mvcdbcontext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> userbyspe(string Search,int spe)
        {
            if (!String.IsNullOrEmpty(Search))
            {
                var search = await _context.Specializations.Where(n => n.SName.ToLower().Contains(Search.ToLower())).ToListAsync();
                return View(search);
            }
            else
            {
                var users = _context.Users.Where(u => u.specializationId == spe).ToList();

                // You can also include additional data if needed, such as the specialization name
                var specialization = _context.Specializations.FirstOrDefault(s => s.Sid == spe);

                ViewData["Specialization"] = specialization;
                //var users = await _context.Users.ToListAsync();
                return View(users);
            }
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Uid == id)).GetValueOrDefault();
        }
    }
}
