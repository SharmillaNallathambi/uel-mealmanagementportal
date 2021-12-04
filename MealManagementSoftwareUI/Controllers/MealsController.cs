using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityModel;
using MealManagementSoftwareUiLayer.ViewModel;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareDataLayer;

namespace MealManagementSoftwareUiLayer.Controllers
{
    public class MealsController : Controller
    {
        private readonly MealManagementDbContext _context;


        public MealsController(MealManagementDbContext context)
        {
            _context = context;
        }

        // GET: MealMenus
        public async Task<IActionResult> Index()
        {
            var canteenSystemDbContext = _context.MealMenus.Include(m => m.Discount).Include(m => m.MealType);
            return View(await canteenSystemDbContext.ToListAsync());
        }

        // GET: MealMenus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenu = await _context.MealMenus
                .Include(m => m.Discount)
                .Include(m => m.MealType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealMenu == null)
            {
                return NotFound();
            }

            return View(mealMenu);
        }

        // GET: MealMenus/Create
        public IActionResult Create()
        {
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Description");
            ViewData["MealTypeId"] = new SelectList(_context.MealTypes, "Id", "Name");
            return View();
        }

        // POST: MealMenus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MealName,MealTypeId,Price,DiscountId,ImageName")] Dish mealMenu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mealMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Description", mealMenu.DiscountId);
            ViewData["MealTypeId"] = new SelectList(_context.MealTypes, "Id", "Name", mealMenu.MealTypeId);
            return View(mealMenu);
        }

        // GET: MealMenus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenu = await _context.MealMenus.FindAsync(id);
            if (mealMenu == null)
            {
                return NotFound();
            }
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Description", mealMenu.DiscountId);
            ViewData["MealTypeId"] = new SelectList(_context.MealTypes, "Id", "Name", mealMenu.MealTypeId);
            return View(mealMenu);
        }

        // POST: MealMenus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MealName,MealTypeId,Price,DiscountId,ImageName")] Dish mealMenu)
        {
            if (id != mealMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mealMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealMenuExists(mealMenu.Id))
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
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Description", mealMenu.DiscountId);
            ViewData["MealTypeId"] = new SelectList(_context.MealTypes, "Id", "Name", mealMenu.MealTypeId);
            return View(mealMenu);
        }

        // GET: MealMenus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenu = await _context.MealMenus
                .Include(m => m.Discount)
                .Include(m => m.MealType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealMenu == null)
            {
                return NotFound();
            }

            return View(mealMenu);
        }

        // POST: MealMenus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mealMenu = await _context.MealMenus.FindAsync(id);
            _context.MealMenus.Remove(mealMenu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MealMenuExists(int id)
        {
            return _context.MealMenus.Any(e => e.Id == id);
        }

        // GET: MealMenus
        public async Task<IActionResult> StudentMealList(MealMenuCollectionModel model = null)
        {
            // Load meal list from the database including discount and meal type and its availabilities
            var canteenSystemDbContext = _context.MealMenus.Include(m => m.Discount).Include(m => m.MealType)
                .Include(m => m.MealMenuAvailabilities);

            // Covert to list
            var listOfValues = await canteenSystemDbContext.ToListAsync();

            // Get all mealtypes from the database and select as select list items
            var availableMealTypes = _context.MealTypes.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            // If input model is not null and selected meal type is not null then filter by selected meal type.
            if (model != null && model.SelectedMealType != null)
            {
                listOfValues = listOfValues.Where(y => y.MealTypeId == model.SelectedMealType).ToList();
            }

            // based on the above filtered list get meal item where the quantity is greater than or equal to 1 and loop through each one and construct
            // meal menu list to be displayed. 
            var mealMenuList = listOfValues.SelectMany(x => x.MealMenuAvailabilities).Where(x=>x.Quantity>=1).Select(x => {
                decimal price = 0;
                decimal? wasPrice = null;

                // Check if discount is not null and availability date is greater than Discount's ValidFromDate and
                //  Discount's ValidToDate is not null  or availability date is lesser than or equal to Discount's ValidToDate 
                if (x.MealMenu.DiscountId != null && x.AvailabilityDate >= x.MealMenu.Discount.ValidFromDate
                 && (x.MealMenu.Discount.ValidToDate == null || x.AvailabilityDate <= x.MealMenu.Discount.ValidToDate))
                {
                    // If yes then calculated discounted price eg: original price is £10  and discount price is 10% then calculated price would be
                    //  £9.00 
                    price = (decimal)x.MealMenu.Price - ((decimal)((x.MealMenu.Price * x.MealMenu.Discount.DiscountPercentage) / 100));
                    wasPrice = (decimal)x.MealMenu.Price;
                }
                else
                {
                    // else no need to calculate price.
                    price = (decimal)x.MealMenu.Price;
                }

                // Create meal menu model
                return new MealListViewModel
                {
                    Id = x.MealMenuId,
                    Name = x.MealMenu.MealName,
                    MealType = x.MealMenu.MealType.Name,
                    AvailableDate = x.AvailabilityDate,
                    Price = price,
                    DiscountName = x.MealMenu.Discount?.Name,
                    WasPrice = wasPrice,
                    AvailabililtyDateId = x.Id,
                    ImageName = x.MealMenu.ImageName
                };
            }).ToList();

            // if model is null or SelectedAvailableDate is not null then filter menu list where AvailableDate greater than current date
            // or menu list where AvailableDate greater than selected available date.
            mealMenuList = model == null || !model.SelectedAvailableDate.HasValue ?
                   mealMenuList.Where(y => y.AvailableDate.Date >= DateTime.Now.Date).ToList() :
               mealMenuList.Where(y => y.AvailableDate.Date == model.SelectedAvailableDate.Value.Date).ToList();

            // MealMenuCollectionModel  retun the result to UI.
            var mealMenuCollection = new MealMenuCollectionModel
            {
                AvailableMealTypes = new SelectList(availableMealTypes, "Value", "Text"),
                MealMenuModels = mealMenuList,
                SelectedMealType= model?.SelectedMealType,
                SelectedAvailableDate = model?.SelectedAvailableDate,
            };
           

            return View(mealMenuCollection);
        }
    }
}
