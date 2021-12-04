using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; 
using MealManagementSoftwareUiLayer.ViewModel;
using System.Net.Mail;
using System.Net;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareDataLayer;

namespace MealManagementSoftwareUiLayer.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MealManagementDbContext _context;

        public OrdersController(MealManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OrderConfirmation(string notificationMessage)
        { 
            return View(new ProductOrderConfirmViewModel(notificationMessage));
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var canteenSystemDbContext = _context.Orders.Include(o => o.UserProfile)
                .Include(o => o.Payments);
            return View(await canteenSystemDbContext.ToListAsync());
        }

       [Route("orders/studentorder/{userId}")]
        public async Task<IActionResult> StudentOrder(int userId)
        {
            var canteenSystemDbContext = _context.Orders.Where(x => x.UserProfileId == userId)
                .Include(o => o.UserProfile).Include(o => o.Payments);
               
            var orders = await canteenSystemDbContext.ToListAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Department");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderReference,UserProfileId,TotalPrice,CreatedDate,UpdatedDate")] DishOrder order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Department", order.UserProfileId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Department", order.UserProfileId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,OrderReference,UserProfileId,TotalPrice,CreatedDate,UpdatedDate")] DishOrder order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Department", order.UserProfileId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

    }
}
