using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityModel;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareDataLayer;

namespace MealManagementSoftwareUiLayer.Controllers
{
    [ClaimRequirement(JwtClaimTypes.Role, "RoleOfAdmin")]
    public class AvailableMealListController : Controller
    {
        private readonly MealManagementDbContext _context;

        public AvailableMealListController(MealManagementDbContext context)
        {
            _context = context;
        }

        // GET: MealMenuAvailabilities
        public async Task<IActionResult> Index()
        {
            var canteenSystemDbContext = _context.MealMenuAvailabilities.Include(m => m.MealMenu);
            return View(await canteenSystemDbContext.ToListAsync());
        }

        // GET: MealMenuAvailabilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenuAvailability = await _context.MealMenuAvailabilities
                .Include(m => m.MealMenu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealMenuAvailability == null)
            {
                return NotFound();
            }

            return View(mealMenuAvailability);
        }

        // GET: MealMenuAvailabilities/Create
        public IActionResult Create()
        {
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName");
            return View();
        }

        // POST: MealMenuAvailabilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MealMenuId,AvailabilityDate,Quantity")] DishAvailability mealMenuAvailability)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mealMenuAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", mealMenuAvailability.MealMenuId);
            return View(mealMenuAvailability);
        }

        // GET: MealMenuAvailabilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenuAvailability = await _context.MealMenuAvailabilities.FindAsync(id);
            if (mealMenuAvailability == null)
            {
                return NotFound();
            }
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", mealMenuAvailability.MealMenuId);
            return View(mealMenuAvailability);
        }

        // POST: MealMenuAvailabilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MealMenuId,AvailabilityDate,Quantity")] DishAvailability mealMenuAvailability)
        {
            if (id != mealMenuAvailability.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mealMenuAvailability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealMenuAvailabilityExists(mealMenuAvailability.Id))
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
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", mealMenuAvailability.MealMenuId);
            return View(mealMenuAvailability);
        }

        // GET: MealMenuAvailabilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenuAvailability = await _context.MealMenuAvailabilities
                .Include(m => m.MealMenu)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealMenuAvailability == null)
            {
                return NotFound();
            }

            return View(mealMenuAvailability);
        }

        // POST: MealMenuAvailabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mealMenuAvailability = await _context.MealMenuAvailabilities.FindAsync(id);
            _context.MealMenuAvailabilities.Remove(mealMenuAvailability);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MealMenuAvailabilityExists(int id)
        {
            return _context.MealMenuAvailabilities.Any(e => e.Id == id);
        }
    }
}
