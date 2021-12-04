using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareDataLayer;

namespace MealManagementSoftwareUiLayer.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly MealManagementDbContext _context;

        public OrderItemsController(MealManagementDbContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        [Route("/OrderItems/Index/{orderId}")]
        public async Task<IActionResult> Index(long? orderId)
        {
            var canteenSystemDbContext = orderId == null?
                  _context.OrderItems
                .Include(o => o.MealMenu).Include(o => o.Order):
                _context.OrderItems.Where(x=>x.OrderId == orderId)
                .Include(o => o.MealMenu).Include(o => o.Order);
            return View(await canteenSystemDbContext.ToListAsync());
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(o => o.MealMenu)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName");
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "OrderReference");
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MealMenuId,MealMenuOrderDate,OrderId,Quantity,Price")] DishOrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", orderItem.MealMenuId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "OrderReference", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", orderItem.MealMenuId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "OrderReference", orderItem.OrderId);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,MealMenuId,MealMenuOrderDate,OrderId,Quantity,Price")] DishOrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.Id))
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
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", orderItem.MealMenuId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "OrderReference", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(o => o.MealMenu)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(long id)
        {
            return _context.OrderItems.Any(e => e.Id == id);
        }
    }
}
