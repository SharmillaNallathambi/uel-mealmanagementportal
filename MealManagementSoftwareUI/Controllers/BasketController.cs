using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; 
using Newtonsoft.Json;
using MealManagementSoftwareUiLayer.ViewModel;
using System.Net.Mail;
using System.Net;
using MealManagementSoftwareDataLayer;
using MealManagementSoftwareDataLayer;

namespace MealManagementSoftwareUiLayer.Controllers
{
    public class BasketController : Controller
    {
        private readonly MealManagementDbContext _context;

        public BasketController(MealManagementDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index(int userProfileId)
        {
            var canteenSystemDbContext = _context.Carts.Include(c => c.MealMenu).Include(c => c.UserProfile)
                .Include(c => c.UserProfile.Cards)
                .Include(c => c.MealMenu.MealMenuAvailabilities);
            var cartList = await canteenSystemDbContext.ToListAsync();
            var itemToRemove = new List<Basket>();
            foreach (var item in cartList)
            {
                var availableQuantity = item.MealMenu.MealMenuAvailabilities.
                    FirstOrDefault(x => x.AvailabilityDate == item.MealAvailableDate)?.Quantity;
                if (availableQuantity< item.Quantity)
                {
                    itemToRemove.Add(item);
                }
                if (item.MealAvailableDate < DateTime.Now)
                {
                    itemToRemove.Add(item);
                }
            } 
            if (itemToRemove != null && itemToRemove.Any())
            {
                _context.RemoveRange(itemToRemove);
                cartList = await _context.Carts.Include(c => c.MealMenu).Include(c => c.UserProfile).ToListAsync();
                  
            } 

            cartList = cartList.Where(x => x.UserProfileId == userProfileId)?.ToList();
            return View(cartList);
        }

        [Route("basket/AddToCart/{menuId}/{availabilityDateId}/{userProfileId}")]
        [HttpPost]
        public async Task<IActionResult> Add(int menuId, int availabilityDateId, int userProfileId)
        {
            try
            {
                var canteenSystemDbContext = _context.MealMenus.Where(x => x.Id == menuId &&
                x.MealMenuAvailabilities.Any(y => y.Id == availabilityDateId)).Include(x => x.MealMenuAvailabilities);
                var mealMenu = await canteenSystemDbContext.FirstOrDefaultAsync();
                if (mealMenu != null)
                {
                    var availableDate = mealMenu.MealMenuAvailabilities.
                        Where(x => x.Id == availabilityDateId).FirstOrDefault().AvailabilityDate;

                    var isExistingOrder = _context.Carts.FirstOrDefault(x => x.MealMenuId == menuId &&
                    x.MealAvailableDate == availableDate && x.UserProfileId == userProfileId);
                    if (isExistingOrder == null)
                    {
                        var cart = new Basket
                        {
                            MealMenuId = mealMenu.Id,
                            MealAvailableDate = availableDate,
                            Price = mealMenu.Price,
                            Quantity = 1,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            UserProfileId = userProfileId
                        };
                        _context.Add(cart);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return Json(new { Status = false });
                    }
                }

                return Json(new { Status = true });
            }
            catch (Exception ex) {
                //jhgh
            }
            return Json(new { Status = false });
        }


        [Route("basket/remove/{cartId}")]
        [HttpPost]
        public async Task<IActionResult> Remove(int cartId)
        {
            try
            {
                var cart = await _context.Carts.FindAsync(cartId);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync(); 
                return Json(new { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false });
            }
        }



        [Route("basket/update/{cartId}/{selectedQuantity}")]
        [HttpPost]
        public async Task<IActionResult> Update(int cartId,int selectedQuantity)
        {
            try
            {
                var cart = await _context.Carts.FindAsync(cartId);
                if (cart != null)
                {
                    var mealMenuAvailabilities =  _context.MealMenuAvailabilities
                        .Where(x=>x.MealMenuId == cart.MealMenuId &&
                        x.AvailabilityDate == cart.MealAvailableDate).FirstOrDefault();
                    if (mealMenuAvailabilities != null)
                    {
                        if (mealMenuAvailabilities.Quantity >= 1 && mealMenuAvailabilities.Quantity >= selectedQuantity)
                        {
                            cart.Quantity = selectedQuantity;
                            _context.Update(cart);
                            await _context.SaveChangesAsync();
                        }
                        else {
                            return Json(new {Status= false, Message = "Selected quantity is not available." });
                        }
                    }
                }

                return Json(new { Status = true, Message = "Updated the quantity" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Unexpected error occurred, please try again later." });
            }
        }
 

        [Route("Basket/Confirm/{userId}")]
        public async Task<IActionResult> Confirm(int userId)
        {
            var cartItems = await _context.Carts.Where(x => x.UserProfileId == userId).ToListAsync();
            decimal totalPrice = 0M;
            if (cartItems != null)
            {

                var orderItems = new List<DishOrderItem>();
              
                cartItems.ForEach(x => {
                    totalPrice += (decimal)x.Price * x.Quantity;
                    orderItems.Add(new DishOrderItem
                    {
                        MealMenuId = x.MealMenuId,
                        MealMenuOrderDate = x.MealAvailableDate,
                        Price = x.Price,
                        Quantity = x.Quantity
                    });

                });
            }
                return View("confirm",new PaymentCardViewModel { Amount=  totalPrice,UserProfileId = userId });
        }


        [Route("Basket/Confirm")]
        [HttpPost]
        public async Task<IActionResult> Confirm(PaymentCardViewModel model)
        {
            try
            { 
             
         
                Random generator = new Random();
                int r = generator.Next(1000, 10000000);
                var orderReferenceNumber = $"ORDER{r}";
                var cartItems = await _context.Carts.Where(x => x.UserProfileId == model.UserProfileId).ToListAsync();
                if (cartItems != null)
                {

                    var orderItems = new List<DishOrderItem>();
                    decimal totalPrice = 0M;
                    cartItems.ForEach(x => {
                        totalPrice += (decimal)x.Price * x.Quantity;
                        orderItems.Add(new DishOrderItem
                        {
                            MealMenuId = x.MealMenuId,
                            MealMenuOrderDate = x.MealAvailableDate,
                            Price = x.Price,
                            Quantity = x.Quantity
                        });

                    });

                    var order = new DishOrder
                    {
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        OrderReference = orderReferenceNumber,
                        TotalPrice = (double)totalPrice,
                        UserProfileId = model.UserProfileId,
                        OrderItems = orderItems
                    };
                    

                    _context.Add(order);
                    _context.SaveChanges();

                    var paymentReferenceNumber = $"PAYMENT{r}";
                    var payment = new OrderPayment
                    {
                        PaymentDate = DateTime.Now,
                        PaymentReference = paymentReferenceNumber,
                        PaymentAmount = (double)totalPrice,
                        OrderId = order.Id 
                    };
                    _context.Add(payment);
                    _context.SaveChanges();
                    var cardDetail = await _context.Cards.Where(x => x.UserProfileId == model.UserProfileId).FirstOrDefaultAsync();
                    if(cardDetail != null)
                    { 
                        cardDetail.AvailableBalance = cardDetail.AvailableBalance - totalPrice;
                        _context.Update(cardDetail);
                        _context.SaveChanges();
                    }
                    var cartToBeRemoved = _context.Carts.Where(c => c.UserProfileId == model.UserProfileId);
                    _context.Carts.RemoveRange(cartToBeRemoved);
                    await _context.SaveChangesAsync();

                    // Update meal menu quantity
                    foreach (var item in cartItems)
                    {
                        var mealMenuAvailability = _context.MealMenuAvailabilities
                            .FirstOrDefault(x => x.MealMenuId == item.MealMenuId && x.AvailabilityDate == item.MealAvailableDate);
                        if (mealMenuAvailability != null)
                        {
                            mealMenuAvailability.Quantity = mealMenuAvailability.Quantity - item.Quantity;
                            _context.Update(mealMenuAvailability);
                            await _context.SaveChangesAsync();
                        }
                    }

                    } 

                return RedirectToAction("OrderConfirmation", "Orders",
                    new ProductOrderConfirmViewModel($"Your order has been confirmed and the reference number is {orderReferenceNumber}. " +
                    $"Please collect your order"));
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Unexpected error occurred, please try again later." });
            }
            return Json(new { Status = false, Message = "Unexpected error occurred, please try again later." });
        }
        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.MealMenu)
                .Include(c => c.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName");
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "EmailAddress");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MealMenuId,Quantity,Price,CreatedDate,UpdatedDate,MealAvailableDate,UserProfileId")] Basket cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", cart.MealMenuId);
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "EmailAddress", cart.UserProfileId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", cart.MealMenuId);
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "EmailAddress", cart.UserProfileId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MealMenuId,Quantity,Price,CreatedDate,UpdatedDate,MealAvailableDate,UserProfileId")] Basket cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", cart.MealMenuId);
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "EmailAddress", cart.UserProfileId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.MealMenu)
                .Include(c => c.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private string ConstructStudentOrderEmail(string toName, DishOrder order, bool isPaid)
        {
            var body = "";
          
            body += $"Hi {toName},\n\r"; 
            if (isPaid)
            {
                body += $"Please use your order reference number to collect your order. \n\r";
            }
            else
            {
                body += $"Please pay at the till and collect your order \n\r";
            }
            body += $"Your order reference number is :{order.OrderReference}\n\r";
            if (isPaid)
            {
                body += $"Paid: £ {order.TotalPrice} \n\r";
            }
            else
            {
                body += $"Need to pay: £ {order.TotalPrice} \n\r";
            }

            body += $"Please login to the Canteen reservation system to view the order details.\n\r";
          
            return body;
        }

       

        private bool SendEmail(string toEmailAddress, string toName, string subject, string body)
        {
            var fromAddress = new MailAddress("admin@gmail.com", "Admin");
            var toAddress = new MailAddress(toEmailAddress, toName);
            const string fromPassword = "Test1234";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
            return true;
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}
