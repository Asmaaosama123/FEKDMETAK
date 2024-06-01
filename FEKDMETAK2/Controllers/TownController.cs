using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FEKDMETAK.Data;
using FEKDMETAK2.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace FEKDMETAK2.Controllers
{
    public class TownController : Controller
    {
        private readonly mvcdbcontext _context;

        public TownController(mvcdbcontext context)
        {
            _context = context;
        }

        // GET: Town
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
            var mvcdbcontext = _context.Towns.Include(t => t.City);
            return View(await mvcdbcontext.ToListAsync());
        }

        // GET: Town/Details/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Towns == null)
            {
                return NotFound();
            }

            var town = await _context.Towns
                .Include(t => t.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (town == null)
            {
                return NotFound();
            }

            return View(town);
        }

        // GET: Town/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            //ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id");
                ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name"); // Display city names in the dropdown
           
            return View();
        }

        // POST: Town/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CityId")] Town town)
        {
            
                _context.Add(town);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", town.CityId);
     
        }

        // GET: Town/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Towns == null)
            {
                return NotFound();
            }

            var town = await _context.Towns.FindAsync(id);
            if (town == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", town.CityId);
            return View(town);
        }

        // POST: Town/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CityId")] Town town)
        {
            if (id != town.Id)
            {
                return NotFound();
            }
            try
                {
                    _context.Update(town);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TownExists(town.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
           
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", town.CityId);
            
        }

        // GET: Town/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Towns == null)
            {
                return NotFound();
            }

            var town = await _context.Towns
                .Include(t => t.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (town == null)
            {
                return NotFound();
            }

            return View(town);
        }

        // POST: Town/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Towns == null)
            {
                return Problem("Entity set 'mvcdbcontext.Towns'  is null.");
            }
            var town = await _context.Towns.FindAsync(id);
            if (town != null)
            {
                _context.Towns.Remove(town);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TownExists(int id)
        {
          return (_context.Towns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
